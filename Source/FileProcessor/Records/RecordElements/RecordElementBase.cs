//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: RecordElement.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Records.RecordElements
{
    using System.Collections.Generic;
    using Fields;

    public class RecordElementBase
    {
        public string FieldName { get; set; }

        public FieldBase Field { get; set; }

        public int Order { get; set; }

        public bool IncludesValidation { get; set; }

        public List<RecordElementBase> NestedElements { get; set; }
    }
}