//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: DelimitedFieldAttribute.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Fields.FieldAttributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DelimitedFieldAttribute : FieldAttribute
    {
        public char QuoteCharacter { get; set; }

        public char EndQuoteCharacter { get; set; }


        public DelimitedFieldAttribute()
        {
            QuoteCharacter = '\0';
            EndQuoteCharacter = '\0';
        }

        public DelimitedFieldAttribute(char quoteCharacter) : this()
        {
            QuoteCharacter = quoteCharacter;
        }
    }
}