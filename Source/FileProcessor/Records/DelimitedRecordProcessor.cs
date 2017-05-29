//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor
//    File Name: DelimitedRecordProcessor.cs
//  
//    Author:    Andrew - 2017/05/28
//  ---------------------------------------------

namespace FileProcessor.Records
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Exceptions;
    using Fields;
    using Fields.FieldAttributes;
    using RecordAttributes;
    using RecordElements;
    using Resources;

    public class DelimitedRecordProcessor<T> : RecordProcessor<T> where T : class, new()
    {
        #region Properties

        public new DelimitedRecordAttribute RecordAttribute
        {
            get => (DelimitedRecordAttribute) base.RecordAttribute;
            set => base.RecordAttribute = value;
        }

        private List<string> RecordValues { get; set; }

        #endregion

        public DelimitedRecordProcessor(DelimitedRecordAttribute recordAttribute)
        {
            RecordAttribute = recordAttribute;

            if (string.IsNullOrEmpty(RecordAttribute.Delimiter))
                throw new AttributeException(ExceptionMessages.MissingDelimiter, typeof(T).Name);
        }

        protected override RecordElementBase CreateRecordElement(PropertyInfo property)
        {
            var fieldAttribute = property.GetCustomAttribute<DelimitedFieldAttribute>();
            var field = new DelimitedField(property, RecordAttribute.QuoteCharacter, RecordAttribute.EndQuoteCharacter);

            return new DelimitedRecordElement
                   {
                       Field = field,
                       Order = fieldAttribute?.Order ?? -1,
                       QuoteCharacter = field.QuoteCharacter,
                       EndQuoteCharacter = field.EndQuoteCharacter
                   };
        }

        protected override string BuildRecordString(List<string> values)
        {
            return string.Join(RecordAttribute.Delimiter, values);
        }

        protected override string PreProcessRecord(string record)
        {
            var fields = FlattenRecordElements(RecordElements).Select(e => (DelimitedField) e.Field).ToList();

            if (RecordAttribute.QuoteCharacter == '\0' && fields.All(f => f.QuoteCharacter == '\0'))
            {
                RecordValues = record.Split(new[] {RecordAttribute.Delimiter}, StringSplitOptions.None).ToList();
                return record;
            }

            RecordValues = new List<string>();

            var quoteIndex = -1;
            var fieldIndex = 0;
            var startFieldIndex = 0;
            var index = 0;

            while (index < record.Length)
            {
                if (index < record.Length - 1 &&
                    (record[index] == fields[fieldIndex].QuoteCharacter && record[index + 1] == fields[fieldIndex].QuoteCharacter ||
                     record[index] == fields[fieldIndex].EndQuoteCharacter && record[index + 1] == fields[fieldIndex].EndQuoteCharacter))
                {
                    index += 2;
                    continue;
                }

                if (record[index] == fields[fieldIndex].QuoteCharacter && quoteIndex == -1)
                    quoteIndex = index;

                if (record[index] == fields[fieldIndex].EndQuoteCharacter && index != quoteIndex)
                    quoteIndex = -1;

                if (record.Substring(index, RecordAttribute.Delimiter.Length) == RecordAttribute.Delimiter)
                    if (quoteIndex == -1)
                    {
                        RecordValues.Add(record.Substring(startFieldIndex, index - startFieldIndex));
                        startFieldIndex = index + RecordAttribute.Delimiter.Length;
                        fieldIndex++;

                        // If there are no more Record Element fields to populate stop parsing record
                        if (fieldIndex >= fields.Count)
                            break;
                    }

                index++;
            }

            if (startFieldIndex < record.Length)
                RecordValues.Add(record.Substring(startFieldIndex));

            return record;
        }

        protected override object ExtractField(RecordElementBase element, string record, ref int position)
        {
            return element.Field.ConvertToValue(RecordValues[position++]);
        }
    }
}