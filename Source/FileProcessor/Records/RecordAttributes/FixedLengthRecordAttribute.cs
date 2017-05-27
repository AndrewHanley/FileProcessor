//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: FixedLengthRecordAttribute.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Records.RecordAttributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class FixedLengthRecordAttribute : RecordAttribute
    {
    }
}