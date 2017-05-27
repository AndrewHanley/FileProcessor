//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor
//    File Name: RecordProcessor.cs
//  
//    Author:    Andrew - 2017/05/26
//  ---------------------------------------------

namespace FileProcessor.Records
{
    using System.Reflection;
    using Exceptions;
    using RecordAttributes;
    using Resources;

    public sealed class RecordProcessor
    {
        public static Record<T> CreateRecord<T>() where T : class, new()
        {
            var recordAttribute = typeof(T).GetTypeInfo().GetCustomAttribute<RecordAttribute>();
            Record<T> record = null;

            if (recordAttribute == null)
                throw new AttributeException(ExceptionMessages.RecordAttributeMissing, typeof(T).Name);

            var delimitedRecordAttributeattribute = recordAttribute as DelimitedRecordAttribute;
            if (delimitedRecordAttributeattribute != null)
                record = new DelimitedRecord<T>(delimitedRecordAttributeattribute);

            var fixedLengthRecordAttribute = recordAttribute as FixedLengthRecordAttribute;
            if (fixedLengthRecordAttribute != null)
                record = new FixedLengthRecord<T>(fixedLengthRecordAttribute);

            if (record == null)
                throw new AttributeException(ExceptionMessages.UnexpectedRecordType, recordAttribute.GetType().Name);

            record.GenerateFields();

            return record;
        }
    }
}