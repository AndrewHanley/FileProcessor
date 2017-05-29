//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor
//    File Name: FieldValidationError.cs
//  
//    Author:    Andrew - 2017/05/27
//  ---------------------------------------------

namespace FileProcessor.Fields.Validation
{
    using System;

    public class FieldValidationError
    {
        public string FieldName { get; set; }
        public object FieldValue { get; set; }
        public Type ValidationType { get; set; }
        public string ErrorMessage { get; set; }
    }
}