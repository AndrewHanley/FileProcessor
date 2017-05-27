//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: DecimalFormatAttribute.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Fields.FieldAttributes.FormatAttributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class DecimalFormatAttribute : Attribute
    {
        public int DecimalPlaces { get; set; }
        public bool IncludesDecimalSeperator { get; set; }

        public DecimalFormatAttribute()
        {
            DecimalPlaces = -1;
            IncludesDecimalSeperator = true;
        }

        public DecimalFormatAttribute(int decimalPlaces) : this()
        {
            DecimalPlaces = decimalPlaces;
        }
    }
}