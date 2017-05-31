//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor
//    File Name: FileProcessor.cs
//  
//    Author:    Andrew - 2017/05/30
//  ---------------------------------------------

namespace FileProcessor.Engines
{
    using System.IO;

    public class FileProcessor<T> : EngineBase<T> where T : class, new()
    {
        public FileInfo FileInfo { get; set; }

        public FileProcessor(string fileName)
        {
            FileInfo = new FileInfo(fileName);
        }

        protected override TextReader GetReader()
        {
            return FileInfo.OpenText();
        }

        protected override TextWriter GetWriter(bool append)
        {
            return append ? FileInfo.AppendText() : FileInfo.CreateText();
        }
    }
}