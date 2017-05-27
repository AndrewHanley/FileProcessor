//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine.Test
//    File Name: DelimitedField.Test.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Test.Fields
{
    using System.Reflection;
    using FileProcessor.Fields;
    using FileProcessor.Fields.FieldAttributes;
    using Xunit;

    public class DelimitedFieldTest
    {
        #region Create and Attribute Value Tests

        [Fact]
        public void CreateDelimitedField()
        {
            var property = typeof(TestDataClass).GetProperty("StringField");
            var field = new DelimitedField(property, '\0', '\0');

            Assert.Equal('\0', field.QuoteCharacter);
        }

        [Fact]
        public void QuoteOverrideField()
        {
            var property = typeof(TestDataClass).GetProperty("QuoteOverrideField");
            var field = new DelimitedField(property, '\0', '\0');

            Assert.Equal('"', field.QuoteCharacter);
        }

        [Fact]
        public void EndQuoteField()
        {
            var property = typeof(TestDataClass).GetProperty("EndQuoteField");
            var field = new DelimitedField(property, '\0', '\0');

            Assert.Equal('[', field.QuoteCharacter);
            Assert.Equal(']', field.EndQuoteCharacter);
        }

        #endregion

        #region Convert To Value Tests

        [Fact]
        public void ToValueNoQuotes()
        {
            var property = typeof(TestDataClass).GetProperty("StringField");
            var field = new DelimitedField(property, '\0', '\0');

            Assert.Equal("No Quotes", field.ConvertToValue("No Quotes"));
        }

        [Fact]
        public void ToValueWithQuotes()
        {
            var property = typeof(TestDataClass).GetProperty("QuoteOverrideField");
            var field = new DelimitedField(property, '\0', '\0');

            Assert.Equal("With \" Quotes", field.ConvertToValue(@"""With "" Quotes"""));
        }

        [Fact]
        public void ToValueWithStartAndEndQuotes()
        {
            var property = typeof(TestDataClass).GetProperty("EndQuoteField");
            var field = new DelimitedField(property, '\0', '\0');

            Assert.Equal("With [Start] & [End] Quotes", field.ConvertToValue(@"[With [[Start]] & [[End]] Quotes]"));
        }

        [Fact]
        public void ToValueInheritedQuotes()
        {
            var property = typeof(TestDataClass).GetProperty("StringField");
            var field = new DelimitedField(property, '"', '\0');

            Assert.Equal("Inherited Quotes", field.ConvertToValue("\"Inherited Quotes\""));
        }

        [Fact]
        public void ToValueInheritedWithStartAndEndQuotes()
        {
            var property = typeof(TestDataClass).GetProperty("StringField");
            var field = new DelimitedField(property, '[', ']');

            Assert.Equal("Inherited Quotes", field.ConvertToValue("[Inherited Quotes]"));
        }

        #endregion

        #region Convert To String Tests

        [Fact]
        public void ToStringNoQuotes()
        {
            var property = typeof(TestDataClass).GetProperty("StringField");
            var field = new DelimitedField(property, '\0', '\0');

            Assert.Equal("No Quotes", field.ConvertToString("No Quotes"));
        }

        [Fact]
        public void ToStringWithQuotes()
        {
            var property = typeof(TestDataClass).GetProperty("QuoteOverrideField");
            var field = new DelimitedField(property, '\0', '\0');

            Assert.Equal("\"With \"\" Quotes\"", field.ConvertToString(@"With "" Quotes"));
        }

        [Fact]
        public void ToStringWithStartAndEndQuotes()
        {
            var property = typeof(TestDataClass).GetProperty("EndQuoteField");
            var field = new DelimitedField(property, '\0', '\0');

            Assert.Equal("[With [[Start]] & [[End]] Quotes]", field.ConvertToString(@"With [Start] & [End] Quotes"));
        }

        [Fact]
        public void ToStringInheritedQuotes()
        {
            var property = typeof(TestDataClass).GetProperty("StringField");
            var field = new DelimitedField(property, '"', '\0');

            Assert.Equal("\"Inherited Quotes\"", field.ConvertToString("Inherited Quotes"));
        }

        [Fact]
        public void ToStringInheritedWithStartAndEndQuotes()
        {
            var property = typeof(TestDataClass).GetProperty("StringField");
            var field = new DelimitedField(property, '[', ']');

            Assert.Equal("[Inherited Quotes]", field.ConvertToString("Inherited Quotes"));
        }

        #endregion

        private class TestDataClass
        {
            public string StringField { get; set; }

            [DelimitedField('"')]
            public string QuoteOverrideField { get; set; }

            [DelimitedField('[', EndQuoteCharacter = ']')]
            public string EndQuoteField { get; set; }
        }
    }
}