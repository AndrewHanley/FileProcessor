//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Example.Moves
//    File Name: RegisteredOwner.cs
//  
//    Author:    Andrew - 2017/05/31
//  ---------------------------------------------

namespace FileProcessor.Example.Moves.Models
{
    using System;
    using Fields.FieldAttributes;
    using Records.RecordAttributes;

    [FixedLengthRecord]
    public class RegisteredOwner
    {
        [FixedLengthField(8)]
        public string PlateNumber { get; set; }

        [FixedLengthField(12)]
        public string TicketNumber { get; set; }

        // 021 to 028	FILLER
        // 029 to 030 	REGISTRATION YEAR	PIC 9(02).
        // 031 to 032 	VEHICLE YEAR		PIC 9(02).
        [FixedLengthField(12)]
        public string Filler1 { get; set; }

        [FixedLengthField(15)]
        public string Make { get; set; }

        [FixedLengthField(15)]
        public string Model { get; set; }

        [FixedLengthField(17)]
        public string VIN { get; set; }

        [FixedLengthField(8)]
        public string Style { get; set; }

        [FixedLengthField(1)]
        public char Gender { get; set; }

        [FixedLengthField(20)]
        public string Status { get; set; }

        // SOMETHING FROM 109 to 128
        [FixedLengthField(20)]
        public string Filler2 { get; set; }

        [FixedLengthField(44)]
        public string OwnersName { get; set; }

        [FixedLengthField(8)]
        public string Colour { get; set; }

        [FixedLengthField(12)]
        public string FirstName { get; set; }

        [FixedLengthField(12)]
        public string MiddleName { get; set; }

        [FixedLengthField(8, FormatString = "yyyyMMdd")]
        public DateTime DateOfBirth { get; set; }

        public Address PostalAddress { get; set; }

        // SOMETHING FROM 373 to 384
        [FixedLengthField(12)]
        public string Filler3 { get; set; }

        public Address PhysicalAddress { get; set; }

        // SOMETHING FROM 564 to 575
        [FixedLengthField(12)]
        public string Filler4 { get; set; }

        [FixedLengthField(10)]
        public string MVID { get; set; }

        [FixedLengthField(4)]
        public int RegistrationYear { get; set; }

        [FixedLengthField(4)]
        public int VehicleYear { get; set; }
    }

    public class Address
    {
        [FixedLengthField(44)]
        public string AddressLine1 { get; set; }

        [FixedLengthField(44)]
        public string AddressLine2 { get; set; }

        [FixedLengthField(21)]
        public string City { get; set; }

        [FixedLengthField(21)]
        public string Province { get; set; }

        [FixedLengthField(20)]
        public string Country { get; set; }

        [FixedLengthField(10)]
        public string PostalCode { get; set; }
    }
}