//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Engine
//    File Name: IFieldConverter.cs
//  
//    Author:    Andrew - 2017/05/23
//  ---------------------------------------------

namespace FileProcessor.Engine.Fields.Converters
{
    using System.Reflection;

    public interface IFieldConverter
    {
        bool CanConvertToValue();

        object ConvertToValue(string value, PropertyInfo property);

        bool CanConvertToString();

        string ConvertToString(object value, PropertyInfo property);
    }
}