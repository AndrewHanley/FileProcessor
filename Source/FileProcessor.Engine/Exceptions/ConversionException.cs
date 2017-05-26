//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: ConversionException.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Exceptions
{
    using System;
    using Resources;

    public class ConversionException : Exception
    {
        #region Properties

        public string FieldName { get; set; }
        public object FieldValue { get; set; }
        public Type SourceType { get; set; }
        public Type DestinationType { get; set; }
        public string FormatString { get; set; }

        #endregion

        public ConversionException(string fieldName, object fieldValue, Type sourceType, Type destinationType, string formatString = null)
            : this(fieldName, fieldValue, sourceType, destinationType, formatString, null)
        {
        }

        public ConversionException(string fieldName, object fieldValue, Type sourceType, Type destinationType, Exception innerException)
            : this(fieldName, fieldValue, sourceType, destinationType, null, innerException)
        {
        }

        public ConversionException(string fieldName, object fieldValue, Type sourceType, Type destinationType, string formatString, Exception innerException)
            : base(GenerateMessage(fieldName, fieldValue, sourceType, destinationType, formatString), innerException)
        {
            FieldName = fieldName;
            FieldValue = fieldValue;
            SourceType = sourceType;
            DestinationType = destinationType;
            FormatString = formatString;
        }

        public ConversionException(string message)
            : base(message)
        {
        }

        public ConversionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private static string GenerateMessage(string fieldName, object fieldValue, Type sourceType, Type destinationType, string formatString = null)
        {
            if (formatString == null)
                return string.Format(ExceptionMessages.ConversionException, fieldName, fieldValue, sourceType.Name, destinationType.Name);

            return string.Format(ExceptionMessages.ConversionExceptionWithFormatString, fieldName, fieldValue, sourceType.Name, destinationType.Name, formatString);
        }
    }
}