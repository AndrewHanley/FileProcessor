//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Example.Moves
//    File Name: TicketPlate.cs
//  
//    Author:    Andrew - 2017/05/30
//  ---------------------------------------------

namespace FileProcessor.Example.Moves.Models
{
    using Fields.FieldAttributes;
    using Records.RecordAttributes;

    [FixedLengthRecord]
    public class TicketPlate
    {
        [FixedLengthField(8)]
        public string PlateNumber { get; set; }

        [FixedLengthField(10)]
        public string TicketNumber { get; set; }
    }
}