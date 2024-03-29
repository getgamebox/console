﻿/*
 * This file is part of the GameBox package , borrowed from CatLib(catlib.io) and modified.
 *
 * (c) MouGuangYi <muguangyi@hotmail.com> , Yu Meng Han <menghanyu1994@gmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: https://catlib.io
 */

using GameBox.Console.Exception;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace GameBox.Console.Util
{
    /// <summary>
    /// The stream extension function.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class StreamExtension
    {
        [ThreadStatic]
        private static byte[] buffer;

        private static byte[] Buffer
        {
            get
            {
                if (buffer == null)
                {
                    buffer = new byte[4096];
                }

                return buffer;
            }
        }

        /// <summary>
        /// Append the source stream to the destination stream.
        /// </summary>
        /// <param name="source">The source stream.</param>
        /// <param name="destination">The destination stream.</param>
        /// <returns>Byte length of transmitted data.</returns>
        public static long AppendTo(this Stream source, Stream destination)
        {
            return source.AppendTo(destination, Buffer);
        }

        /// <summary>
        /// Append the source stream to the destination stream.
        /// </summary>
        /// <param name="source">The source stream.</param>
        /// <param name="destination">The destination stream.</param>
        /// <param name="buffer">The buffer to use.</param>
        /// <returns>Byte length of transmitted data.</returns>
        public static long AppendTo(this Stream source, Stream destination, byte[] buffer)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            Guard.Requires<ArgumentNullException>(destination != null);

            long result = 0;
            int read;
            while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                destination.Write(buffer, 0, read);
                result += read;
            }

            return result;
        }

        /// <summary>
        /// Read the stream's data and convert it to a string.
        /// </summary>
        /// <param name="source">The source stream.</param>
        /// <param name="encoding">The encoding for data.</param>
        /// <param name="closed">Whether auto closed stream.</param>
        /// <returns>The string.</returns>
        public static string ToText(this Stream source, Encoding encoding = null, bool closed = true)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            try
            {
                if (!source.CanRead)
                {
                    throw new ConsoleLogicException($"Can not read stream, {nameof(source.CanRead)} == false");
                }

                encoding = encoding ?? Encoding.Default;
                if (source is MemoryStream memoryStream)
                {
                    byte[] internalBuffer;
                    try
                    {
                        internalBuffer = memoryStream.GetBuffer();
                    }
                    catch (UnauthorizedAccessException)
                    {
                        internalBuffer = memoryStream.ToArray();
                    }

                    return encoding.GetString(internalBuffer, 0, (int)memoryStream.Length);
                }

                var length = 0;
                try
                {
                    length = (int)source.Length;
                }
                catch (NotSupportedException)
                {
                    // ignore
                }

                MemoryStream targetStream;
                if (length > 0 && length <= Buffer.Length)
                {
                    targetStream = new MemoryStream(Buffer, 0, Buffer.Length, true, true);
                }
                else
                {
                    targetStream = new MemoryStream(length);
                }

                using (targetStream)
                {
                    var read = source.AppendTo(targetStream);
                    return encoding.GetString(targetStream.GetBuffer(), 0, (int)read);
                }
            }
            finally
            {
                if (closed)
                {
                    source.Dispose();
                }
            }
        }

        /// <summary>
        /// Convert the specified string to a stream.
        /// </summary>
        /// <param name="str">The specified string.</param>
        /// <param name="encoding">The string encoding.</param>
        /// <returns>The stream instance.</returns>
        public static Stream ToStream(this string str, Encoding encoding = null)
        {
            return new MemoryStream((encoding ?? Encoding.Default).GetBytes(str));
        }
    }
}
