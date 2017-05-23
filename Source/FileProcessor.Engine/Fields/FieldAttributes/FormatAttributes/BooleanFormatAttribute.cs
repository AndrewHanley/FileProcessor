//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: BooleanFormatAttribute.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Engine.Fields.FieldAttributes.FormatAttributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class BooleanFormatAttribute : Attribute
    {
        public string TrueValue { get; set; }

        public string FalseValue { get; set; }
    }
}