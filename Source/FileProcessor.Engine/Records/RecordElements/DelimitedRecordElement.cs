//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: DelimitedRecordElement.cs
//  
//    Author:    Andrew - 2017/05/24
//  ---------------------------------------------

namespace FileProcessor.Records.RecordElements
{
    public class DelimitedRecordElement : RecordElementBase
    {
        public char QuoteCharacter { get; set; }

        public char EndQuoteCharacter { get; set; }
    }
}