﻿//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine.Test
//    File Name: RecordBase.Test.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Test.Records
{
    using FileProcessor.Fields.FieldAttributes;
    using FileProcessor.Records;
    using FileProcessor.Records.RecordAttributes;
    using Exceptions;
    using Xunit;

    public class RecordTest
    {
        #region Create Tests

        [Fact]
        public void NoAttributeRecord()
        {
            var exception = Assert.Throws<AttributeException>(() => RecordBuilder.CreateProcessor<BadDataClass>());

            Assert.Equal("The BadDataClass record requires a RecordAttribute to be defined.", exception.Message);
        }

        [Fact]
        public void UnexpectedTypeRecord()
        {
            var exception = Assert.Throws<AttributeException>(() => RecordBuilder.CreateProcessor<InvalidTypeClass>());

            Assert.Equal("Processing for record type TestRecAttrAttribute has not been implemented.", exception.Message);
        }

        #endregion

        #region Fixed Length Field Generation Tests

        [Fact]
        public void CreateFixedLengthRecord()
        {
            var record = RecordBuilder.CreateProcessor<FixedLengthClass>();

            Assert.Equal(typeof(FixedLengthRecordProcessor<FixedLengthClass>), record.GetType());
        }

        [Fact]
        public void FixedLengthFields()
        {
            var record = RecordBuilder.CreateProcessor<FixedLengthClass>();

            Assert.Equal("StringField", record.RecordElements[0].Field.Name);
            Assert.Equal("IntField", record.RecordElements[1].Field.Name);
        }

        [Fact]
        public void FixedLengthNestedFields()
        {
            var record = RecordBuilder.CreateProcessor<FixedLengthNestedClass>();

            Assert.Equal("StringField", record.RecordElements[0].Field.Name);
            Assert.Equal("IntField", record.RecordElements[1].Field.Name);
            Assert.Equal("Embedded", record.RecordElements[2].Field.Name);
            Assert.Equal("EmbeddedField", record.RecordElements[2].NestedElements[0].Field.Name);
        }

        [Fact]
        public void InheritedFields()
        {
            var record = RecordBuilder.CreateProcessor<FixedLengthInheritedClass>();

            Assert.Equal("StringField", record.RecordElements[0].Field.Name);
            Assert.Equal("IntField", record.RecordElements[1].Field.Name);
            Assert.Equal("InheritAddedField", record.RecordElements[2].Field.Name);
        }

        [Fact]
        public void FixedLengthException()
        {
            var exception = Assert.Throws<AttributeException>(() => RecordBuilder.CreateProcessor<FixedLengthExceptionClass>());

            Assert.Equal("The IntField property requires a FixedLengthAttribute with a Length definited.", exception.Message);
        }

        [Fact]
        public void FixedLengthNestedException()
        {
            var exception = Assert.Throws<AttributeException>(() => RecordBuilder.CreateProcessor<FixedLengthNestedExceptionClass>());

            Assert.Equal("The EmbeddedField property requires a FixedLengthAttribute with a Length definited.", exception.Message);
        }

        #endregion

        #region Delimited Field Generation Tests

        [Fact]
        public void CreateDelimitedRecord()
        {
            var record = RecordBuilder.CreateProcessor<DelimitedClass>();

            Assert.Equal(typeof(DelimitedRecordProcessor<DelimitedClass>), record.GetType());
            Assert.Equal(";", ((DelimitedRecordAttribute) record.RecordAttribute).Delimiter);
            Assert.Equal('[', ((DelimitedRecordAttribute) record.RecordAttribute).QuoteCharacter);
            Assert.Equal(']', ((DelimitedRecordAttribute) record.RecordAttribute).EndQuoteCharacter);
        }

        [Fact]
        public void NoDelimiterException()
        {
            var exception = Assert.Throws<AttributeException>(() => RecordBuilder.CreateProcessor<NoDelimiterClass>());

            Assert.Equal("NoDelimiterClass has a DelimiterRecordAttribute without a specified Delimiter. A Delimiter is required for file processing.", exception.Message);
        }

        [Fact]
        public void DelimitedFields()
        {
            var record = RecordBuilder.CreateProcessor<DelimitedDataClass>();

            Assert.Equal("StringField", record.RecordElements[0].Field.Name);
            Assert.Equal("IntField", record.RecordElements[1].Field.Name);
        }

        [Fact]
        public void DelimitedNestedFields()
        {
            var record = RecordBuilder.CreateProcessor<DelimitedNestedClass>();

            Assert.Equal("StringField", record.RecordElements[0].Field.Name);
            Assert.Equal("IntField", record.RecordElements[1].Field.Name);
            Assert.Equal("Embedded", record.RecordElements[2].Field.Name);
            Assert.Equal("EmbeddedField", record.RecordElements[2].NestedElements[0].Field.Name);
        }

        [Fact]
        public void DelimitedOrderedFields()
        {
            var record = RecordBuilder.CreateProcessor<OrderedDelimitedClass>();

            Assert.Equal("IntField", record.RecordElements[0].Field.Name);
            Assert.Equal("StringField", record.RecordElements[1].Field.Name);
        }

        #endregion

        #region Ordered Fields Tests

        [Fact]
        public void OrderedFields()
        {
            var record = RecordBuilder.CreateProcessor<OrderedTestClass>();

            Assert.Equal("IntField", record.RecordElements[0].Field.Name);
            Assert.Equal("StringField", record.RecordElements[1].Field.Name);
        }

        [Fact]
        public void OrderedNestedFields()
        {
            var record = RecordBuilder.CreateProcessor<OrderedNestedTestClass>();

            Assert.Equal("IntField", record.RecordElements[0].Field.Name);
            Assert.Equal("StringField", record.RecordElements[1].Field.Name);
            Assert.Equal("Embedded1Field", record.RecordElements[2].NestedElements[0].Field.Name);
            Assert.Equal("Embedded2Field", record.RecordElements[2].NestedElements[1].Field.Name);
        }

        #endregion

        #region Creation Test Classes

        [FixedLengthRecord]
        private class CreateTestClass
        {
        }

        private class BadDataClass
        {
        }

        private class TestRecAttrAttribute : RecordAttribute
        {
        }

        [TestRecAttr]
        private class InvalidTypeClass
        {
        }

        #endregion

        #region Fixed Length Test Classes

        [FixedLengthRecord]
        private class FixedLengthClass
        {
            [FixedLengthField(10)]
            public string StringField { get; set; }

            [FixedLengthField(5)]
            public int IntField { get; set; }
        }

        [FixedLengthRecord]
        private class FixedLengthExceptionClass
        {
            [FixedLengthField(10)]
            public string StringField { get; set; }

            public int IntField { get; set; }
        }

        [FixedLengthRecord]
        private class FixedLengthNestedClass
        {
            [FixedLengthField(10)]
            public string StringField { get; set; }

            [FixedLengthField(5)]
            public int IntField { get; set; }

            public FixedLengthEmbeddedClass Embedded { get; set; }
        }

        private class FixedLengthEmbeddedClass
        {
            [FixedLengthField(10)]
            public string EmbeddedField { get; set; }
        }

        [FixedLengthRecord]
        private class FixedLengthNestedExceptionClass
        {
            [FixedLengthField(10)]
            public string StringField { get; set; }

            [FixedLengthField(5)]
            public int IntField { get; set; }

            public FixedLengthEmbeddedExceptionClass Embedded { get; set; }
        }

        private class FixedLengthEmbeddedExceptionClass
        {
            public string EmbeddedField { get; set; }
        }

        private class FixedLengthInheritedClass : FixedLengthClass
        {
            [FixedLengthField(5)]
            public string InheritAddedField { get; set; }
        }

        #endregion

        #region Delimited Test Classes

        [DelimitedRecord(Delimiter = ";", QuoteCharacter = '[', EndQuoteCharacter = ']')]
        private class DelimitedClass
        {
        }

        [DelimitedRecord(Delimiter = ";")]
        private class DelimitedDataClass
        {
            public string StringField { get; set; }

            public int IntField { get; set; }
        }

        [DelimitedRecord(Delimiter = ";")]
        private class DelimitedNestedClass
        {
            public string StringField { get; set; }

            public int IntField { get; set; }

            public DelimitedEmbeddedClass Embedded { get; set; }
        }

        private class DelimitedEmbeddedClass
        {
            public string EmbeddedField { get; set; }
        }

        [DelimitedRecord(Delimiter = ";")]
        private class OrderedDelimitedClass
        {
            [DelimitedField(Order = 2)]
            public string StringField { get; set; }

            [DelimitedField(Order = 1)]
            public int IntField { get; set; }
        }

        [DelimitedRecord]
        private class NoDelimiterClass
        {
        }

        #endregion

        #region Ordered Properties Test Classes

        [FixedLengthRecord]
        private class OrderedTestClass
        {
            [FixedLengthField(10, Order = 2)]
            public string StringField { get; set; }

            [FixedLengthField(5, Order = 1)]
            public int IntField { get; set; }
        }

        [FixedLengthRecord]
        private class OrderedNestedTestClass
        {
            [FixedLengthField(10, Order = 2)]
            public string StringField { get; set; }

            [FixedLengthField(5, Order = 1)]
            public int IntField { get; set; }

            [FixedLengthField(Order = 3)]
            public EmbeddedOrderedClass Embedded { get; set; }
        }

        private class EmbeddedOrderedClass
        {
            [FixedLengthField(10, Order = 2)]
            public string Embedded2Field { get; set; }

            [FixedLengthField(5, Order = 1)]
            public int Embedded1Field { get; set; }
        }

        #endregion
    }
}