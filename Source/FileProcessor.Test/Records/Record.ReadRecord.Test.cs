//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Test
//    File Name: Record.ReadRecord.Test.cs
//  
//    Author:    Andrew - 2017/05/27
//  ---------------------------------------------

namespace FileProcessor.Test.Records
{
    using System;
    using FileProcessor.Fields.FieldAttributes;
    using FileProcessor.Fields.FieldAttributes.FormatAttributes;
    using FileProcessor.Records;
    using FileProcessor.Records.RecordAttributes;
    using Xunit;

    public class RecordReadRecordTest
    {
        #region Delimited Record Read Tests

        [Fact]
        public void ReadDelimitedClientRecord()
        {
            var record = RecordProcessor.CreateRecord<Client>();
            var client = record.ReadRecord("1,Client A");

            Assert.Equal(1, client.ClientId);
            Assert.Equal("Client A", client.Name);
        }

        [Fact]
        public void ReadDelimitedEmployeeWithDelimiterInQuoteRecord()
        {
            var record = RecordProcessor.CreateRecord<Employee>();
            var employee = record.ReadRecord("2,Jones,Vinnie,\"$1,500,000.00\"");

            Assert.Equal(2, employee.EmployeeId);
            Assert.Equal("Jones", employee.LastName);
            Assert.Equal("Vinnie", employee.FirstName);
            Assert.Equal(1500000, employee.Salary);
        }

        [Fact]
        public void ReadDelimitedWithQuoteInQuoteEmployeeRecord()
        {
            var record = RecordProcessor.CreateRecord<Actor>();
            var actor = record.ReadRecord("\"Dwayne \"\"The Rock\"\" Johnson\",\"19720502\"");

            Assert.Equal("Dwayne \"The Rock\" Johnson", actor.Name);
            Assert.Equal(new DateTime(1972, 5, 2), actor.BirthDate);
        }

        [Fact]
        public void ReadDelimitedNestedRecord()
        {
            var record = RecordProcessor.CreateRecord<Contact>();
            var contact = record.ReadRecord("[John Wick]|[(555) 444-9999]|[12323 - 45 street]|[Edmonton]|[AB]|[T6W 1T4]");

            Assert.Equal("John Wick", contact.Name);
            Assert.Equal("(555) 444-9999", contact.PhoneNumber);
            Assert.Equal("12323 - 45 street", contact.Address.StreetAddress);
            Assert.Equal("Edmonton", contact.Address.City);
            Assert.Equal("AB", contact.Address.Province);
            Assert.Equal("T6W 1T4", contact.Address.PostalCode);
        }

        [DelimitedRecord(Delimiter = ",")]
        private class Client
        {
            public int ClientId { get; set; }

            public string Name { get; set; }
        }

        [DelimitedRecord(Delimiter = ",")]
        private class Employee
        {
            public int EmployeeId { get; set; }

            public string LastName { get; set; }

            public string FirstName { get; set; }

            [DelimitedField(FormatString = "C", QuoteCharacter = '"')]
            public decimal Salary { get; set; }
        }

        [DelimitedRecord(Delimiter = ",", QuoteCharacter = '"')]
        private class Actor
        {
            public string Name { get; set; }

            [DelimitedField(FormatString = "yyyyMMdd")]
            public DateTime BirthDate { get; set; }
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
        public void ReadFixedLengthVehicleRecord()
        {
            var record = RecordProcessor.CreateRecord<Vehicle>();
            var vehicle = record.ReadRecord("Ferrari             Enzo                2004");

            Assert.Equal("Ferrari", vehicle.Make);
            Assert.Equal("Enzo", vehicle.Model);
            Assert.Equal(2004, vehicle.Year);
        }

        [Fact]
        public void ReadNestedFixedLengthRecord()
        {
            var record = RecordProcessor.CreateRecord<Book>();
            var book = record.ReadRecord("978-0439554930Harry Potter and the Philosopher's Stone          Rowling             J.K.                0922");

            Assert.Equal("978-0439554930", book.IsbnNumber);
            Assert.Equal("Harry Potter and the Philosopher's Stone", book.Name);
            Assert.Equal("Rowling", book.Author.LastName);
            Assert.Equal("J.K.", book.Author.FirstName);
            Assert.Equal(9.22m, book.Price);
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

            [FixedLengthField(4, PaddingCharacter = '0')]
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