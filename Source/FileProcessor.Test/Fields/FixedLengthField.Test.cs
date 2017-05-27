//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine.Test
//    File Name: FixedLengthField.Test.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Test.Fields
{
    using System;
    using System.Reflection;
    using FileProcessor.Fields;
    using FileProcessor.Fields.FieldAttributes;
    using Exceptions;
    using Xunit;

    public class FixedLengthFieldTest
    {
        #region Create and Attribute Tests

        [Fact]
        public void CreateFixedLengthField()
        {
            var property = typeof(TestDataClass).GetProperty("StringField");
            var field = new FixedLengthField(property);

            Assert.Equal(5, field.Length);
            Assert.Equal(false, field.TruncateField);
            Assert.Equal(FieldAlignment.Left, field.FieldAlignment);
            Assert.Equal(' ', field.PaddingCharacter);
        }

        [Fact]
        public void OverrideField()
        {
            var property = typeof(TestDataClass).GetProperty("OverrideField");
            var field = new FixedLengthField(property);

            Assert.Equal(10, field.Length);
            Assert.Equal(true, field.TruncateField);
            Assert.Equal(FieldAlignment.Right, field.FieldAlignment);
            Assert.Equal('A', field.PaddingCharacter);
        }

        #endregion

        #region Convert To Value Tests

        // No tests as FieldBase ConvertToValue is used without any overrides

        #endregion

        #region Convert To String Tests

        [Fact]
        public void NullPaddingField()
        {
            var property = typeof(TestDataClass).GetProperty("NullPaddingField");
            var field = new FixedLengthField(property);

            Assert.Equal("==========", field.ConvertToString(null));
        }

        #region Trunction Tests

        [Fact]
        public void TruncateExceptionField()
        {
            var property = typeof(TestDataClass).GetProperty("TruncateExceptionField");
            var field = new FixedLengthField(property);

            var exception = Assert.Throws<ConversionException>(() => field.ConvertToString("1234567890"));

            Assert.Equal("TruncateExceptionField field failed to convert 1234567890 from String to String. See inner exception for details.", exception.Message);
            Assert.Equal(typeof(OverflowException), exception.InnerException.GetType());
            Assert.Equal("Value is longer than the expected field length.", exception.InnerException.Message);
        }

        [Fact]
        public void TruncateLeftField()
        {
            var property = typeof(TestDataClass).GetProperty("TruncateLeftField");
            var field = new FixedLengthField(property);

            Assert.Equal("12345", field.ConvertToString("1234567890"));
        }

        [Fact]
        public void TruncateRightField()
        {
            var property = typeof(TestDataClass).GetProperty("TruncateRightField");
            var field = new FixedLengthField(property);

            Assert.Equal("67890", field.ConvertToString("1234567890"));
        }

        #endregion

        [Fact]
        public void LeftField()
        {
            var property = typeof(TestDataClass).GetProperty("LeftField");
            var field = new FixedLengthField(property);

            Assert.Equal("12345     ", field.ConvertToString("12345"));
        }

        [Fact]
        public void RightField()
        {
            var property = typeof(TestDataClass).GetProperty("RightField");
            var field = new FixedLengthField(property);

            Assert.Equal("     12345", field.ConvertToString("12345"));
        }

        #endregion

        private class TestDataClass
        {
            [FixedLengthField(5)]
            public string StringField { get; set; }

            public string ExceptionField { get; set; }

            [FixedLengthField(Length = 10, TruncateField = true, FieldAlignment = FieldAlignment.Right, PaddingCharacter = 'A')]
            public string OverrideField { get; set; }

            [FixedLengthField(10, PaddingCharacter = '=')]
            public string NullPaddingField { get; set; }

            [FixedLengthField(5)]
            public string TruncateExceptionField { get; set; }

            [FixedLengthField(5, TruncateField = true)]
            public string TruncateLeftField { get; set; }

            [FixedLengthField(5, TruncateField = true, FieldAlignment = FieldAlignment.Right)]
            public string TruncateRightField { get; set; }

            [FixedLengthField(10)]
            public string LeftField { get; set; }

            [FixedLengthField(10, FieldAlignment = FieldAlignment.Right)]
            public string RightField { get; set; }
        }
    }
}