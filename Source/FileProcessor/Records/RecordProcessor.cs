//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor
//    File Name: RecordProcessor.cs
//  
//    Author:    Andrew - 2017/05/28
//  ---------------------------------------------

namespace FileProcessor.Records
{
    using System;
    using System.Collections.Generic;
    //using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using Exceptions;
    using Fields.Validation;
    using RecordAttributes;
    using RecordElements;
    using Resources;

    public abstract class RecordProcessor<T> where T : class, new()
    {
        #region Properties

        public RecordAttribute RecordAttribute { get; set; }

        public List<RecordElementBase> RecordElements { get; set; }

        public bool RunValidation { get; set; }

        #endregion

        protected RecordProcessor()
        {
            RunValidation = false;
        }

        #region Field Generation/Manipulation

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

                //TODO: Put back once DataAnnotation reference issue is resolved
                //if (property.IsDefined(typeof(ValidationAttribute), true))
                //{
                //    element.IncludesValidation = true;
                //    RunValidation = true;
                //}

                if (IsPropertyEmbeddedClass(property))
                    element.NestedElements = SortElements(ExtractRecordElements(property.PropertyType));

                elements.Add(element);
            }

            return SortElements(elements);
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

        protected bool IsPropertyEmbeddedClass(PropertyInfo property)
        {
            return property.PropertyType.GetTypeInfo().IsClass && property.PropertyType != typeof(string);
        }

        protected IEnumerable<RecordElementBase> FlattenRecordElements(IEnumerable<RecordElementBase> elements)
        {
            foreach (var element in elements)
                if (element.NestedElements != null)
                    foreach (var nestedElement in FlattenRecordElements(element.NestedElements))
                        yield return nestedElement;
                else
                    yield return element;
        }

        #endregion

        #region Write Record

        public string WriteRecord(T entity)
        {
            if (RunValidation)
            {
                var errors = ValidateRecord(entity);

                if (errors != null && errors.Any())
                    throw new RecordValidationException(errors);
            }

            return BuildRecordString(GenerateString(entity, RecordElements));
        }

        private List<string> GenerateString(object entity, List<RecordElementBase> elements)
        {
            var values = new List<string>();

            foreach (var element in elements)
            {
                var value = entity.GetType().GetRuntimeProperty(element.FieldName).GetValue(entity);

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

            entity = PostProcessRecord(entity);

            if (RunValidation)
            {
                var errors = ValidateRecord(entity);

                if (errors != null && errors.Any())
                    throw new RecordValidationException(errors);
            }

            return entity;
        }

        private object ReadElement(object entity, List<RecordElementBase> elements, string record, ref int position)
        {
            foreach (var element in elements)
            {
                var property = entity.GetType().GetRuntimeProperty(element.FieldName);

                if (element.NestedElements != null)
                {
                    var nestedEntity = Activator.CreateInstance(property.PropertyType);

                    property.SetValue(entity, ReadElement(nestedEntity, element.NestedElements, record, ref position));
                }
                else
                {
                    property.SetValue(entity, ExtractField(element, record, ref position));
                }
            }

            return entity;
        }

        #endregion

        #region Validation

        public List<FieldValidationError> ValidateRecord(T entity)
        {
            return ValidateElements(entity, RecordElements).ToList();
        }

        private IEnumerable<FieldValidationError> ValidateElements(object entity, List<RecordElementBase> elements)
        {
            //TODO: Put back once DataAnnotation reference issue is resolved
            //foreach (var element in elements.Where(e => e.IncludesValidation || e.NestedElements != null))
            //{
            //    var validationContext = new ValidationContext(entity) {MemberName = element.FieldName};
            //    var value = entity.GetType().GetRuntimeProperty(element.FieldName).GetValue(entity);

            //    foreach (var error in element.Field.Validate(value, validationContext))
            //        yield return error;

            //    if (element.NestedElements != null)
            //        foreach (var error in ValidateElements(value, element.NestedElements))
            //            yield return error;
            //}
            return null;
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