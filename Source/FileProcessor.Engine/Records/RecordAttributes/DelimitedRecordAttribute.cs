﻿//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: DelimitedRecordAttribute.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Engine.Records.RecordAttributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class DelimitedRecordAttribute : RecordAttribute
    {
        public char Delimiter { get; set; }
    }
}