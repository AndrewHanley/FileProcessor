//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Test
//    File Name: Record.Validation.Test.cs
//  
//    Author:    Andrew - 2017/05/28
//  ---------------------------------------------

namespace FileProcessor.Test.Records
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Exceptions;
    using FileProcessor.Fields.FieldAttributes;
    using FileProcessor.Records;
    using FileProcessor.Records.RecordAttributes;
    using Xunit;

    public class RecordValidationTest
    {
        #region Write Record Validation Tests

        [Fact]
        public void WriteValidEmployee()
        {
            var record = RecordBuilder.CreateProcessor<Employee>();
            var employee = new Employee
                           {
                               LastName = "Sparrow",
                               FirstName = "Jack",
                               Address = new Address
                                         {
                                             StreetAddress = "Black Pearl",
                                             City = "Edmonton",
                                             Province = "AB",
                                             PostalCode = "H0H 0H0"
                                         },
                               Salary = 95000
                           };

            Assert.Equal("Sparrow;Jack;Black Pearl;Edmonton;AB;H0H 0H0;$95,000.00", record.WriteRecord(employee));
        }

        [Fact]
        public void WriteInvalidEmployee()
        {
            var record = RecordBuilder.CreateProcessor<Employee>();
            var employee = new Employee
                           {
                               LastName = null,
                               FirstName = "ThisNameIsTooLongForTheField",
                               Address = new Address
                                         {
                                             StreetAddress = null,
                                             City = "CityCannotBeGreaterThan20Characters",
                                             Province = "Prov",
                                             PostalCode = "123456"
                                         },
                               Salary = 125000
                           };

            var exception = Assert.Throws<RecordValidationException>(() => record.WriteRecord(employee));
            var validationMessages = exception.Message.Split(new[] {"\r\n"}, StringSplitOptions.None);

            Assert.Equal("The LastName field is required.", validationMessages[0]);
            Assert.Equal("The field FirstName must be a string or array type with a maximum length of '20'.", validationMessages[1]);
            Assert.Equal("The StreetAddress field is required.", validationMessages[2]);
            Assert.Equal("The field City must be a string or array type with a maximum length of '20'.", validationMessages[3]);
            Assert.Equal("The field Province must be a string or array type with a maximum length of '2'.", validationMessages[4]);
            Assert.Equal("The field PostalCode must match the regular expression '[A-Z][0-9][A-Z] [0-9][A-Z][0-9]'.", validationMessages[5]);
            Assert.Equal("The field Salary must be between 10000 and 100000.", validationMessages[6]);
        }

        #endregion

        #region Read Record Validation Tests

        [Fact]
        public void ReadValidEmployee()
        {
            var record = RecordBuilder.CreateProcessor<Employee>();
            var employee = record.ReadRecord("Sparrow;Jack;Black Pearl;Edmonton;AB;H0H 0H0;$95,000.00");

            Assert.Equal("Sparrow", employee.LastName);
            Assert.Equal("Jack", employee.FirstName);
            Assert.Equal("Black Pearl", employee.Address.StreetAddress);
            Assert.Equal("Edmonton", employee.Address.City);
            Assert.Equal("AB", employee.Address.Province);
            Assert.Equal("H0H 0H0", employee.Address.PostalCode);
            Assert.Equal(95000, employee.Salary);
        }

        [Fact]
        public void ReadInvalidEmployee()
        {
            var record = RecordBuilder.CreateProcessor<Employee>();
            var exception = Assert.Throws<RecordValidationException>(() => record.ReadRecord(";ThisNameIsTooLongForTheField;;CityCannotBeGreaterThan20Characters;Prov;123456;$123,456.00"));

            var validationMessages = exception.Message.Split(new[] {"\r\n"}, StringSplitOptions.None);

            Assert.Equal("The LastName field is required.", validationMessages[0]);
            Assert.Equal("The field FirstName must be a string or array type with a maximum length of '20'.", validationMessages[1]);
            Assert.Equal("The StreetAddress field is required.", validationMessages[2]);
            Assert.Equal("The field City must be a string or array type with a maximum length of '20'.", validationMessages[3]);
            Assert.Equal("The field Province must be a string or array type with a maximum length of '2'.", validationMessages[4]);
            Assert.Equal("The field PostalCode must match the regular expression '[A-Z][0-9][A-Z] [0-9][A-Z][0-9]'.", validationMessages[5]);
            Assert.Equal("The field Salary must be between 10000 and 100000.", validationMessages[6]);
        }

        #endregion

        [DelimitedRecord(Delimiter = ";")]
        private class Employee
        {
            [Required]
            [MaxLength(20)]
            [DelimitedField(ThrowExceptionOnNull = false)]
            public string LastName { get; set; }

            [MaxLength(20)]
            public string FirstName { get; set; }

            [Required]
            public Address Address { get; set; }

            [DelimitedField(FormatString = "C")]
            [Range(10000, 100000)]
            public decimal Salary { get; set; }
        }

        private class Address
        {
            [Required]
            [MaxLength(40)]
            [DelimitedField(ThrowExceptionOnNull = false)]
            public string StreetAddress { get; set; }

            [Required]
            [MaxLength(20)]
            public string City { get; set; }

            [Required]
            [MaxLength(2)]
            public string Province { get; set; }

            // Not a good RegEx but works for testing
            [Required]
            [RegularExpression("[A-Z][0-9][A-Z] [0-9][A-Z][0-9]")]
            public string PostalCode { get; set; }
        }
    }
}