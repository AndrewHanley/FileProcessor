//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: AttributeException.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Engine.Exceptions
{
    using System;

    public class AttributeException : Exception
    {
        public AttributeException(string message, params object[] args) : base(string.Format(message, args))
        {
        }

        public AttributeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}