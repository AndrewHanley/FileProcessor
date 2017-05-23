//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: FixedLengthFieldAttribute.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Engine.Fields.FieldAttributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class FixedLengthFieldAttribute : FieldAttribute
    {
        public int Length { get; set; }

        public int StartIndex { get; set; }

        public FieldAlignment FieldAlignment { get; set; }

        public char PaddingCharacter { get; set; }

        public bool TruncateField { get; set; }

        public FixedLengthFieldAttribute()
        {
            StartIndex = -1;
            FieldAlignment = FieldAlignment.Default;
            PaddingCharacter = '\0';
            TruncateField = false;
        }

        public FixedLengthFieldAttribute(int length) : this()
        {
            Length = length;
        }
    }
}