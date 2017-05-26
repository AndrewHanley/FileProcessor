//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: DelimitedRecordAttribute.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Records.RecordAttributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class DelimitedRecordAttribute : RecordAttribute
    {
        public string Delimiter { get; set; }

        public char QuoteCharacter { get; set; }
        public char EndQuoteCharacter { get; set; }

        public DelimitedRecordAttribute()
        {
            QuoteCharacter = '\0';
            EndQuoteCharacter = '\0';
        }
    }
}