﻿using BinaryGo.Binary.Deserialize;
using BinaryGo.Interfaces;
using BinaryGo.IO;
using BinaryGo.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryGo.Runtime.Variables
{
    /// <summary>
    /// String serializer and deserializer
    /// </summary>
    public class StringVariable : BaseVariable, ISerializationVariable<string>
    {
        /// <summary>
        /// default constructor to initialize
        /// </summary>
        public StringVariable() : base(typeof(string))
        {

        }

        Encoding Encoding;
        /// <summary>
        /// Initalizes TypeGo variable
        /// </summary>
        /// <param name="typeGoInfo">TypeGo variable to initialize</param>
        /// <param name="options">Serializer or deserializer options</param>
        public void Initialize(TypeGoInfo<string> typeGoInfo, ITypeOptions options)
        {
            Encoding = options.Encoding;
            typeGoInfo.IsNoQuotesValueType = false;

            //set the default value of variable
            typeGoInfo.DefaultValue = default;

            //set delegates to access faster and make it pointer directly usage
            typeGoInfo.JsonSerialize = JsonSerialize;

            //set delegates to access faster and make it pointer directly usage for json deserializer
            typeGoInfo.JsonDeserialize = JsonDeserialize;

            //set delegates to access faster and make it pointer directly usage for binary serializer
            typeGoInfo.BinarySerialize = BinarySerialize;

            //set delegates to access faster and make it pointer directly usage for binary deserializer
            typeGoInfo.BinaryDeserialize = BinaryDeserialize;

            IsUnixNewLine = Environment.NewLine.Length == 1;
        }

        bool IsUnixNewLine { get; set; }
        /// <summary>
        /// json serialize
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="value"></param>
        public void JsonSerialize(ref JsonSerializeHandler handler, ref string value)
        {
            handler.TextWriter.Write(JsonConstantsString.Quotes);
            var result = value.AsSpan();
            var len = result.Length;
            //int hasBackSlashNIndex = -1;
            for (int i = 0; i < len; i++)
            {
                if (result[i] == JsonConstantsString.Quotes)
                {
                    handler.TextWriter.Write(JsonConstantsString.BackSlashQuotes);
                }
                else if (result[i] == JsonConstantsString.BackSlash)
                {
                    handler.TextWriter.Write(JsonConstantsString.DoubleBackSlash);
                }
                else if (result[i] == JsonConstantsString.NSpace)
                {
                    //if (hasBackSlashNIndex != i - 1 && IsUnixNewLine)
                    //    handler.TextWriter.Write(JsonConstantsString.BackSlashRN);
                    //else
                        handler.TextWriter.Write(JsonConstantsString.BackSlashN);
                }
                else if (result[i] == JsonConstantsString.RSpace)
                {
                    //hasBackSlashNIndex = i;
                    handler.TextWriter.Write(JsonConstantsString.BackSlashR);
                }
                else if (result[i] == JsonConstantsString.TSpace)
                    handler.TextWriter.Write(JsonConstantsString.BackSlashT);
                else
                    handler.TextWriter.Write(result[i]);
            }
            handler.TextWriter.Write(JsonConstantsString.Quotes);
        }

        /// <summary>
        /// json deserialize
        /// </summary>
        /// <param name="text">json text</param>
        /// <returns>convert text to type</returns>
        public string JsonDeserialize(ref ReadOnlySpan<char> text)
        {
            return new string(text);
        }

        /// <summary>
        /// Binary serialize
        /// </summary>
        /// <param name="stream">stream to write</param>
        /// <param name="value">value to serialize</param>
        public void BinarySerialize(ref BufferBuilder stream, ref string value)
        {
            if (value == null)
            {
                int len = -1;
                stream.Write(ref len);
            }
            else
            {
                var serialized = MemoryMarshal.Cast<char, byte>(value);
                int len = serialized.Length;
                stream.Write(ref len);
                //stream.Write(Encoding.GetBytes(value));
                stream.Write(ref serialized);
            }
        }

        /// <summary>
        /// Binary deserialize
        /// </summary>
        /// <param name="reader">Reader of binary</param>
        public string BinaryDeserialize(ref BinarySpanReader reader)
        {
            var length = BitConverter.ToInt32(reader.Read(sizeof(int)));
            if (length == -1)
                return null;
            return new string(MemoryMarshal.Cast<byte, char>(reader.Read(length)));
        }
    }
}
