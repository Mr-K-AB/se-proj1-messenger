﻿/******************************************************************************
 * Filename    = SendFileData.cs
 *
 * Author      = Manikanta
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContent
 *
 * Description = 
 *****************************************************************************/

using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace MessengerContent.DataModels
{
    [ExcludeFromCodeCoverage]
    public class SendFileData
    {
        /// <summary>
        /// Data present in the file.
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// Name of the file.
        /// </summary>
        public string Name;

        /// <summary>
        /// Size of the file.
        /// </summary>
        public long Size;

        /// <summary>
        /// Constructor to create type with parameters.
        /// </summary>
        /// <param name="filePath">Path of the file</param>
        /// <exception cref="FileNotFoundException"></exception>
        public SendFileData(string filePath)
        {
            if (File.Exists(filePath))
            {
                // set details of the file
                Data = File.ReadAllBytes(filePath);
                Name = Path.GetFileName(filePath);
                Size = Data.Length;
            }
        }
    }
}
