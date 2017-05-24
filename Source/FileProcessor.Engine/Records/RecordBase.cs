using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessor.Engine.Records
{
    using System.Linq;
    using System.Reflection;
    using Exceptions;
    using Fields;
    using Fields.FieldAttributes;
    using RecordAttributes;
    using Resources;

    public abstract class RecordBase<T> where T : class
    {
        #region Properties

        public RecordType RecordType { get; set; }

        public RecordAttribute RecordAttribute { get; set; }

        public List<RecordElement> RecordElements { get; set; }

        #endregion

        protected RecordBase()
        {
            ProcessRecordAttribute(typeof(T).GetTypeInfo().GetCustomAttribute<RecordAttribute>());
            GenerateFields();
        }

        protected RecordBase(List<RecordElement> recordElements, RecordAttribute recordAttribute)
        {
            ProcessRecordAttribute(recordAttribute);
            RecordElements = recordElements;
        }

        #region Process Attributes

        private void ProcessRecordAttribute(RecordAttribute attribute)
        {
            RecordAttribute = attribute ?? throw new AttributeException(ExceptionMessages.RecordAttributeMissing, typeof(T).Name);

            if (attribute is DelimitedRecordAttribute)
            {
                RecordType = RecordType.Delimited;

                if (((DelimitedRecordAttribute)attribute).Delimiter == '\0')
                    throw new AttributeException(ExceptionMessages.MissingDelimiter, typeof(T).Name);
            }
            else if (attribute is FixedLengthRecordAttribute)
            {
                RecordType = RecordType.FixedLength;
            }
            else
            {
                throw new AttributeException(ExceptionMessages.UnexpectedRecordType, attribute.GetType().Name);
            }
        }

        #endregion

        #region Field Generation

        private void GenerateFields()
        {
            RecordElements = ExtractRecordElements(typeof(T));

            if (RecordType == RecordType.FixedLength)
                VerifyLengthProperty(RecordElements);

            CreateFields(RecordElements);
        }

        private void VerifyLengthProperty(List<RecordElement> elements)
        {
            foreach (var element in elements)
            {
                if (element.NestedElements != null)
                {
                    VerifyLengthProperty(element.NestedElements);
                }
                else
                {
                    var attribute = element.PropertyInfo.GetCustomAttribute<FixedLengthFieldAttribute>();

                    if (attribute == null || attribute.Length <= 0)
                        throw new AttributeException(ExceptionMessages.FixedLengthMissing, element.PropertyInfo.Name);
                }
            }
        }

        private void CreateFields(List<RecordElement> elements)
        {
            foreach (var element in elements)
            {
                if (element.NestedElements != null)
                {
                    CreateFields(element.NestedElements);
                }
                else if (RecordType == RecordType.Delimited)
                {
                    element.Field = new DelimitedField(element.PropertyInfo);
                }
                else
                {
                    element.Field = new FixedLengthField(element.PropertyInfo);
                }
            }
        }

        private List<RecordElement> ExtractRecordElements(Type entity)
        {
            var elements = new List<RecordElement>();

            if (entity.GetTypeInfo().BaseType != null)
            {
                elements.AddRange(ExtractRecordElements(entity.GetTypeInfo().BaseType));
            }

            // Inherited Classes will define their properties first and then their parent properties
            // to assist in maintaining declared order of parent -> child... we limit the property
            // retrieval to only enity declared and the code above will process parents first
            foreach (var property in entity.GetTypeInfo().DeclaredProperties.Where(p => p.CanRead && p.CanWrite))
            {
                var element = new RecordElement {PropertyInfo = property};

                if (property.PropertyType.GetTypeInfo().IsClass && property.PropertyType != typeof(string))
                {
                    element.NestedElements = SortElements(ExtractRecordElements(property.PropertyType));
                }

                elements.Add(element);
            }

            return SortElements(elements);
        }

        private List<RecordElement> SortElements(List<RecordElement> elements)
        {
            bool ordered = elements.Where(e => e.PropertyInfo.IsDefined(typeof(FieldAttribute)))
                                   .Any(e => e.PropertyInfo.GetCustomAttribute<FieldAttribute>().Order != -1);

            if (ordered)
            {
                if (elements.Any(e => !e.PropertyInfo.IsDefined(typeof(FieldAttribute))))
                    throw new AttributeException(ExceptionMessages.InvalidFieldOrderValue, typeof(T).Name);

                if (elements.All(e => e.PropertyInfo.GetCustomAttribute<FieldAttribute>().Order != -1))
                {
                    return elements.OrderBy(e => e.PropertyInfo.GetCustomAttribute<FieldAttribute>().Order).ToList();
                }

                throw new AttributeException(ExceptionMessages.InvalidFieldOrderValue, typeof(T).Name);
            }

            return elements;
        }

        #endregion
    }

    public enum RecordType
    {
        FixedLength,
        Delimited
    }
}
