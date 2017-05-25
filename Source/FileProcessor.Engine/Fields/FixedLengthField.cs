//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: FixedLengthField.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Engine.Fields
{
    using System;
    using System.Reflection;
    using Exceptions;
    using FieldAttributes;
    using Resources;

    public class FixedLengthField : FieldBase
    {
        #region Properties

        public int Length { get; set; }

        public FieldAlignment FieldAlignment { get; set; }

        public char PaddingCharacter { get; set; }

        public bool TruncateField { get; set; }

        #endregion

        public FixedLengthField(PropertyInfo property) : base(property)
        {
            TruncateField = false;
            FieldAlignment = GetDefaultFieldAlignment(property.PropertyType);
            PaddingCharacter = GetDefaultPaddingCharacter(FieldAlignment, property.PropertyType);

            ProcessFixedLengthFieldAttribute(property);
        }

        #region Property Attribute Processing

        private void ProcessFixedLengthFieldAttribute(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<FixedLengthFieldAttribute>();

            if (attribute != null)
            {
                Length = attribute.Length;
                FieldAlignment = attribute.FieldAlignment == FieldAlignment.Default ? FieldAlignment : attribute.FieldAlignment;
                PaddingCharacter = attribute.PaddingCharacter == '\0' ? PaddingCharacter : attribute.PaddingCharacter;
                TruncateField = attribute.TruncateField;
            }
        }

        private FieldAlignment GetDefaultFieldAlignment(Type propertyType)
        {
            var t = propertyType;
            t = Nullable.GetUnderlyingType(t) ?? t;

            if (t.GetTypeInfo().IsPrimitive
                && propertyType != typeof(bool)
                && propertyType != typeof(char))
                return FieldAlignment.Right;

            return FieldAlignment.Left;
        }

        private char GetDefaultPaddingCharacter(FieldAlignment alignment, Type propertyType)
        {
            if (alignment == FieldAlignment.Left)
                return ' ';

            var t = propertyType;
            t = Nullable.GetUnderlyingType(propertyType) ?? t;

            if (t.GetTypeInfo().IsPrimitive
                && propertyType != typeof(bool)
                && propertyType != typeof(char))
                return '0';

            return ' ';
        }

        #endregion

        public override string ConvertToString(object value)
        {
            var result = base.ConvertToString(value);

            if (string.IsNullOrEmpty(result))
                return string.Empty.PadLeft(Length, PaddingCharacter);

            if (result.Length > Length && !TruncateField)
                throw new ConversionException(Name, value, FieldType, typeof(string),
                                              new OverflowException(ExceptionMessages.Overflow));

            switch (FieldAlignment)
            {
                case FieldAlignment.Left:
                    return ConvertToLeftAlignedString(result);

                case FieldAlignment.Right:
                    return ConvertToRightAlignedString(result);

                default:
                    throw new ConversionException(Name, value, FieldType, typeof(string),
                                                  new AttributeException(ExceptionMessages.InvalidFieldAlignment, FieldAlignment));
            }
        }

        private string ConvertToLeftAlignedString(string value)
        {
            if (value.Length > Length)
                value = value.Substring(0, Length);

            return value.PadRight(Length, PaddingCharacter);
        }

        private string ConvertToRightAlignedString(string value)
        {
            if (value.Length > Length)
                value = value.Substring(value.Length - Length);

            return value.PadLeft(Length, PaddingCharacter);
        }
    }

    public enum FieldAlignment
    {
        Default,
        Left,
        Right
    }
}