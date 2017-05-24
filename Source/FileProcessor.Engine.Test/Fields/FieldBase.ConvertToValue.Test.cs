//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine.Test
//    File Name: FieldBase.ConvertToValue.Test.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Engine.Test.Fields
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using Engine.Fields;
    using Engine.Fields.Converters;
    using Engine.Fields.FieldAttributes;
    using Engine.Fields.FieldAttributes.FormatAttributes;
    using Exceptions;
    using Xunit;

    public class FieldBaseConvertToValueTest
    {
        #region Null Value Tests

        [Fact]
        public void Nullable()
        {
            var property = typeof(TestDataClass).GetProperty("NullableField");
            var field = new TestField(property, 1);

            Assert.Equal(null, field.ConvertToValue(null));
        }

        [Fact]
        public void NullToDefault()
        {
            var property = typeof(TestDataClass).GetProperty("NullField");
            var field = new TestField(property, 1);

            Assert.Equal(0, field.ConvertToValue(null));
        }

        [Fact]
        public void NullException()
        {
            var property = typeof(TestDataClass).GetProperty("IntField");
            var field = new TestField(property, 1);

            var exception = Assert.Throws<ConversionException>(() => field.ConvertToValue(null));

            Assert.Equal(typeof(NullReferenceException), exception.InnerException.GetType());
        }

        #endregion

        #region String Field Tests

        [Fact]
        public void StringField()
        {
            var property = typeof(TestDataClass).GetProperty("StringField");
            var field = new TestField(property, 1);

            Assert.Equal("Field Value", field.ConvertToValue("Field Value"));
        }

        #endregion

        #region Char Field Tests

        [Fact]
        public void CharField()
        {
            var property = typeof(TestDataClass).GetProperty("CharField");
            var field = new TestField(property, 1);

            Assert.Equal('A', field.ConvertToValue("A"));
        }

        #endregion

        #region Date/Time Field Tests

        [Fact]
        public void DateTimeFieldNoFormat()
        {
            var property = typeof(TestDataClass).GetProperty("DateTimeField");
            var field = new TestField(property, 1);
            var dateTime = DateTime.Today.AddHours(12).AddMinutes(15);

            Assert.Equal(dateTime, field.ConvertToValue(dateTime.ToString(CultureInfo.CurrentCulture)));
        }

        [Fact]
        public void DateTime_yyyyMMddHHmmss()
        {
            var property = typeof(TestDataClass).GetProperty("DateTime_yyyyMMddHHmmss_Field");
            var field = new TestField(property, 1);
            var dateTime = new DateTime(1973, 3, 30, 13, 45, 55);

            Assert.Equal(dateTime, field.ConvertToValue("19730330134555"));
        }

        [Fact]
        public void Date_yyyyMMdd()
        {
            var property = typeof(TestDataClass).GetProperty("Date_yyyyMMdd_Field");
            var field = new TestField(property, 1);
            var dateTime = new DateTime(1973, 3, 30);

            Assert.Equal(dateTime, field.ConvertToValue("19730330"));
        }

        [Fact]
        public void Time_h_mm_tt()
        {
            var property = typeof(TestDataClass).GetProperty("Time_h_mm_tt_Field");
            var field = new TestField(property, 1);
            var dateTime = DateTime.Today.AddHours(13).AddMinutes(45);

            Assert.Equal(dateTime, field.ConvertToValue("1:45 PM"));
        }


        [Fact]
        public void DateTimeException()
        {
            var property = typeof(TestDataClass).GetProperty("DateTimeExceptionField");
            var field = new TestField(property, 1);
            var dateTime = new DateTime(1973, 3, 30, 13, 45, 0);

            var exception = Assert.Throws<ConversionException>(() => field.ConvertToValue(dateTime.ToString(CultureInfo.CurrentCulture)));

            Assert.Equal("DateTimeExceptionField field failed to convert 3/30/1973 1:45:00 PM from String to DateTime. See inner exception for details.", exception.Message);
            Assert.Equal("3/30/1973 1:45:00 PM is not valid to convert with the yyyyMMdd format string.", exception.InnerException.Message);
        }

        #endregion

        #region Boolean Field Tests

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        public void BooleanField(string value, bool expectedResult)
        {
            var property = typeof(TestDataClass).GetProperty("BooleanField");
            var field = new TestField(property, 1);

            Assert.Equal(expectedResult, field.ConvertToValue(value));
        }

        [Theory]
        [InlineData("Yes", true)]
        [InlineData("No", false)]
        public void YesNoField(string value, bool expectedResult)
        {
            var property = typeof(TestDataClass).GetProperty("YesNoField");
            var field = new TestField(property, 1);

            Assert.Equal(expectedResult, field.ConvertToValue(value));
        }

        [Fact]
        public void BooleanFieldException()
        {
            var property = typeof(TestDataClass).GetProperty("BooleanField");
            var field = new TestField(property, 1);

            var exception = Assert.Throws<ConversionException>(() => field.ConvertToValue("NOPER"));

            Assert.Equal(typeof(FormatException), exception.InnerException.GetType());
            Assert.Equal("NOPER is not a valid boolean value.", exception.InnerException.Message);
        }

        #endregion

        #region Int Field Tests

        [Theory]
        [InlineData("-2147483648", int.MinValue)]
        [InlineData("2147483647", int.MaxValue)]
        [InlineData("44", 44)]
        public void IntField(string value, int expectedResult)
        {
            var property = typeof(TestDataClass).GetProperty("IntField");
            var field = new TestField(property, 1);

            Assert.Equal(expectedResult, field.ConvertToValue(value));
        }

        [Fact]
        public void IntFieldException()
        {
            var property = typeof(TestDataClass).GetProperty("IntField");
            var field = new TestField(property, 1);

            var exception = Assert.Throws<ConversionException>(() => field.ConvertToValue("3147483647"));

            Assert.Equal(typeof(OverflowException), exception.InnerException.GetType());
        }

        #endregion

        #region Decimal Field Tests

        [Theory]
        [InlineData("1,234", 1234)]
        [InlineData("123.4", 123.4)]
        [InlineData("0.1234", 0.1234)]
        [InlineData("$123.40", 123.4)]
        public void DecimalField(string value, decimal expectedResult)
        {
            var property = typeof(TestDataClass).GetProperty("DecimalField");
            var field = new TestField(property, 1);

            Assert.Equal(expectedResult, field.ConvertToValue(value));
        }

        [Theory]
        [InlineData("123.44", 123.44)]
        [InlineData("123.446", 123.45)]
        [InlineData("123.443", 123.44)]
        public void Decimal2PlacesField(string value, decimal expectedResult)
        {
            var property = typeof(TestDataClass).GetProperty("Decimal2PlacesField");
            var field = new TestField(property, 1);

            Assert.Equal(expectedResult, field.ConvertToValue(value));
        }

        [Theory]
        [InlineData("12344", 123.44)]
        [InlineData("123445", 1234.45)]
        [InlineData("123443", 1234.43)]
        public void DecimalNoSeperatorField(string value, decimal expectedResult)
        {
            var property = typeof(TestDataClass).GetProperty("DecimalNoSeperatorField");
            var field = new TestField(property, 1);

            Assert.Equal(expectedResult, field.ConvertToValue(value));
        }

        [Fact]
        public void DecimalFieldException()
        {
            var property = typeof(TestDataClass).GetProperty("DecimalField");
            var field = new TestField(property, 1);

            var exception = Assert.Throws<ConversionException>(() => field.ConvertToValue("ABC"));

            Assert.Equal(typeof(FormatException), exception.InnerException.GetType());
        }

        #endregion

        #region Custom Converter Tests

        [Fact]
        public void TicketNumberField()
        {
            var property = typeof(TestDataClass).GetProperty("TicketNumberField");
            var field = new TestField(property, 1);

            Assert.Equal(123456789, field.ConvertToValue("123-456-789"));
        }

        [Fact]
        public void ConverterExceptionField()
        {
            var property = typeof(TestDataClass).GetProperty("ConverterExceptionField");
            Assert.Throws<InvalidCastException>(() => new TestField(property, 1));
        }

        #endregion

        private class TestField : FieldBase
        {
            public TestField(PropertyInfo property, int order) : base(property, order)
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
                return true;
            }

            public object ConvertToValue(string value, PropertyInfo property)
            {
                return int.Parse(value.Replace("-", string.Empty));
            }

            public bool CanConvertToString()
            {
                return false;
            }

            public string ConvertToString(object value, PropertyInfo property)
            {
                throw new NotImplementedException();
            }
        }

        private class BadFieldConverter
        {
        }

        private class TestDataClass
        {
            public string StringField { get; set; }

            public char CharField { get; set; }

            #region Date/Time Fields

            public DateTime DateTimeField { get; set; }

            [TestField(FormatString = "yyyyMMddHHmmss")]
            public DateTime DateTime_yyyyMMddHHmmss_Field { get; set; }

            [TestField(FormatString = "yyyyMMdd")]
            public DateTime Date_yyyyMMdd_Field { get; set; }

            [TestField(FormatString = "h:mm tt")]
            public DateTime Time_h_mm_tt_Field { get; set; }

            [TestField(FormatString = "yyyyMMdd")]
            public DateTime DateTimeExceptionField { get; set; }

            #endregion

            public bool BooleanField { get; set; }

            [BooleanFormat(TrueValue = "Yes")]
            public bool YesNoField { get; set; }

            public int IntField { get; set; }

            public decimal DecimalField { get; set; }

            [DecimalFormat(2)]
            public decimal Decimal2PlacesField { get; set; }

            [DecimalFormat(2, IncludesDecimalSeperator = false)]
            public decimal DecimalNoSeperatorField { get; set; }

            public int? NullableField { get; set; }

            [TestField(ThrowExceptionOnNull = false)]
            public int NullField { get; set; }

            [TestField(FieldConverter = typeof(TestFieldConverter))]
            public int TicketNumberField { get; set; }

            [TestField(FieldConverter = typeof(BadFieldConverter))]
            public int ConverterExceptionField { get; set; }
        }
    }
}