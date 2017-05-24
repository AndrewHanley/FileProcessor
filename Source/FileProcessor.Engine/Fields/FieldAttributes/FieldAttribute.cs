//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: FieldAttribute.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Engine.Fields.FieldAttributes
{
    using System;
    using Converters;
    using Exceptions;
    using Resources;

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class FieldAttribute : Attribute
    {
        public int Order { get; set; }

        public string Name { get; set; }

        public bool ThrowExceptionOnNull { get; set; }

        public string FormatString { get; set; }

        public Type FieldConverter { get; set; }

        protected FieldAttribute()
        {
            Order = -1;
            ThrowExceptionOnNull = true;
        }
    }
}