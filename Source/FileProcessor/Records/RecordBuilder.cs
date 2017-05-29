//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor
//    File Name: RecordBuilder.cs
//  
//    Author:    Andrew - 2017/05/28
//  ---------------------------------------------

namespace FileProcessor.Records
{
    using System.Reflection;
    using Exceptions;
    using RecordAttributes;
    using Resources;

    public sealed class RecordBuilder
    {
        public static RecordProcessor<T> CreateProcessor<T>() where T : class, new()
        {
            var recordAttribute = typeof(T).GetTypeInfo().GetCustomAttribute<RecordAttribute>();
            RecordProcessor<T> record = null;

            if (recordAttribute == null)
                throw new AttributeException(ExceptionMessages.RecordAttributeMissing, typeof(T).Name);

            var delimitedRecordAttributeattribute = recordAttribute as DelimitedRecordAttribute;
            if (delimitedRecordAttributeattribute != null)
                record = new DelimitedRecordProcessor<T>(delimitedRecordAttributeattribute);

            var fixedLengthRecordAttribute = recordAttribute as FixedLengthRecordAttribute;
            if (fixedLengthRecordAttribute != null)
                record = new FixedLengthRecordProcessor<T>(fixedLengthRecordAttribute);

            if (record == null)
                throw new AttributeException(ExceptionMessages.UnexpectedRecordType, recordAttribute.GetType().Name);

            record.GenerateFields();

            return record;
        }
    }
}