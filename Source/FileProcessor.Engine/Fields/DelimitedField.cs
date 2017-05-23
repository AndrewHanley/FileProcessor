//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: DelimitedField.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Engine.Fields
{
    using System.Reflection;
    using FieldAttributes;

    public class DelimitedField : FieldBase
    {
        #region Properties

        public char QuoteCharacter { get; set; }

        public char EndQuoteCharacter { get; set; }

        #endregion

        public DelimitedField(PropertyInfo property, int order) : base(property, order)
        {
            QuoteCharacter = '\0';
            EndQuoteCharacter = '\0';

            ProcessDelimitedFieldAttribute(property);
        }

        #region Property Attribute Processing

        private void ProcessDelimitedFieldAttribute(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<DelimitedFieldAttribute>();

            if (attribute != null)
            {
                QuoteCharacter = attribute.QuoteCharacter == '\0' ? QuoteCharacter : attribute.QuoteCharacter;
                EndQuoteCharacter = attribute.EndQuoteCharacter == '\0' ? QuoteCharacter : attribute.EndQuoteCharacter;
            }
        }

        #endregion

        public override object ConvertToValue(string value)
        {
            if (QuoteCharacter != '\0')
            {
                var quoteString = QuoteCharacter.ToString();
                var endQuoteString = EndQuoteCharacter.ToString();

                if (value.StartsWith(quoteString))
                    value = value.Remove(0, 1);

                if (value.EndsWith(endQuoteString))
                    value = value.Remove(value.Length - 1);

                value = value.Replace($"{QuoteCharacter}{QuoteCharacter}", quoteString);
                value = value.Replace($"{EndQuoteCharacter}{EndQuoteCharacter}", endQuoteString);
            }

            return base.ConvertToValue(value);
        }

        public override string ConvertToString(object value)
        {
            var result = base.ConvertToString(value);

            if (QuoteCharacter != '\0')
            {
                result = result.Replace(QuoteCharacter.ToString(), $"{QuoteCharacter}{QuoteCharacter}");

                if (!QuoteCharacter.Equals(EndQuoteCharacter))
                    result = result.Replace(EndQuoteCharacter.ToString(), $"{EndQuoteCharacter}{EndQuoteCharacter}");

                return $"{QuoteCharacter}{result}{EndQuoteCharacter}";
            }

            return result;
        }
    }
}