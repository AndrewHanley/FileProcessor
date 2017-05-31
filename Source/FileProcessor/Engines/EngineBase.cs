//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor
//    File Name: EngineBase.cs
//  
//    Author:    Andrew - 2017/05/30
//  ---------------------------------------------

namespace FileProcessor.Engines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Records;

    public abstract class EngineBase<T> : IDisposable where T : class, new()
    {
        #region Properties

        public TextReader Reader { get; set; }
        public TextWriter Writer { get; set; }

        public int LineNumber { get; set; }

        protected RecordProcessor<T> RecordProcessor { get; set; }
        //protected List<IRecordProcessor> ProcessorTypes { get; set; }

        #endregion

        protected EngineBase()
        {
            LineNumber = -1;
            RecordProcessor = RecordBuilder.CreateProcessor<T>();
        }

        #region Record Processor Management

        //protected void AddRecordProcessor(Type recordType)
        //{
        //    ProcessorTypes.Add((IRecordProcessor)Activator.CreateInstance(typeof(RecordProcessor<>).MakeGenericType(recordType)));
        //}

        #endregion

        #region Read Operations

        protected IEnumerable<T> ReadRecords()
        {
            using (Reader = GetReader())
            {
                T entity;

                while ((entity = ReadRecord()) != null)
                    yield return entity;
            }
        }

        private T ReadRecord()
        {
            // Check Reader??
            return ReadRecord(1);
        }

        private T ReadRecord(int recordLines)
        {
            var builder = new StringBuilder(recordLines);

            for (var lineCount = 0; lineCount < recordLines; lineCount++)
            {
                builder.AppendLine(Reader.ReadLine());
                LineNumber++;
            }

            var record = builder.ToString();

            return record == null ? null : RecordProcessor.ReadRecord(record);
        }

        #endregion

        #region Write Operations

        protected virtual void WriteRecords(IEnumerable<T> entities, bool append = false)
        {
            using (Writer = GetWriter(append))
            {
                foreach (var entity in entities)
                    Writer.WriteLine(RecordProcessor.WriteRecord(entity));
            }
        }

        #endregion

        #region Abstract Methods

        protected abstract TextReader GetReader();
        protected abstract TextWriter GetWriter(bool append);

        #endregion

        public virtual void Dispose()
        {
            Reader?.Dispose();
            Writer?.Dispose();
        }
    }
}