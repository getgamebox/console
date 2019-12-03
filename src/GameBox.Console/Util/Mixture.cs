/*
 * This file is part of the GameBox package.
 *
 * (c) Yu Meng Han <menghanyu1994@gmail.com>  LiuSiJia <394754029@qq.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: https://github.com/getgamebox/console
 */

#pragma warning disable S2933
#pragma warning disable S3877
#pragma warning disable S3875

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace GameBox.Console.Util
{
    /// <summary>
    /// <see cref="Mixture"/> can be implicitly converted to other base types.
    /// </summary>
    [DebuggerDisplay("{Name}:{ToString()}")]
    public sealed class Mixture : IDisposable
    {
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
#pragma warning disable CA1051 // Do not declare visible instance fields
        private struct Value
#pragma warning restore CA1051
        {
            /// <summary>
            /// Save the type of object value by pointer.
            /// </summary>
            [FieldOffset(0)]
            public IntPtr objectValue;

            /// <summary>
            /// The type of int value.
            /// </summary>
            [FieldOffset(0)]
            public int intValue;

            /// <summary>
            /// The type of float value.
            /// </summary>
            [FieldOffset(0)]
            public float floatValue;

            /// <summary>
            /// The type of double value.
            /// </summary>
            [FieldOffset(0)]
            public double doubleValue;

            /// <summary>
            /// The type of char value.
            /// </summary>
            [FieldOffset(0)]
            public char charValue;

            /// <summary>
            /// The type of bool value.
            /// </summary>
            [FieldOffset(0)]
            public bool boolValue;
        }

        /// <summary>
        /// The value is null.
        /// </summary>
        public static readonly Mixture Null = new Mixture();

        /// <summary>
        /// The value name.
        /// </summary>
        public string Name { get; set; } = "UNKNOW";

        /// <summary>
        /// The value of mixture.
        /// </summary>
        private Value mixture;

        /// <summary>
        /// The length of array item.
        /// </summary>
        private int arrayLength;

        /// <summary>
        /// The original object type.
        /// </summary>
        private MixtureTypes type;

        /// <summary>
        /// The original object is null.
        /// </summary>
        public bool IsNull => type == MixtureTypes.None;

        /// <summary>
        /// The original object is array.
        /// </summary>
        public bool IsArray => (MixtureTypes.Array & type) == MixtureTypes.Array;

        /// <summary>
        /// The original object is int.
        /// </summary>
        public bool IsInt => (MixtureTypes.IntValue & type) == MixtureTypes.IntValue;

        /// <summary>
        /// The original object is string.
        /// </summary>
        public bool IsString => (MixtureTypes.StringValue & type) == MixtureTypes.StringValue;

        /// <summary>
        /// The original object is boolean.
        /// </summary>
        public bool IsBoolean => (MixtureTypes.BoolValue & type) == MixtureTypes.BoolValue;

        /// <summary>
        /// The original object is char.
        /// </summary>
        public bool IsChar => (MixtureTypes.CharValue & type) == MixtureTypes.CharValue;

        /// <summary>
        /// The original object is float.
        /// </summary>
        public bool IsFloat => (MixtureTypes.FloatValue & type) == MixtureTypes.FloatValue;

        /// <summary>
        /// The original object is double.
        /// </summary>
        public bool IsDouble => (MixtureTypes.DoubleValue & type) == MixtureTypes.DoubleValue;

        /// <summary>
        /// The resource is disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The mixture length.
        /// </summary>
        public int Length => GetLength();

        /// <summary>
        ///  Create a new <see cref="Mixture"/> Instance.
        /// </summary>
        public Mixture()
        {
            type = MixtureTypes.None;
            arrayLength = 0;
        }

        /// <summary>
        /// Create a new <see cref="Mixture"/> Instance.
        /// </summary>
        /// <param name="value">The value.</param>
        public Mixture(int value)
        {
            type = MixtureTypes.IntValue;
            arrayLength = 0;
            mixture.intValue = value;
        }

        /// <summary>
        /// Create a new <see cref="Mixture"/> Instance.
        /// </summary>
        /// <param name="value">The value.</param>
        public Mixture(bool value)
        {
            type = MixtureTypes.BoolValue;
            arrayLength = 0;
            mixture.boolValue = value;
        }

        /// <summary>
        /// Create a new <see cref="Mixture"/> Instance.
        /// </summary>
        /// <param name="value">The value.</param>
        public Mixture(float value)
        {
            type = MixtureTypes.FloatValue;
            arrayLength = 0;
            mixture.floatValue = value;
        }

        /// <summary>
        /// Create a new <see cref="Mixture"/> Instance.
        /// </summary>
        /// <param name="value">The value.</param>
        public Mixture(double value)
        {
            type = MixtureTypes.DoubleValue;
            arrayLength = 0;
            mixture.doubleValue = value;
        }

        /// <summary>
        /// Create a new <see cref="Mixture"/> Instance.
        /// </summary>
        /// <param name="value">The value.</param>
        public Mixture(char value)
        {
            type = MixtureTypes.CharValue;
            arrayLength = 0;
            mixture.charValue = value;
        }

        /// <summary>
        /// Create a new <see cref="Mixture"/> Instance.
        /// </summary>
        /// <param name="value">The value.</param>
        public Mixture(string value)
        {
            Guard.Requires<ArgumentNullException>(value != null);
            type = MixtureTypes.StringValue;
            mixture.objectValue = Marshal.StringToHGlobalAuto(value);
            arrayLength = 0;
        }

        /// <summary>
        /// Create a new <see cref="Mixture"/> Instance.
        /// </summary>
        /// <param name="value">The value.</param>
        public Mixture(int[] value)
        {
            Guard.Requires<ArgumentNullException>(value != null);
            type = MixtureTypes.Array | MixtureTypes.IntValue;
            arrayLength = value.Length;
            mixture.objectValue = Marshal.AllocHGlobal(sizeof(int) * arrayLength);
            Marshal.Copy(value, 0, mixture.objectValue, arrayLength);
        }

        /// <summary>
        /// Create a new <see cref="Mixture"/> Instance.
        /// </summary>
        /// <param name="value">The value.</param>
        public Mixture(bool[] value)
        {
            Guard.Requires<ArgumentNullException>(value != null);
            type = MixtureTypes.Array | MixtureTypes.BoolValue;
            arrayLength = value.Length;
            var data = new int[arrayLength];
            for (var i = 0; i < arrayLength; ++i)
            {
                data[i] = value[i] ? 1 : 0;
            }

            var size = Marshal.SizeOf(typeof(int)) * arrayLength;
            mixture.objectValue = Marshal.AllocHGlobal(size);
            Marshal.Copy(data, 0, mixture.objectValue, arrayLength);
        }

        /// <summary>
        /// Create a new <see cref="Mixture"/> Instance.
        /// </summary>
        /// <param name="value">The value.</param>
        public Mixture(char[] value)
        {
            Guard.Requires<ArgumentNullException>(value != null);
            type = MixtureTypes.Array | MixtureTypes.CharValue;
            arrayLength = value.Length;
            var size = Marshal.SizeOf(typeof(char)) * arrayLength;
            mixture.objectValue = Marshal.AllocHGlobal(size);
            Marshal.Copy(value, 0, mixture.objectValue, arrayLength);
        }

        /// <summary>
        /// Create a new <see cref="Mixture"/> Instance.
        /// </summary>
        /// <param name="value">The value.</param>
        public Mixture(float[] value)
        {
            Guard.Requires<ArgumentNullException>(value != null);
            type = MixtureTypes.Array | MixtureTypes.FloatValue;
            arrayLength = value.Length;
            var size = Marshal.SizeOf(typeof(float)) * arrayLength;
            mixture.objectValue = Marshal.AllocHGlobal(size);
            Marshal.Copy(value, 0, mixture.objectValue, arrayLength);
        }

        /// <summary>
        /// Create a new <see cref="Mixture"/> Instance.
        /// </summary>
        /// <param name="value">The value.</param>
        public Mixture(double[] value)
        {
            Guard.Requires<ArgumentNullException>(value != null);
            type = MixtureTypes.Array | MixtureTypes.DoubleValue;
            arrayLength = value.Length;
            var size = Marshal.SizeOf(typeof(double)) * arrayLength;
            mixture.objectValue = Marshal.AllocHGlobal(size);
            Marshal.Copy(value, 0, mixture.objectValue, arrayLength);
        }

        /// <summary>
        /// Create a new <see cref="Mixture"/> Instance.
        /// </summary>
        /// <param name="value">The value.</param>
        public Mixture(string[] value)
        {
            Guard.Requires<ArgumentNullException>(value != null);
            type = MixtureTypes.Array | MixtureTypes.StringValue;
            arrayLength = value.Length;
            IntPtr[] pointers = new IntPtr[arrayLength];
            for (var i = 0; i < arrayLength; ++i)
            {
                pointers[i] = Marshal.StringToHGlobalAuto(value[i]);
            }

            var size = Marshal.SizeOf(typeof(IntPtr)) * arrayLength;
            mixture.objectValue = Marshal.AllocHGlobal(size);
            Marshal.Copy(pointers, 0, mixture.objectValue, arrayLength);
        }

        ~Mixture()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                //release managed resources
            }

            if ((IsArray || IsString) && mixture.objectValue != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(mixture.objectValue);
                mixture.objectValue = IntPtr.Zero;
            }

            disposed = true;
        }

        /// <summary>
        /// Get the length of mixture.
        /// If value is array type then return the length of array; otherwise return then value convert to string's length.
        /// </summary>
        /// <returns>The length of mixture.</returns>
        private int GetLength()
        {
            if (IsArray)
            {
                return arrayLength;
            }

            if (IsInt)
            {
                return Convert.ToString(mixture.intValue).Length;
            }

            if (IsFloat)
            {
                return Convert.ToString(mixture.floatValue, CultureInfo.InvariantCulture).Length;
            }

            if (IsDouble)
            {
                return Convert.ToString(mixture.doubleValue, CultureInfo.InvariantCulture).Length;
            }

            if (IsBoolean)
            {
                return mixture.boolValue ? 4 : 5;
            }

            if (IsChar)
            {
                return 1;
            }

            if (IsString)
            {
                var s = Marshal.PtrToStringAuto(mixture.objectValue);
                if (!string.IsNullOrEmpty(s))
                {
                    return s.Length;
                }
            }

            return 0;
        }

        /// <summary>
        /// Convert <see cref="string"/> to <see cref="Mixture"/>.
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator Mixture(string content)
        {
            return content == null ? null : new Mixture(content);
        }

        /// <summary>
        /// Convert <see cref="Mixture"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator string(Mixture content)
        {
            if (content is null)
            {
                return null;
            }

            if (content.IsArray)
            {
                // todo: format array
                return null;
            }

            if (content.IsBoolean)
            {
                return Convert.ToString(content.mixture.boolValue);
            }

            if (content.IsInt)
            {
                return Convert.ToString(content.mixture.intValue);
            }

            if (content.IsFloat)
            {
                return Convert.ToString(content.mixture.floatValue, CultureInfo.InvariantCulture);
            }

            if (content.IsDouble)
            {
                return Convert.ToString(content.mixture.doubleValue, CultureInfo.InvariantCulture);
            }

            if (content.IsChar)
            {
                return Convert.ToString(content.mixture.charValue);
            }

            if (content.IsString)
            {
                return Marshal.PtrToStringAuto(content.mixture.objectValue);
            }

            return null;
        }

        /// <summary>
        /// Convert <see cref="int"/> to <see cref="Mixture"/>.
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator Mixture(int content)
        {
            return new Mixture(content);
        }

        /// <summary>
        /// Convert <see cref="Mixture"/> to <see cref="int"/>.
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator int(Mixture content)
        {
            if (content is null)
            {
                return 0;
            }

            if (content.IsArray)
            {
                throw new InvalidCastException("This mixture is array type and can not convert to the type of int!");
            }

            if (content.IsBoolean)
            {
                return Convert.ToInt32(content.mixture.boolValue);
            }

            if (content.IsInt)
            {
                return content.mixture.intValue;
            }

            if (content.IsFloat)
            {
                return Convert.ToInt32(content.mixture.floatValue);
            }

            if (content.IsDouble)
            {
                return Convert.ToInt32(content.mixture.doubleValue);
            }

            if (content.IsChar)
            {
                return Convert.ToInt32(content.mixture.charValue);
            }

            if (content.IsString)
            {
                return Convert.ToInt32(Marshal.PtrToStringAuto(content.mixture.objectValue));
            }

            return 0;
        }

        /// <summary>
        /// Convert <see cref="bool"/> to <see cref="Mixture"/>.
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator Mixture(bool content)
        {
            return new Mixture(content);
        }

        /// <summary>
        /// Convert <see cref="Mixture"/> to <see cref="bool"/>.
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator bool(Mixture content)
        {
            if (content is null)
            {
                return false;
            }

            if (content.IsArray)
            {
                throw new InvalidCastException("This mixture is array type and can not convert to the type of bool!");
            }

            if (content.IsBoolean)
            {
                return content.mixture.boolValue;
            }

            if (content.IsInt)
            {
                return Convert.ToBoolean(content.mixture.intValue);
            }

            if (content.IsFloat)
            {
                return Convert.ToBoolean(content.mixture.floatValue);
            }

            if (content.IsDouble)
            {
                return Convert.ToBoolean(content.mixture.doubleValue);
            }

            if (content.IsChar)
            {
                return Convert.ToBoolean(content.mixture.charValue);
            }

            if (content.IsString)
            {
                return Convert.ToBoolean(Marshal.PtrToStringAuto(content.mixture.objectValue));
            }

            return false;
        }

        /// <summary>
        /// Convert <see cref="float"/> to <see cref="Mixture"/>.
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator Mixture(float content)
        {
            return new Mixture(content);
        }

        /// <summary>
        /// Convert <see cref="Mixture"/> to <see cref="float"/>.
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator float(Mixture content)
        {
            if (content is null)
            {
                return 0.0f;
            }

            if (content.IsArray)
            {
                throw new InvalidCastException("This mixture is array type and can not convert to the type of float!");
            }

            if (content.IsBoolean)
            {
                return Convert.ToSingle(content.mixture.boolValue);
            }

            if (content.IsInt)
            {
                return Convert.ToSingle(content.mixture.intValue);
            }

            if (content.IsFloat)
            {
                return content.mixture.floatValue;
            }

            if (content.IsDouble)
            {
                return Convert.ToSingle(content.mixture.doubleValue);
            }

            if (content.IsChar)
            {
                return Convert.ToSingle(content.mixture.charValue);
            }

            if (content.IsString)
            {
                return Convert.ToSingle(Marshal.PtrToStringAuto(content.mixture.objectValue));
            }

            return 0.0f;
        }

        /// <summary>
        /// Convert <see cref="double"/> to <see cref="Mixture"/>.
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator Mixture(double content)
        {
            return new Mixture(content);
        }

        /// <summary>
        /// Convert <see cref="Mixture"/> to <see cref="double"/>.
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator double(Mixture content)
        {
            if (content is null)
            {
                return 0.0d;
            }

            if (content.IsArray)
            {
                throw new InvalidCastException("This mixture is array type and can not convert to the type of double!");
            }

            if (content.IsBoolean)
            {
                return Convert.ToDouble(content.mixture.boolValue);
            }

            if (content.IsInt)
            {
                return Convert.ToDouble(content.mixture.intValue);
            }

            if (content.IsFloat)
            {
                return Convert.ToDouble(content.mixture.floatValue);
            }

            if (content.IsDouble)
            {
                return content.mixture.doubleValue;
            }

            if (content.IsChar)
            {
                return Convert.ToDouble(content.mixture.charValue);
            }

            if (content.IsString)
            {
                return Convert.ToDouble(Marshal.PtrToStringAuto(content.mixture.objectValue));
            }

            return 0.0d;
        }

        /// <summary>
        /// Convert <see cref="char"/> to <see cref="Mixture"/>.
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator Mixture(char content)
        {
            return new Mixture(content);
        }

        /// <summary>
        /// Convert <see cref="Mixture"/> to <see cref="char"/>.
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator char(Mixture content)
        {
            if (content is null)
            {
                return char.MinValue;
            }

            if (content.IsArray)
            {
                throw new InvalidCastException("This mixture is array type and can not convert to the type of char!");
            }

            if (content.IsBoolean)
            {
                return Convert.ToChar(content.mixture.boolValue);
            }

            if (content.IsInt)
            {
                return Convert.ToChar(content.mixture.intValue);
            }

            if (content.IsFloat)
            {
                return Convert.ToChar(content.mixture.floatValue);
            }

            if (content.IsDouble)
            {
                return Convert.ToChar(content.mixture.doubleValue);
            }

            if (content.IsChar)
            {
                return content.mixture.charValue;
            }

            if (content.IsString)
            {
                return Convert.ToChar(Marshal.PtrToStringAuto(content.mixture.objectValue));
            }

            return char.MinValue;
        }

        /// <summary>
        /// Convert int[] to <see cref="Mixture"/>.
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator Mixture(int[] content)
        {
            return content == null ? null : new Mixture(content);
        }

        /// <summary>
        /// Convert <see cref="Mixture"/> to int[].
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator int[](Mixture content)
        {
            if (content is null)
            {
                return null;
            }

            if (content.IsArray)
            {
                if (!content.IsInt)
                {
                    throw new InvalidCastException("Can not convert to the type of int[] because if the element is not int type.");
                }

                var result = new int[content.arrayLength];
                Marshal.Copy(content.mixture.objectValue, result, 0, content.arrayLength);
                return result;
            }

            if (content.IsBoolean)
            {
                return new[] { Convert.ToInt32(content.mixture.boolValue) };
            }

            if (content.IsInt)
            {
                return new[] { content.mixture.intValue };
            }

            if (content.IsFloat)
            {
                return new[] { Convert.ToInt32(content.mixture.floatValue) };
            }

            if (content.IsDouble)
            {
                return new[] { Convert.ToInt32(content.mixture.doubleValue) };
            }

            if (content.IsChar)
            {
                return new[] { Convert.ToInt32(content.mixture.charValue) };
            }

            if (content.IsString)
            {
                return new[] { Convert.ToInt32(Marshal.PtrToStringAuto(content.mixture.objectValue)) };
            }

            return null;
        }

        /// <summary>
        /// Convert float[] to <see cref="Mixture"/>.
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator Mixture(float[] content)
        {
            return content == null ? null : new Mixture(content);
        }

        /// <summary>
        /// Convert <see cref="Mixture"/> to float[].
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator float[](Mixture content)
        {
            if (content is null)
            {
                return null;
            }

            if (content.IsArray)
            {
                if (!content.IsFloat)
                {
                    throw new InvalidCastException("Can not convert to the type of float[] because if the element is not float type.");
                }

                var result = new float[content.arrayLength];
                Marshal.Copy(content.mixture.objectValue, result, 0, content.arrayLength);
                return result;
            }

            if (content.IsBoolean)
            {
                return new[] { Convert.ToSingle(content.mixture.boolValue) };
            }

            if (content.IsInt)
            {
                return new[] { Convert.ToSingle(content.mixture.intValue) };
            }

            if (content.IsFloat)
            {
                return new[] { content.mixture.floatValue };
            }

            if (content.IsDouble)
            {
                return new[] { Convert.ToSingle(content.mixture.doubleValue) };
            }

            if (content.IsChar)
            {
                return new[] { Convert.ToSingle(content.mixture.charValue) };
            }

            if (content.IsString)
            {
                return new[] { Convert.ToSingle(Marshal.PtrToStringAuto(content.mixture.objectValue)) };
            }

            return null;
        }

        /// <summary>
        /// Convert double[] to <see cref="Mixture"/>.
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator Mixture(double[] content)
        {
            return content == null ? null : new Mixture(content);
        }

        /// <summary>
        /// Convert <see cref="Mixture"/> to double[].
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator double[](Mixture content)
        {
            if (content is null)
            {
                return null;
            }

            if (content.IsArray)
            {
                if (!content.IsDouble)
                {
                    throw new InvalidCastException("Can not convert to the type of double[] because if the element is not double type.");
                }

                var result = new double[content.arrayLength];
                Marshal.Copy(content.mixture.objectValue, result, 0, content.arrayLength);
                return result;
            }

            if (content.IsBoolean)
            {
                return new[] { Convert.ToDouble(content.mixture.boolValue) };
            }

            if (content.IsInt)
            {
                return new[] { Convert.ToDouble(content.mixture.intValue) };
            }

            if (content.IsFloat)
            {
                return new[] { Convert.ToDouble(content.mixture.floatValue) };
            }

            if (content.IsDouble)
            {
                return new[] { content.mixture.doubleValue };
            }

            if (content.IsChar)
            {
                return new[] { Convert.ToDouble(content.mixture.charValue) };
            }

            if (content.IsString)
            {
                return new[] { Convert.ToDouble(Marshal.PtrToStringAuto(content.mixture.objectValue)) };
            }

            return null;
        }

        /// <summary>
        /// Convert char[] to <see cref="Mixture"/>.
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator Mixture(char[] content)
        {
            return content == null ? null : new Mixture(content);
        }

        /// <summary>
        /// Convert <see cref="Mixture"/> to char[].
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator char[](Mixture content)
        {
            if (content is null)
            {
                return null;
            }

            if (content.IsArray)
            {
                if (!content.IsChar)
                {
                    throw new InvalidCastException("Can not convert to the type of char[] because if the element is not char type.");
                }

                var result = new char[content.arrayLength];
                Marshal.Copy(content.mixture.objectValue, result, 0, content.arrayLength);
                return result;
            }

            if (content.IsBoolean)
            {
                return new[] { Convert.ToChar(content.mixture.boolValue) };
            }

            if (content.IsInt)
            {
                return new[] { Convert.ToChar(content.mixture.intValue) };
            }

            if (content.IsFloat)
            {
                return new[] { Convert.ToChar(content.mixture.floatValue) };
            }

            if (content.IsDouble)
            {
                return new[] { Convert.ToChar(content.mixture.doubleValue) };
            }

            if (content.IsChar)
            {
                return new[] { content.mixture.charValue };
            }

            if (content.IsString)
            {
                return new[] { Convert.ToChar(Marshal.PtrToStringAuto(content.mixture.objectValue)) };
            }

            return null;
        }

        /// <summary>
        /// Convert bool[] to <see cref="Mixture"/>.
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator Mixture(bool[] content)
        {
            return content == null ? null : new Mixture(content);
        }

        /// <summary>
        /// Convert <see cref="Mixture"/> to bool[].
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator bool[](Mixture content)
        {
            if (content is null)
            {
                return null;
            }

            if (content.IsArray)
            {
                if (!content.IsBoolean)
                {
                    throw new InvalidCastException("Can not convert to the type of bool[] because if the element is not bool type.");
                }

                var tempArr = new int[content.arrayLength];
                var result = new bool[content.arrayLength];
                Marshal.Copy(content.mixture.objectValue, tempArr, 0, content.arrayLength);
                for (var i = 0; i < content.arrayLength; ++i)
                {
                    result[i] = tempArr[i] > 0;
                }

                return result;
            }

            if (content.IsBoolean)
            {
                return new[] { content.mixture.boolValue };
            }

            if (content.IsInt)
            {
                return new[] { Convert.ToBoolean(content.mixture.intValue) };
            }

            if (content.IsFloat)
            {
                return new[] { Convert.ToBoolean(content.mixture.floatValue) };
            }

            if (content.IsDouble)
            {
                return new[] { Convert.ToBoolean(content.mixture.doubleValue) };
            }

            if (content.IsChar)
            {
                return new[] { Convert.ToBoolean(content.mixture.charValue) };
            }

            if (content.IsString)
            {
                return new[] { Convert.ToBoolean(Marshal.PtrToStringAuto(content.mixture.objectValue)) };
            }

            return null;
        }

        /// <summary>
        /// Convert string[] to <see cref="Mixture"/>.
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator Mixture(string[] content)
        {
            return content == null ? null : new Mixture(content);
        }

        /// <summary>
        /// Convert <see cref="Mixture"/> to string[].
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator string[](Mixture content)
        {
            if (content is null)
            {
                return null;
            }

            if (content.IsArray)
            {
                if (!content.IsString)
                {
                    throw new InvalidCastException("Can not convert to the type of string[] because if the element is not string type.");
                }

                var pointers = new IntPtr[content.arrayLength];
                var result = new string[content.arrayLength];
                Marshal.Copy(content.mixture.objectValue, pointers, 0, content.arrayLength);
                for (var i = 0; i < content.arrayLength; ++i)
                {
                    result[i] = Marshal.PtrToStringAuto(pointers[i]);
                }

                return result;
            }

            if (content.IsBoolean)
            {
                return new[] { Convert.ToString(content.mixture.boolValue) };
            }

            if (content.IsInt)
            {
                return new[] { Convert.ToString(content.mixture.intValue) };
            }

            if (content.IsFloat)
            {
                return new[] { Convert.ToString(content.mixture.floatValue, CultureInfo.InvariantCulture) };
            }

            if (content.IsDouble)
            {
                return new[] { Convert.ToString(content.mixture.doubleValue, CultureInfo.InvariantCulture) };
            }

            if (content.IsChar)
            {
                return new[] { Convert.ToString(content.mixture.charValue) };
            }

            if (content.IsString)
            {
                return new[] { Marshal.PtrToStringAuto(content.mixture.objectValue) };
            }

            return null;
        }

        public static bool operator ==(Mixture left, Mixture right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Mixture left, Mixture right)
        {
            return !(left == right);
        }

        public static bool Equals(Mixture left, Mixture right)
        {
            // Casting is a must, it prevents an infinite loop
            if ((object)left == (object)right)
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            if (left.IsNull || right.IsNull)
            {
                return false;
            }

            if (left.type != right.type)
            {
                return false;
            }

            if (left.IsArray)
            {
                if (left.arrayLength != right.arrayLength)
                {
                    return false;
                }

                if (left.IsInt)
                {
                    var leftArr = new int[left.arrayLength];
                    var rightArr = new int[left.arrayLength];
                    Marshal.Copy(left.mixture.objectValue, leftArr, 0, left.arrayLength);
                    Marshal.Copy(right.mixture.objectValue, rightArr, 0, left.arrayLength);

                    for (var i = 0; i < left.arrayLength; ++i)
                    {
                        if (leftArr[i] != rightArr[i])
                        {
                            return false;
                        }
                    }

                    return true;
                }

                if (left.IsFloat)
                {
                    var leftArr = new float[left.arrayLength];
                    var rightArr = new float[left.arrayLength];
                    Marshal.Copy(left.mixture.objectValue, leftArr, 0, left.arrayLength);
                    Marshal.Copy(right.mixture.objectValue, rightArr, 0, left.arrayLength);

                    for (var i = 0; i < left.arrayLength; ++i)
                    {
                        if (Math.Abs(leftArr[i] - rightArr[i]) > float.Epsilon)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                if (left.IsDouble)
                {
                    var leftArr = new double[left.arrayLength];
                    var rightArr = new double[left.arrayLength];
                    Marshal.Copy(left.mixture.objectValue, leftArr, 0, left.arrayLength);
                    Marshal.Copy(right.mixture.objectValue, rightArr, 0, left.arrayLength);

                    for (var i = 0; i < left.arrayLength; ++i)
                    {
                        if (Math.Abs(leftArr[i] - rightArr[i]) > double.Epsilon)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                if (left.IsChar)
                {
                    var leftArr = new char[left.arrayLength];
                    var rightArr = new char[left.arrayLength];
                    Marshal.Copy(left.mixture.objectValue, leftArr, 0, left.arrayLength);
                    Marshal.Copy(right.mixture.objectValue, rightArr, 0, left.arrayLength);

                    for (var i = 0; i < left.arrayLength; ++i)
                    {
                        if (leftArr[i] != rightArr[i])
                        {
                            return false;
                        }
                    }

                    return true;
                }

                if (left.IsBoolean)
                {
                    var leftArr = new int[left.arrayLength];
                    var rightArr = new int[left.arrayLength];
                    Marshal.Copy(left.mixture.objectValue, leftArr, 0, left.arrayLength);
                    Marshal.Copy(right.mixture.objectValue, rightArr, 0, left.arrayLength);

                    for (var i = 0; i < left.arrayLength; ++i)
                    {
                        if (leftArr[i] != rightArr[i])
                        {
                            return false;
                        }
                    }

                    return true;
                }

                if (left.IsString)
                {
                    var leftPointers = new IntPtr[left.arrayLength];
                    var rightPointers = new IntPtr[left.arrayLength];
                    Marshal.Copy(left.mixture.objectValue, leftPointers, 0, left.arrayLength);
                    Marshal.Copy(right.mixture.objectValue, rightPointers, 0, left.arrayLength);

                    for (var i = 0; i < left.arrayLength; ++i)
                    {
                        var strLeft = Marshal.PtrToStringAuto(leftPointers[i]);
                        var strright = Marshal.PtrToStringAuto(rightPointers[i]);
                        if (string.Compare(strLeft, strright, StringComparison.Ordinal) != 0)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            else
            {
                if (left.IsInt)
                {
                    return left.mixture.intValue == right.mixture.intValue;
                }

                if (left.IsFloat)
                {
                    return Math.Abs(left.mixture.floatValue - right.mixture.floatValue) < float.Epsilon;
                }

                if (left.IsDouble)
                {
                    return Math.Abs(left.mixture.doubleValue - right.mixture.doubleValue) < double.Epsilon;
                }

                if (left.IsChar)
                {
                    return left.mixture.charValue == right.mixture.charValue;
                }

                if (left.IsBoolean)
                {
                    return left.mixture.boolValue == right.mixture.boolValue;
                }

                if (left.IsString)
                {
                    return string.Compare(Marshal.PtrToStringAuto(left.mixture.objectValue), Marshal.PtrToStringAuto(right.mixture.objectValue), StringComparison.Ordinal) == 0;
                }
            }

            return false;
        }

        #region compare to int

        public static bool operator ==(Mixture left, int right)
        {
            if (left is null)
            {
                return false;
            }

            if (left.IsNull)
            {
                return false;
            }

            if (left.IsArray)
            {
                return false;
            }

            if (left.IsInt)
            {
                return left.mixture.intValue == right;
            }

            if (left.IsFloat)
            {
                return Math.Abs(left.mixture.floatValue - right) < float.Epsilon;
            }

            if (left.IsDouble)
            {
                return Math.Abs(left.mixture.doubleValue - right) < double.Epsilon;
            }

            if (left.IsChar)
            {
                return left.mixture.charValue == right;
            }

            return false;
        }

        public static bool operator !=(Mixture left, int right)
        {
            return !(left == right);
        }

        public static bool operator ==(int left, Mixture right)
        {
            return right == left;
        }

        public static bool operator !=(int left, Mixture right)
        {
            return !(right == left);
        }

        #endregion

        #region compare to float

        public static bool operator ==(Mixture left, float right)
        {
            if (left is null)
            {
                return false;
            }

            if (left.IsNull)
            {
                return false;
            }

            if (left.IsArray)
            {
                return false;
            }

            if (left.IsInt)
            {
                return Math.Abs(left.mixture.intValue - right) < float.Epsilon;
            }

            if (left.IsFloat)
            {
                return Math.Abs(left.mixture.floatValue - right) < float.Epsilon;
            }

            if (left.IsDouble)
            {
                return Math.Abs(left.mixture.doubleValue - right) < double.Epsilon;
            }

            if (left.IsChar)
            {
                return Math.Abs(left.mixture.charValue - right) < float.Epsilon;
            }

            return false;
        }

        public static bool operator !=(Mixture left, float right)
        {
            return !(left == right);
        }

        public static bool operator ==(float left, Mixture right)
        {
            return right == left;
        }

        public static bool operator !=(float left, Mixture right)
        {
            return !(right == left);
        }

        #endregion

        #region compare to double

        public static bool operator ==(Mixture left, double right)
        {
            if (left is null)
            {
                return false;
            }

            if (left.IsNull)
            {
                return false;
            }

            if (left.IsArray)
            {
                return false;
            }

            if (left.IsInt)
            {
                return Math.Abs(left.mixture.intValue - right) < double.Epsilon;
            }

            if (left.IsFloat)
            {
                return Math.Abs(left.mixture.floatValue - right) < double.Epsilon;
            }

            if (left.IsDouble)
            {
                return Math.Abs(left.mixture.doubleValue - right) < double.Epsilon;
            }

            if (left.IsChar)
            {
                return Math.Abs(left.mixture.charValue - right) < double.Epsilon;
            }

            return false;
        }

        public static bool operator !=(Mixture left, double right)
        {
            return !(left == right);
        }

        public static bool operator ==(double left, Mixture right)
        {
            return right == left;
        }

        public static bool operator !=(double left, Mixture right)
        {
            return !(right == left);
        }

        #endregion

        #region compare to char

        public static bool operator ==(Mixture left, char right)
        {
            if (left is null)
            {
                return false;
            }

            if (left.IsNull)
            {
                return false;
            }

            if (left.IsArray)
            {
                return false;
            }

            if (left.IsInt)
            {
                return left.mixture.intValue == right;
            }

            if (left.IsFloat)
            {
                return Math.Abs(left.mixture.floatValue - right) < float.Epsilon;
            }

            if (left.IsDouble)
            {
                return Math.Abs(left.mixture.doubleValue - right) < double.Epsilon;
            }

            if (left.IsChar)
            {
                return left.mixture.charValue == right;
            }

            return false;
        }

        public static bool operator !=(Mixture left, char right)
        {
            return !(left == right);
        }

        public static bool operator ==(char left, Mixture right)
        {
            return right == left;
        }

        public static bool operator !=(char left, Mixture right)
        {
            return !(right == left);
        }

        #endregion

        #region compare to bool

        public static bool operator ==(Mixture left, bool right)
        {
            if (left is null)
            {
                return false;
            }

            if (left.IsNull)
            {
                return false;
            }

            if (left.IsArray)
            {
                return false;
            }

            if (left.IsBoolean)
            {
                return left.mixture.boolValue == right;
            }

            return false;
        }

        public static bool operator !=(Mixture left, bool right)
        {
            return !(left == right);
        }

        public static bool operator ==(bool left, Mixture right)
        {
            return right == left;
        }

        public static bool operator !=(bool left, Mixture right)
        {
            return !(right == left);
        }

        #endregion

        #region compare to string

        public static bool operator ==(Mixture left, string right)
        {
            if (left is null)
            {
                return false;
            }

            if (left.IsNull)
            {
                return false;
            }

            if (left.IsArray)
            {
                return false;
            }

            if (string.IsNullOrEmpty(right))
            {
                return false;
            }

            if (left.IsString)
            {
                return string.Compare(Marshal.PtrToStringAuto(left.mixture.objectValue), right, StringComparison.Ordinal) == 0;
            }

            return false;
        }

        public static bool operator !=(Mixture left, string right)
        {
            return !(right == left);
        }

        public static bool operator ==(string left, Mixture right)
        {
            return right == left;
        }

        public static bool operator !=(string left, Mixture right)
        {
            return !(right == left);
        }

        #endregion

        #region compare to int array

        public static bool operator ==(Mixture left, int[] right)
        {
            if (left is null)
            {
                return false;
            }

            if (left.IsNull)
            {
                return false;
            }

            if (!left.IsArray)
            {
                return false;
            }

            if (right == null)
            {
                return false;
            }

            if (left.IsInt)
            {
                if (left.arrayLength != right.Length)
                {
                    return false;
                }

                var leftArr = new int[left.arrayLength];
                Marshal.Copy(left.mixture.objectValue, leftArr, 0, left.arrayLength);

                for (var i = 0; i < left.arrayLength; ++i)
                {
                    if (leftArr[i] != right[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public static bool operator !=(Mixture left, int[] right)
        {
            return !(right == left);
        }

        public static bool operator ==(int[] left, Mixture right)
        {
            return right == left;
        }

        public static bool operator !=(int[] left, Mixture right)
        {
            return !(right == left);
        }

        #endregion

        #region compare to float array

        public static bool operator ==(Mixture left, float[] right)
        {
            if (left is null)
            {
                return false;
            }

            if (left.IsNull)
            {
                return false;
            }

            if (!left.IsArray)
            {
                return false;
            }

            if (right == null)
            {
                return false;
            }

            if (left.IsFloat)
            {
                if (left.arrayLength != right.Length)
                {
                    return false;
                }

                var leftArr = new float[left.arrayLength];
                Marshal.Copy(left.mixture.objectValue, leftArr, 0, left.arrayLength);

                for (var i = 0; i < left.arrayLength; ++i)
                {
                    if (Math.Abs(leftArr[i] - right[i]) > float.Epsilon)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public static bool operator !=(Mixture left, float[] right)
        {
            return !(right == left);
        }

        public static bool operator ==(float[] left, Mixture right)
        {
            return right == left;
        }

        public static bool operator !=(float[] left, Mixture right)
        {
            return !(right == left);
        }

        #endregion

        #region compare to double array

        public static bool operator ==(Mixture left, double[] right)
        {
            if (left is null)
            {
                return false;
            }

            if (left.IsNull)
            {
                return false;
            }

            if (!left.IsArray)
            {
                return false;
            }

            if (right == null)
            {
                return false;
            }

            if (left.IsDouble)
            {
                if (left.arrayLength != right.Length)
                {
                    return false;
                }

                var leftArr = new double[left.arrayLength];
                Marshal.Copy(left.mixture.objectValue, leftArr, 0, left.arrayLength);

                for (var i = 0; i < left.arrayLength; ++i)
                {
                    if (Math.Abs(leftArr[i] - right[i]) > double.Epsilon)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public static bool operator !=(Mixture left, double[] right)
        {
            return !(right == left);
        }

        public static bool operator ==(double[] left, Mixture right)
        {
            return right == left;
        }

        public static bool operator !=(double[] left, Mixture right)
        {
            return !(right == left);
        }

        #endregion

        #region compare to char array

        public static bool operator ==(Mixture left, char[] right)
        {
            if (left is null)
            {
                return false;
            }

            if (left.IsNull)
            {
                return false;
            }

            if (!left.IsArray)
            {
                return false;
            }

            if (right == null)
            {
                return false;
            }

            if (left.IsChar)
            {
                if (left.arrayLength != right.Length)
                {
                    return false;
                }

                var leftArr = new char[left.arrayLength];
                Marshal.Copy(left.mixture.objectValue, leftArr, 0, left.arrayLength);

                for (var i = 0; i < left.arrayLength; ++i)
                {
                    if (leftArr[i] != right[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public static bool operator !=(Mixture left, char[] right)
        {
            return !(right == left);
        }

        public static bool operator ==(char[] left, Mixture right)
        {
            return right == left;
        }

        public static bool operator !=(char[] left, Mixture right)
        {
            return !(right == left);
        }

        #endregion

        #region compare to bool array

        public static bool operator ==(Mixture left, bool[] right)
        {
            if (left is null)
            {
                return false;
            }

            if (left.IsNull)
            {
                return false;
            }

            if (!left.IsArray)
            {
                return false;
            }

            if (right == null)
            {
                return false;
            }

            if (left.IsBoolean)
            {
                if (left.arrayLength != right.Length)
                {
                    return false;
                }

                var leftArr = new int[left.arrayLength];
                Marshal.Copy(left.mixture.objectValue, leftArr, 0, left.arrayLength);

                for (var i = 0; i < left.arrayLength; ++i)
                {
                    if (leftArr[i] > 0 != right[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public static bool operator !=(Mixture left, bool[] right)
        {
            return !(right == left);
        }

        public static bool operator ==(bool[] left, Mixture right)
        {
            return right == left;
        }

        public static bool operator !=(bool[] left, Mixture right)
        {
            return !(right == left);
        }

        #endregion

        #region compare to string array

        public static bool operator ==(Mixture left, string[] right)
        {
            if (left is null)
            {
                return false;
            }

            if (left.IsNull)
            {
                return false;
            }

            if (!left.IsArray)
            {
                return false;
            }

            if (right == null)
            {
                return false;
            }

            if (left.IsString)
            {
                if (left.arrayLength != right.Length)
                {
                    return false;
                }

                var leftPointers = new IntPtr[left.arrayLength];
                Marshal.Copy(left.mixture.objectValue, leftPointers, 0, left.arrayLength);

                for (var i = 0; i < left.arrayLength; ++i)
                {
                    if (string.Compare(Marshal.PtrToStringAuto(leftPointers[i]), right[i], StringComparison.Ordinal) != 0)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public static bool operator !=(Mixture left, string[] right)
        {
            return !(right == left);
        }

        public static bool operator ==(string[] left, Mixture right)
        {
            return right == left;
        }

        public static bool operator !=(string[] left, Mixture right)
        {
            return !(right == left);
        }

        #endregion

        public override string ToString()
        {
            return this;
        }

        public override bool Equals(object obj)
        {
            return this == (obj as Mixture);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
