//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Test
//    File Name: FieldBase.Validation.Test.cs
//  
//    Author:    Andrew - 2017/05/27
//  ---------------------------------------------

namespace FileProcessor.Test.Fields
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using FileProcessor.Fields;
    using Xunit;

    public class FieldBaseValidationTest
    {
        #region Validation Tests

        [Theory]
        [InlineData("Valid", 0)]
        [InlineData("", 1)]
        [InlineData(null, 1)]
        public void RequiredField(string value, int expectedValue)
        {
            var property = typeof(ValidationClass).GetProperty("RequiredField");
            var field = new TestField(property);

            var entity = new ValidationClass {RequiredField = value};
            var context = new ValidationContext(entity) {MemberName = "RequiredField"};

            var errors = field.Validate(value, context);

            Assert.Equal(expectedValue, errors.Count());
        }

        [Theory]
        [InlineData(50, 0)]
        [InlineData(5, 1)]
        [InlineData(500, 1)]
        [InlineData(null, 1)]
        public void RangeField(int value, int expectedValue)
        {
            var property = typeof(ValidationClass).GetProperty("RangeField");
            var field = new TestField(property);

            var entity = new ValidationClass {RangeField = value};
            var context = new ValidationContext(entity) {MemberName = "RangeField"};

            var errors = field.Validate(value, context);

            Assert.Equal(expectedValue, errors.Count());
        }

        [Theory]
        [InlineData('A', 0)]
        [InlineData('B', 0)]
        [InlineData('C', 0)]
        [InlineData('D', 1)]
        [InlineData(null, 1)]
        public void RegExField(char value, int expectedValue)
        {
            var property = typeof(ValidationClass).GetProperty("RegExField");
            var field = new TestField(property);

            var entity = new ValidationClass {RegExField = value};
            var context = new ValidationContext(entity) {MemberName = "RegExField"};

            var errors = field.Validate(value, context);

            Assert.Equal(expectedValue, errors.Count());
        }

        [Theory]
        [InlineData("Good", 0)]
        [InlineData("Bad String", 1)]
        public void LengthField(string value, int expectedValue)
        {
            var property = typeof(ValidationClass).GetProperty("LengthField");
            var field = new TestField(property);

            var entity = new ValidationClass {LengthField = value};
            var context = new ValidationContext(entity) {MemberName = "LengthField"};

            var errors = field.Validate(value, context);

            Assert.Equal(expectedValue, errors.Count());
        }

        #endregion

        private class TestField : FieldBase
        {
            public TestField(PropertyInfo property) : base(property)
            {
            }
        }

        private class ValidationClass
        {
            [Required]
            public string RequiredField { get; set; }

            [Range(10, 100)]
            public int RangeField { get; set; }

            [RegularExpression("[ABC]")]
            public char RegExField { get; set; }

            [MaxLength(5)]
            public string LengthField { get; set; }
        }
    }
}