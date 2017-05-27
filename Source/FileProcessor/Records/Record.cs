//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor
//    File Name: Record.cs
//  
//    Author:    Andrew - 2017/05/26
//  ---------------------------------------------

namespace FileProcessor.Records
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Exceptions;
    using RecordAttributes;
    using RecordElements;
    using Resources;

    public abstract class Record<T> where T : class, new()
    {
        #region Properties

        public RecordAttribute RecordAttribute { get; set; }

        public List<RecordElementBase> RecordElements { get; set; }

        #endregion

        #region Field Generation

        public void GenerateFields()
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
                var element = CreateRecordElement(property);

                element.FieldName = property.Name;

                if (IsPropertyEmbeddedClass(property))
                    element.NestedElements = SortElements(ExtractRecordElements(property.PropertyType));

                elements.Add(element);
            }

            return SortElements(elements);
        }

        protected bool IsPropertyEmbeddedClass(PropertyInfo property)
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

        #region Write Record

        public string WriteRecord(T entity)
        {
            return BuildRecordString(GenerateString(entity, RecordElements));
        }

        private List<string> GenerateString(object entity, List<RecordElementBase> elements)
        {
            var values = new List<string>();

            foreach (var element in elements)
            {
                var value = entity.GetType().GetProperty(element.FieldName).GetValue(entity);

                if (element.NestedElements != null)
                {
                    if (value == null)
                        continue;

                    values.AddRange(GenerateString(value, element.NestedElements));
                }
                else
                {
                    values.Add(element.Field.ConvertToString(value));
                }
            }

            return values;
        }

        #endregion

        #region Read Record

        public T ReadRecord(string record)
        {
            var position = 0;

            record = PreProcessRecord(record);

            var entity = (T) ReadElement(new T(), RecordElements, record, ref position);

            return PostProcessRecord(entity);
        }

        private object ReadElement(object entity, List<RecordElementBase> elements, string record, ref int position)
        {
            foreach (var element in elements)
            {
                var property = entity.GetType().GetProperty(element.FieldName);

                if (element.NestedElements != null)
                {
                    var nestedEntity = Activator.CreateInstance(property.PropertyType);

                    ReadElement(nestedEntity, element.NestedElements, record, ref position);
                }

                property.SetValue(entity, ExtractField(element, record, ref position));
            }

            return entity;
        }

        #endregion

        #region Abstract Methods

        protected abstract RecordElementBase CreateRecordElement(PropertyInfo property);
        protected abstract string BuildRecordString(List<string> values);
        protected abstract object ExtractField(RecordElementBase element, string record, ref int position);

        protected virtual string PreProcessRecord(string record)
        {
            return record;
        }

        protected virtual T PostProcessRecord(T entity)
        {
            return entity;
        }

        #endregion
    }

    public enum RecordType
    {
        FixedLength,
        Delimited
    }
}