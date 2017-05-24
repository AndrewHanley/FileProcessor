//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine.Test
//    File Name: FieldBase.Test.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Engine.Test.Fields
{
    using System.Reflection;
    using Engine.Fields;
    using Engine.Fields.FieldAttributes;
    using Xunit;

    public class FieldBaseTest
    {
        [Fact]
        public void CreateField()
        {
            var property = typeof(TestDataClass).GetProperty("BooleanField");
            var field = new TestField(property);

            Assert.Equal("BooleanField", field.Name);
            Assert.Equal(-1, field.Order);
            Assert.Equal(typeof(bool), field.FieldType);
            Assert.NotEqual(null, field.FieldProperty);
        }

        [Fact]
        public void FieldAttributeValues()
        {
            var property = typeof(TestDataClass).GetProperty("StringField");
            var field = new TestField(property);

            Assert.Equal("NameOverride", field.Name);
            Assert.Equal(12, field.Order);
            Assert.Equal("CustomFormat", field.FormatString);
            Assert.Equal(false, field.ThrowExceptionOnNull);
        }

        private class TestField : FieldBase
        {
            public TestField(PropertyInfo property) : base(property)
            {
            }
        }

        private class TestFieldAttribute : FieldAttribute
        {
        }

        private class TestDataClass
        {
            public bool BooleanField { get; set; }

            [TestField(Order = 12, Name = "NameOverride", FormatString = "CustomFormat", ThrowExceptionOnNull = false)]
            public string StringField { get; set; }
        }
    }
}