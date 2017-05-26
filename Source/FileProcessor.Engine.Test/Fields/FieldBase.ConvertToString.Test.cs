//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine.Test
//    File Name: FieldBase.ConvertToString.Test.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Test.Fields
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using FileProcessor.Fields;
    using FileProcessor.Fields.Converters;
    using FileProcessor.Fields.FieldAttributes;
    using FileProcessor.Fields.FieldAttributes.FormatAttributes;
    using Xunit;

    public class FieldBaseConvertToStringTest
    {
        #region Null Field Tests

        [Fact]
        public void NUllField()
        {
            var property = typeof(TestDataClass).GetProperty("StringField");
            var field = new TestField(property);

            Assert.Equal(null, field.ConvertToString(null));
        }

        #endregion

        #region String Field Tests

        [Theory]
        [InlineData("Test", "Test")]
        [InlineData(" Test", "Test")]
        [InlineData("Test ", "Test")]
        [InlineData(" Test ", "Test")]
        public void StringField(string value, string expectedResult)
        {
            var property = typeof(TestDataClass).GetProperty("StringField");
            var field = new TestField(property);

            Assert.Equal(expectedResult, field.ConvertToString(value));
        }

        #endregion

        #region Boolean Field Tests

        [Theory]
        [InlineData(true, "True")]
        [InlineData(false, "False")]
        public void BooleanField(bool value, string expectedResult)
        {
            var property = typeof(TestDataClass).GetProperty("BooleanField");
            var field = new TestField(property);

            Assert.Equal(expectedResult, field.ConvertToString(value));
        }

        [Theory]
        [InlineData(true, "On")]
        [InlineData(false, "Off")]
        public void OnOffField(bool value, string expectedResult)
        {
            var property = typeof(TestDataClass).GetProperty("OnOffField");
            var field = new TestField(property);

            Assert.Equal(expectedResult, field.ConvertToString(value));
        }

        #endregion

        #region Date\Time Tests

        [Fact]
        public void DateTimeField()
        {
            var property = typeof(TestDataClass).GetProperty("DateTimeField");
            var field = new TestField(property);
            var dateTime = DateTime.Now;

            Assert.Equal(dateTime.ToString(CultureInfo.CurrentCulture), field.ConvertToString(dateTime));
        }

        [Fact]
        public void Date_yyyyMMdd_Field()
        {
            var property = typeof(TestDataClass).GetProperty("Date_yyyyMMdd_Field");
            var field = new TestField(property);
            var dateTime = new DateTime(1994, 7, 23, 16, 24, 15);

            Assert.Equal("19940723", field.ConvertToString(dateTime));
        }

        [Fact]
        public void DateTime_MMddyyyyHHmm_Field()
        {
            var property = typeof(TestDataClass).GetProperty("DateTime_MMddyyyyHHmm_Field");
            var field = new TestField(property);
            var dateTime = new DateTime(1994, 7, 23, 16, 24, 15);

            Assert.Equal("072319941624", field.ConvertToString(dateTime));
        }

        [Fact]
        public void Time_h_mm_ss_tt_Field()
        {
            var property = typeof(TestDataClass).GetProperty("Time_h_mm_ss_tt_Field");
            var field = new TestField(property);
            var dateTime = new DateTime(1994, 7, 23, 16, 24, 15);

            Assert.Equal("4:24:15 PM", field.ConvertToString(dateTime));
        }

        #endregion

        #region Numeric Tests

        [Fact]
        public void IntField()
        {
            var property = typeof(TestDataClass).GetProperty("IntField");
            var field = new TestField(property);

            Assert.Equal("1234", field.ConvertToString(1234));
        }

        [Theory]
        [InlineData(123, "123")]
        [InlineData(1234, "1,234")]
        public void GroupedIntField(int value, string expectedValue)
        {
            var property = typeof(TestDataClass).GetProperty("GroupedIntField");
            var field = new TestField(property);

            Assert.Equal(expectedValue, field.ConvertToString(value));
        }

        [Theory]
        [InlineData(123, "$123.00")]
        [InlineData(1234, "$1,234.00")]
        [InlineData(1234.5, "$1,234.50")]
        public void MoneyField(decimal value, string expectedValue)
        {
            var property = typeof(TestDataClass).GetProperty("MoneyField");
            var field = new TestField(property);

            Assert.Equal(expectedValue, field.ConvertToString(value));
        }

        #endregion

        #region Custom Converter Tests

        [Fact]
        public void TicketNumberField()
        {
            var property = typeof(TestDataClass).GetProperty("TicketNumberField");
            var field = new TestField(property);

            Assert.Equal("123-456-789", field.ConvertToString(123456789));
        }

        #endregion

        private class TestField : FieldBase
        {
            public TestField(PropertyInfo property) : base(property)
            {
            }
        }

        private class TestFieldAttribute : FieldAttribute
        {
        }

        private class TestFieldConverter : IFieldConverter
        {
            public bool CanConvertToValue()
            {
                return false;
            }

            public object ConvertToValue(string value, PropertyInfo property)
            {
                throw new NotImplementedException();
            }

            public bool CanConvertToString()
            {
                return true;
            }

            public string ConvertToString(object value, PropertyInfo property)
            {
                return value.ToString().Insert(6, "-").Insert(3, "-");
            }
        }

        private class TestDataClass
        {
            public string StringField { get; set; }

            public DateTime DateTimeField { get; set; }

            [TestField(FormatString = "MMddyyyyHHmm")]
            public DateTime DateTime_MMddyyyyHHmm_Field { get; set; }

            [TestField(FormatString = "yyyyMMdd")]
            public DateTime Date_yyyyMMdd_Field { get; set; }

            [TestField(FormatString = "h:mm:ss tt")]
            public DateTime Time_h_mm_ss_tt_Field { get; set; }

            public bool BooleanField { get; set; }

            [BooleanFormat(TrueValue = "On", FalseValue = "Off")]
            public bool OnOffField { get; set; }

            public int IntField { get; set; }

            [TestField(FormatString = "n0")]
            public int GroupedIntField { get; set; }

            [TestField(FormatString = "C")]
            public decimal MoneyField { get; set; }

            [TestField(FieldConverter = typeof(TestFieldConverter))]
            public int TicketNumberField { get; set; }
        }
    }
}