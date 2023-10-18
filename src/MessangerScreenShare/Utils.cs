///<author>Satyam Mishra</author>
///<summary>
/// This file has Frame and the related structures which are used for storing
/// the difference in the pixel and the resolution of image. It also has some other
/// general utilities.
///</summary>

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MessangerScreenShare
{
    public static class Utils
    {
        /// <summary>
        /// The string representing the module identifier for screen share.
        /// </summary>
        public const string ModuleIdentifier = "ScreenShare";

        /// <summary>
        /// Static method to get a nice debug message wrapped with useful information.
        /// </summary>
        /// <param name="message">
        /// Message to wrap
        /// </param>
        /// <param name="withTimeStamp">
        /// Whether to prefix the wrapped message with time stamp or not
        /// </param>
        /// <returns>
        /// The message wrapped with class and method name and prefixed with time stamp if asked
        /// </returns>
        public static string GetDebugMessage( string message , bool withTimeStamp = false )
        {
            // Get the class name and the name of the caller function
            StackFrame? stackFrame = (new StackTrace()).GetFrame( 1 );
            string className = stackFrame?.GetMethod()?.DeclaringType?.Name ?? "SharedClientScreen";
            string methodName = stackFrame?.GetMethod()?.Name ?? "GetDebugMessage";

            string prefix = withTimeStamp ? $"{DateTimeOffset.Now:F} | " : "";

            return $"{prefix}[{className}::{methodName}] : {message}";
        }
    }
}
