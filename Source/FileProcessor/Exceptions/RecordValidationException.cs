//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor
//    File Name: RecordValidationException.cs
//  
//    Author:    Andrew - 2017/05/27
//  ---------------------------------------------

namespace FileProcessor.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fields.Validation;

    public class RecordValidationException : Exception
    {
        #region Properties

        public List<FieldValidationError> Errors { get; set; }

        public List<string> ErrorMessages => Errors?.Select(e => e.ErrorMessage).ToList();

        #endregion

        public RecordValidationException(List<FieldValidationError> errors) : this(errors, null)
        {
        }

        public RecordValidationException(List<FieldValidationError> errors, Exception innerException)
            : base(GenerateMessage(errors), innerException)
        {
            Errors = errors;
        }

        private static string GenerateMessage(IEnumerable<FieldValidationError> errors)
        {
            return string.Join("\r\n", errors?.Select(e => e.ErrorMessage));
        }
    }
}