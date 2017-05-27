//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Test
//    File Name: RecordBase.WriteRecord.Test.cs
//  
//    Author:    Andrew - 2017/05/25
//  ---------------------------------------------

namespace FileProcessor.Test.Records
{
    using FileProcessor.Fields.FieldAttributes;
    using FileProcessor.Fields.FieldAttributes.FormatAttributes;
    using FileProcessor.Records;
    using FileProcessor.Records.RecordAttributes;
    using Xunit;

    public class RecordWriteRecordTest
    {
        #region Delimited Record Tests

        [Fact]
        public void WriteDelimitedEmployeeRecord()
        {
            var record = RecordProcessor.CreateRecord<Employee>();
            var employee = new Employee
                           {
                               EmployeeId = 1,
                               LastName = "Slug",
                               FirstName = "Bub",
                               Salary = 45000
                           };

            Assert.Equal("1,Slug,Bub,$45,000.00", record.WriteRecord(employee));
        }

        [Fact]
        public void WriteDelimitedNestedRecord()
        {
            var record = RecordProcessor.CreateRecord<Contact>();
            var contact = new Contact
                          {
                              Name = "John Wick",
                              PhoneNumber = "(555) 444-9999",
                              Address = new Address
                                        {
                                            StreetAddress = "12323 - 45 street",
                                            City = "Edmonton",
                                            Province = "AB",
                                            PostalCode = "T6W 1T4"
                                        }
                          };

            Assert.Equal("[John Wick]|[(555) 444-9999]|[12323 - 45 street]|[Edmonton]|[AB]|[T6W 1T4]", record.WriteRecord(contact));
        }

        [DelimitedRecord(Delimiter = ",")]
        private class Employee
        {
            public int EmployeeId { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }

            [DelimitedField(FormatString = "C")]
            public decimal Salary { get; set; }
        }

        [DelimitedRecord(Delimiter = "|", QuoteCharacter = '[', EndQuoteCharacter = ']')]
        private class Contact
        {
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public Address Address { get; set; }
        }

        private class Address
        {
            public string StreetAddress { get; set; }
            public string City { get; set; }
            public string Province { get; set; }
            public string PostalCode { get; set; }
        }

        #endregion

        #region Fixed Length Record Tests

        [Fact]
        public void WriteFixedLengthVehicleRecord()
        {
            var record = RecordProcessor.CreateRecord<Vehicle>();
            var vehicle = new Vehicle
                          {
                              Make = "Ferrari",
                              Model = "Enzo",
                              Year = 2004
                          };

            Assert.Equal("Ferrari             Enzo                2004", record.WriteRecord(vehicle));
        }

        [Fact]
        public void WriteNestedFixedLengthRecord()
        {
            var record = RecordProcessor.CreateRecord<Book>();
            var book = new Book
                       {
                           IsbnNumber = "978-0439554930",
                           Name = "Harry Potter and the Philosopher's Stone",
                           Author = new Author
                                    {
                                        LastName = "Rowling",
                                        FirstName = "J.K."
                                    },
                           Price = 9.22m
                       };

            Assert.Equal("978-0439554930Harry Potter and the Philosopher's Stone          Rowling             J.K.                0922", record.WriteRecord(book));
        }

        [FixedLengthRecord]
        private class Vehicle
        {
            [FixedLengthField(20)]
            public string Make { get; set; }

            [FixedLengthField(20)]
            public string Model { get; set; }

            [FixedLengthField(4)]
            public int Year { get; set; }
        }

        [FixedLengthRecord]
        private class Book
        {
            [FixedLengthField(14)]
            public string IsbnNumber { get; set; }

            [FixedLengthField(50)]
            public string Name { get; set; }

            public Author Author { get; set; }

            [FixedLengthField(4)]
            [DecimalFormat(DecimalPlaces = 2, IncludesDecimalSeperator = false)]
            public decimal Price { get; set; }
        }

        private class Author
        {
            [FixedLengthField(20)]
            public string LastName { get; set; }

            [FixedLengthField(20)]
            public string FirstName { get; set; }
        }

        #endregion
    }
}