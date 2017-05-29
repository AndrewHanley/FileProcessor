//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: FieldBase.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Fields
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Reflection;
    using Converters;
    using Exceptions;
    using FieldAttributes;
    using FieldAttributes.FormatAttributes;
    using Resources;
    using Validation;

    public abstract class FieldBase
    {
        #region Properties

        public Type FieldType { get; set; }

        public int Order { get; set; }

        public string Name { get; set; }

        public bool ThrowExceptionOnNull { get; set; }

        public string FormatString { get; set; }

        public IFieldConverter FieldConverter { get; set; }

        public PropertyInfo FieldProperty { get; set; }

        #endregion

        #region Constructors

        protected FieldBase(PropertyInfo property)
        {
            FieldProperty = property;
            FieldType = property.PropertyType;
            Order = -1;
            Name = property.Name;
            ThrowExceptionOnNull = true;
            FieldConverter = null;

            ProcessFieldAttribute(property);
        }

        #endregion

        #region Property Attribute Processing

        private void ProcessFieldAttribute(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<FieldAttribute>();

            if (attribute != null)
            {
                Order = attribute.Order;
                Name = attribute.Name ?? Name;
                ThrowExceptionOnNull = attribute.ThrowExceptionOnNull;
                FormatString = attribute.FormatString;
                FieldConverter = attribute.FieldConverter == null ? null : (IFieldConverter) Activator.CreateInstance(attribute.FieldConverter);
            }
        }

        #endregion

        #region Conversion: To Value

        public virtual object ConvertToValue(string value)
        {
            try
            {
                if (FieldConverter != null && FieldConverter.CanConvertToValue())
                    return FieldConverter.ConvertToValue(value, FieldProperty);

                if (string.IsNullOrWhiteSpace(value) || string.IsNullOrEmpty(value))
                    return GetNullValue();

                value = value.Trim();

                if (FieldType == typeof(bool))
                    return ConvertToBoolean(value);

                if (FieldType == typeof(decimal))
                    return ConvertToDecimal(value);

                if (FieldType == typeof(DateTime) && !string.IsNullOrEmpty(FormatString))
                    return ConvertToDateTime(value);

                // If property type is Nullable then create new Type that isn't nullable as Convert.ChangeType cannot convert 
                // Nullable fields and NULL has already been checked above
                var t = FieldType;
                t = Nullable.GetUnderlyingType(t) ?? t;

                return Convert.ChangeType(value, t, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                throw new ConversionException(Name, value, typeof(string), FieldType, e);
            }
        }

        protected virtual bool ConvertToBoolean(string value)
        {
            var formatAttribute = FieldProperty.GetCustomAttribute<BooleanFormatAttribute>();

            if (formatAttribute != null)
            {
                if (formatAttribute.TrueValue == null)
                    throw new AttributeException(ExceptionMessages.InvalidBooleanAttribute);

                return value == formatAttribute.TrueValue;
            }

            bool boolValue;

            if (!bool.TryParse(value, out boolValue))
                throw new FormatException(string.Format(ExceptionMessages.InvalidBooleanValue, value));

            return boolValue;
        }

        protected virtual decimal ConvertToDecimal(string value)
        {
            var formatAttribute = FieldProperty.GetCustomAttribute<DecimalFormatAttribute>();

            decimal decimalValue;

            if (!decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out decimalValue))
                throw new FormatException(string.Format(ExceptionMessages.InvalidDecimalValue, value));

            if (formatAttribute != null && formatAttribute.DecimalPlaces != -1)
                return formatAttribute.IncludesDecimalSeperator
                    ? Math.Round(decimalValue, formatAttribute.DecimalPlaces)
                    : decimalValue / (decimal) Math.Pow(10, formatAttribute.DecimalPlaces);

            return decimalValue;
        }

        protected virtual DateTime ConvertToDateTime(string value)
        {
            DateTime parseValue;

            if (!DateTime.TryParseExact(value, FormatString, CultureInfo.InvariantCulture, DateTimeStyles.None, out parseValue))
                throw new FormatException(string.Format(ExceptionMessages.InvalidDateTimeFormat, value, FormatString));

            return parseValue;
        }

        protected virtual object GetNullValue()
        {
            if (Nullable.GetUnderlyingType(FieldType) != null)
                return null;

            if (ThrowExceptionOnNull)
                throw new NullReferenceException(ExceptionMessages.NullValue);

            if (FieldType.GetTypeInfo().IsValueType)
                return Activator.CreateInstance(FieldType);

            return null;
        }

        #endregion

        #region Conversion: To String

        public virtual string ConvertToString(object value)
        {
            if (FieldConverter != null && FieldConverter.CanConvertToString())
                return FieldConverter.ConvertToString(value, FieldProperty);

            if (value == null)
                return null;

            if (value is string)
                return ConvertString((string) value);

            if (value is bool)
                return ConvertBoolean((bool) value);

            if (value is DateTime)
                return ConvertDateTime((DateTime) value);

            if (value is decimal)
                return ConvertDecimal((decimal) value);

            if (value is ValueType)
                return ConvertValueType((ValueType) value);

            throw new FormatException(ExceptionMessages.UnexpectedDataType);
        }

        protected virtual string ConvertString(string value)
        {
            return value.Trim();
        }

        protected virtual string ConvertBoolean(bool value)
        {
            var attribute = FieldProperty.GetCustomAttribute<BooleanFormatAttribute>();

            if (attribute != null)
            {
                if (attribute.TrueValue == null ||
                    attribute.FalseValue == null ||
                    attribute.TrueValue == attribute.FalseValue)
                    throw new AttributeException(ExceptionMessages.InvalidBooleanAttribute);

                return value
                    ? attribute.TrueValue
                    : attribute.FalseValue;
            }

            return value.ToString();
        }

        protected virtual string ConvertDateTime(DateTime value)
        {
            if (value.Equals(DateTime.MinValue))
                return null;

            return string.IsNullOrEmpty(FormatString)
                ? value.ToString(CultureInfo.CurrentCulture)
                : value.ToString(FormatString);
        }

        protected virtual string ConvertDecimal(decimal value)
        {
            var formatAttribute = FieldProperty.GetCustomAttribute<DecimalFormatAttribute>();
            var formatString = string.IsNullOrEmpty(FormatString) 
                ? "F"
                : FormatString;

            if (formatAttribute != null && formatAttribute.DecimalPlaces != -1)
            {
                if (formatAttribute.IncludesDecimalSeperator)
                {
                    formatString = $"F{formatAttribute.DecimalPlaces}";
                }
                else
                {
                    value = value * (decimal)Math.Pow(10, formatAttribute.DecimalPlaces);
                    formatString = "F0";
                }                
            }

            return string.Format($"{{0:{formatString}}}", value); ;
        }

        protected virtual string ConvertValueType(object numericValue)
        {
            return string.IsNullOrEmpty(FormatString)
                ? numericValue.ToString()
                : string.Format($"{{0:{FormatString}}}", numericValue);
        }

        #endregion

        #region Validation

        public IEnumerable<FieldValidationError> Validate(object value, ValidationContext context)
        {
            var results = new List<ValidationResult>();

            if (Validator.TryValidateProperty(value, context, results))
            {
                yield break;
            }

            foreach (var result in results)
            {
                yield return new FieldValidationError
                                {
                                    FieldName = Name,
                                    FieldValue = value,
                                    ValidationType = result.GetType(),
                                    ErrorMessage = result.ErrorMessage
                                };
            }
        }

        #endregion
    }
}