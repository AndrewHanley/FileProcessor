//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: RecordAttribute.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Engine.Records.RecordAttributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public abstract class RecordAttribute : Attribute
    {
    }
}