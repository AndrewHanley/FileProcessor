//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: FixedLengthRecordElement.cs
//  
//    Author:    Andrew - 2017/05/24
//  ---------------------------------------------

namespace FileProcessor.Records.RecordElements
{
    public class FixedLengthRecordElement : RecordElementBase
    {
        public int Length { get; set; }
    }
}