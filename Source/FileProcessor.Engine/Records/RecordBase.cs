//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: RecordBase.cs
//  
//    Author:    Andrew - 2017/05/24
//  ---------------------------------------------

namespace FileProcessor.Engine.Records
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

    public abstract class RecordBase<T> where T : class
    {
        #region Properties

        public RecordType RecordType { get; set; }

        public RecordAttribute RecordAttribute { get; set; }

        public List<RecordElementBase> RecordElements { get; set; }

        #endregion

        protected RecordBase()
        {
            ProcessRecordAttribute(typeof(T).GetTypeInfo().GetCustomAttribute<RecordAttribute>());
            GenerateFields();
        }

        protected RecordBase(List<RecordElementBase> recordElements, RecordAttribute recordAttribute)
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

                if (string.IsNullOrEmpty(((DelimitedRecordAttribute) attribute).Delimiter))
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
        }

        private List<RecordElementBase> ExtractRecordElements(Type entity)
        {
            var elements = new List<RecordElementBase>();

            if (entity.GetTypeInfo().BaseType != null)
                elements.AddRange(ExtractRecordElements(entity.GetTypeInfo().BaseType));

            // Inherited Classes will define their properties first and then their parent properties
            // to assist in maintaining declared order of parent -> child... we limit the property
            // retrieval to only enity declared and the code above will process parents first
            foreach (var property in entity.GetTypeInfo().DeclaredProperties.Where(p => p.CanRead && p.CanWrite))
            {
                RecordElementBase element;

                switch (RecordType)
                {
                    case RecordType.Delimited:
                        element = CreateDelimitedElement(property);
                        break;

                    case RecordType.FixedLength:
                        element = CreateFixedLengthElement(property);
                        break;

                    default:
                        return null;
                }

                if (IsPropertyEmbeddedClass(property))
                    element.NestedElements = SortElements(ExtractRecordElements(property.PropertyType));

                elements.Add(element);
            }

            return SortElements(elements);
        }

        private RecordElementBase CreateDelimitedElement(PropertyInfo property)
        {
            var fieldAttribute = property.GetCustomAttribute<DelimitedFieldAttribute>();
            var recAttribute = (DelimitedRecordAttribute) RecordAttribute;
            var field = new DelimitedField(property, recAttribute.QuoteCharacter, recAttribute.EndQuoteCharacter);

            return new DelimitedRecordElement
                   {
                       Field = field,
                       Order = fieldAttribute?.Order ?? -1,
                       QuoteCharacter = field.QuoteCharacter,
                       EndQuoteCharacter = field.EndQuoteCharacter
                   };
        }

        private RecordElementBase CreateFixedLengthElement(PropertyInfo property)
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

        private bool IsPropertyEmbeddedClass(PropertyInfo property)
        {
            return property.PropertyType.GetTypeInfo().IsClass && property.PropertyType != typeof(string);
        }

        private List<RecordElementBase> SortElements(List<RecordElementBase> elements)
        {
            if (elements.Any(e => e.Order != -1))
            {
                if (elements.All(e => e.Order != -1))
                    return elements.OrderBy(e => e.Order).ToList();

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