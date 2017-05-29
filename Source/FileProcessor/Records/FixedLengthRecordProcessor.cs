//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor
//    File Name: FixedLengthRecordProcessor.cs
//  
//    Author:    Andrew - 2017/05/28
//  ---------------------------------------------

namespace FileProcessor.Records
{
    using System.Collections.Generic;
    using System.Reflection;
    using Exceptions;
    using Fields;
    using Fields.FieldAttributes;
    using RecordAttributes;
    using RecordElements;
    using Resources;

    public class FixedLengthRecordProcessor<T> : RecordProcessor<T> where T : class, new()
    {
        #region Properties

        public new FixedLengthRecordAttribute RecordAttribute
        {
            get => (FixedLengthRecordAttribute) base.RecordAttribute;
            set => base.RecordAttribute = value;
        }

        #endregion

        public FixedLengthRecordProcessor(FixedLengthRecordAttribute recordAttribute)
        {
            RecordAttribute = recordAttribute;
        }

        protected override RecordElementBase CreateRecordElement(PropertyInfo property)
        {
            var fieldAttribute = property.GetCustomAttribute<FixedLengthFieldAttribute>();

            if (!IsPropertyEmbeddedClass(property) && (fieldAttribute == null || fieldAttribute.Length <= 0))
                throw new AttributeException(ExceptionMessages.FixedLengthMissing, property.Name);

            return new FixedLengthRecordElement
                   {
                       Field = new FixedLengthField(property),
                       Order = fieldAttribute?.Order ?? -1,
                       Length = fieldAttribute?.Length ?? -1
                   };
        }

        protected override string BuildRecordString(List<string> values)
        {
            return string.Join(string.Empty, values);
        }

        protected override object ExtractField(RecordElementBase element, string record, ref int position)
        {
            var field = (FixedLengthField) element.Field;
            var startIndex = position;

            position += field.Length;

            return field.ConvertToValue(record.Substring(startIndex, field.Length));
        }
    }
}