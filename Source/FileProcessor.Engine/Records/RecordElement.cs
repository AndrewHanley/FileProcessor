//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: RecordElement.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Engine.Records
{
    using System.Collections.Generic;
    using System.Reflection;
    using Fields;

    public class RecordElement
    {
        public FieldBase Field { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public List<RecordElement> NestedElements { get; set; }
    }
}