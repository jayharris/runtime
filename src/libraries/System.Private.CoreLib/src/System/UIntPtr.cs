// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Versioning;

#pragma warning disable SA1121 // explicitly using type aliases instead of built-in types
#pragma warning disable CA1066 // Implement IEquatable when overriding Object.Equals

#if TARGET_64BIT
using nuint_t = System.UInt64;
#else
using nuint_t = System.UInt32;
#endif

namespace System
{
    [Serializable]
    [CLSCompliant(false)]
    [StructLayout(LayoutKind.Sequential)]
    [System.Runtime.CompilerServices.TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public readonly struct UIntPtr : IEquatable<nuint>, IComparable, IComparable<nuint>, ISpanFormattable, ISerializable
#if FEATURE_GENERIC_MATH
#pragma warning disable SA1001, CA2252 // SA1001: Comma positioning; CA2252: Preview Features
        , IBinaryInteger<nuint>,
          IMinMaxValue<nuint>,
          IUnsignedNumber<nuint>
#pragma warning restore SA1001, CA2252
#endif // FEATURE_GENERIC_MATH
    {
        private readonly unsafe void* _value; // Do not rename (binary serialization)

        [Intrinsic]
        public static readonly UIntPtr Zero;

        [NonVersionable]
        public unsafe UIntPtr(uint value)
        {
            _value = (void*)value;
        }

        [NonVersionable]
        public unsafe UIntPtr(ulong value)
        {
#if TARGET_64BIT
            _value = (void*)value;
#else
            _value = (void*)checked((uint)value);
#endif
        }

        [NonVersionable]
        public unsafe UIntPtr(void* value)
        {
            _value = value;
        }

        private unsafe UIntPtr(SerializationInfo info, StreamingContext context)
        {
            ulong l = info.GetUInt64("value");

            if (Size == 4 && l > uint.MaxValue)
                throw new ArgumentException(SR.Serialization_InvalidPtrValue);

            _value = (void*)l;
        }

        void ISerializable.GetObjectData(SerializationInfo info!!, StreamingContext context)
        {
            info.AddValue("value", ToUInt64());
        }

        public override unsafe bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is UIntPtr)
            {
                return _value == ((UIntPtr)obj)._value;
            }
            return false;
        }

        public override unsafe int GetHashCode()
        {
#if TARGET_64BIT
            ulong l = (ulong)_value;
            return unchecked((int)l) ^ (int)(l >> 32);
#else
            return unchecked((int)_value);
#endif
        }

        [NonVersionable]
        public unsafe uint ToUInt32()
        {
#if TARGET_64BIT
            return checked((uint)_value);
#else
            return (uint)_value;
#endif
        }

        [NonVersionable]
        public unsafe ulong ToUInt64() => (ulong)_value;

        [NonVersionable]
        public static explicit operator UIntPtr(uint value) =>
            new UIntPtr(value);

        [NonVersionable]
        public static explicit operator UIntPtr(ulong value) =>
            new UIntPtr(value);

        [NonVersionable]
        public static unsafe explicit operator UIntPtr(void* value) =>
            new UIntPtr(value);

        [NonVersionable]
        public static unsafe explicit operator void*(UIntPtr value) =>
            value._value;

        [NonVersionable]
        public static unsafe explicit operator uint(UIntPtr value) =>
#if TARGET_64BIT
            checked((uint)value._value);
#else
            (uint)value._value;
#endif

        [NonVersionable]
        public static unsafe explicit operator ulong(UIntPtr value) =>
            (ulong)value._value;

        [NonVersionable]
        public static unsafe bool operator ==(UIntPtr value1, UIntPtr value2) =>
            value1._value == value2._value;

        [NonVersionable]
        public static unsafe bool operator !=(UIntPtr value1, UIntPtr value2) =>
            value1._value != value2._value;

        [NonVersionable]
        public static UIntPtr Add(UIntPtr pointer, int offset) =>
            pointer + offset;

        [NonVersionable]
        public static unsafe UIntPtr operator +(UIntPtr pointer, int offset) =>
            (nuint)pointer._value + (nuint)offset;

        [NonVersionable]
        public static UIntPtr Subtract(UIntPtr pointer, int offset) =>
            pointer - offset;

        [NonVersionable]
        public static unsafe UIntPtr operator -(UIntPtr pointer, int offset) =>
            (nuint)pointer._value - (nuint)offset;

        public static int Size
        {
            [NonVersionable]
            get => sizeof(nuint_t);
        }

        [NonVersionable]
        public unsafe void* ToPointer() => _value;

        public static UIntPtr MaxValue
        {
            [NonVersionable]
            get => (UIntPtr)nuint_t.MaxValue;
        }

        public static UIntPtr MinValue
        {
            [NonVersionable]
            get => (UIntPtr)nuint_t.MinValue;
        }

        public unsafe int CompareTo(object? value)
        {
            if (value is null)
            {
                return 1;
            }
            if (value is nuint i)
            {
                if ((nuint)_value < i) return -1;
                if ((nuint)_value > i) return 1;
                return 0;
            }

            throw new ArgumentException(SR.Arg_MustBeUIntPtr);
        }

        public unsafe int CompareTo(UIntPtr value) => ((nuint_t)_value).CompareTo((nuint_t)value);

        [NonVersionable]
        public unsafe bool Equals(UIntPtr other) => (nuint)_value == (nuint)other;

        public unsafe override string ToString() => ((nuint_t)_value).ToString();
        public unsafe string ToString(string? format) => ((nuint_t)_value).ToString(format);
        public unsafe string ToString(IFormatProvider? provider) => ((nuint_t)_value).ToString(provider);
        public unsafe string ToString(string? format, IFormatProvider? provider) => ((nuint_t)_value).ToString(format, provider);

        public unsafe bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) =>
            ((nuint_t)_value).TryFormat(destination, out charsWritten, format, provider);

        public static UIntPtr Parse(string s) => (UIntPtr)nuint_t.Parse(s);
        public static UIntPtr Parse(string s, NumberStyles style) => (UIntPtr)nuint_t.Parse(s, style);
        public static UIntPtr Parse(string s, IFormatProvider? provider) => (UIntPtr)nuint_t.Parse(s, provider);
        public static UIntPtr Parse(string s, NumberStyles style, IFormatProvider? provider) => (UIntPtr)nuint_t.Parse(s, style, provider);
        public static UIntPtr Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null) => (UIntPtr)nuint_t.Parse(s, style, provider);

        public static bool TryParse([NotNullWhen(true)] string? s, out UIntPtr result)
        {
            Unsafe.SkipInit(out result);
            return nuint_t.TryParse(s, out Unsafe.As<UIntPtr, nuint_t>(ref result));
        }

        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out UIntPtr result)
        {
            Unsafe.SkipInit(out result);
            return nuint_t.TryParse(s, style, provider, out Unsafe.As<UIntPtr, nuint_t>(ref result));
        }

        public static bool TryParse(ReadOnlySpan<char> s, out UIntPtr result)
        {
            Unsafe.SkipInit(out result);
            return nuint_t.TryParse(s, out Unsafe.As<UIntPtr, nuint_t>(ref result));
        }

        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out UIntPtr result)
        {
            Unsafe.SkipInit(out result);
            return nuint_t.TryParse(s, style, provider, out Unsafe.As<UIntPtr, nuint_t>(ref result));
        }

#if FEATURE_GENERIC_MATH
        //
        // IAdditionOperators
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IAdditionOperators<nuint, nuint, nuint>.operator +(nuint left, nuint right)
            => (nuint)(left + right);

        // [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        // static checked nuint IAdditionOperators<nuint, nuint, nuint>.operator +(nuint left, nuint right)
        //     => checked((nuint)(left + right));

        //
        // IAdditiveIdentity
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IAdditiveIdentity<nuint, nuint>.AdditiveIdentity => 0;

        //
        // IBinaryInteger
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IBinaryInteger<nuint>.LeadingZeroCount(nuint value)
        {
            if (Environment.Is64BitProcess)
            {
                return (nuint)BitOperations.LeadingZeroCount((ulong)value);
            }
            else
            {
                return (nuint)BitOperations.LeadingZeroCount((uint)value);
            }
        }

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IBinaryInteger<nuint>.PopCount(nuint value)
        {
            if (Environment.Is64BitProcess)
            {
                return (nuint)BitOperations.PopCount((ulong)value);
            }
            else
            {
                return (nuint)BitOperations.PopCount((uint)value);
            }
        }

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IBinaryInteger<nuint>.RotateLeft(nuint value, int rotateAmount)
        {
            if (Environment.Is64BitProcess)
            {
                return (nuint)BitOperations.RotateLeft((ulong)value, rotateAmount);
            }
            else
            {
                return (nuint)BitOperations.RotateLeft((uint)value, rotateAmount);
            }
        }

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IBinaryInteger<nuint>.RotateRight(nuint value, int rotateAmount)
        {
            if (Environment.Is64BitProcess)
            {
                return (nuint)BitOperations.RotateRight((ulong)value, rotateAmount);
            }
            else
            {
                return (nuint)BitOperations.RotateRight((uint)value, rotateAmount);
            }
        }

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IBinaryInteger<nuint>.TrailingZeroCount(nuint value)
        {
            if (Environment.Is64BitProcess)
            {
                return (nuint)BitOperations.TrailingZeroCount((ulong)value);
            }
            else
            {
                return (nuint)BitOperations.TrailingZeroCount((uint)value);
            }
        }

        //
        // IBinaryNumber
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static bool IBinaryNumber<nuint>.IsPow2(nuint value)
        {
            if (Environment.Is64BitProcess)
            {
                return BitOperations.IsPow2((ulong)value);
            }
            else
            {
                return BitOperations.IsPow2((uint)value);
            }
        }

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IBinaryNumber<nuint>.Log2(nuint value)
        {
            if (Environment.Is64BitProcess)
            {
                return (nuint)BitOperations.Log2((ulong)value);
            }
            else
            {
                return (nuint)BitOperations.Log2((uint)value);
            }
        }

        //
        // IBitwiseOperators
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IBitwiseOperators<nuint, nuint, nuint>.operator &(nuint left, nuint right)
            => left & right;

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IBitwiseOperators<nuint, nuint, nuint>.operator |(nuint left, nuint right)
            => left | right;

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IBitwiseOperators<nuint, nuint, nuint>.operator ^(nuint left, nuint right)
            => left ^ right;

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IBitwiseOperators<nuint, nuint, nuint>.operator ~(nuint value)
            => ~value;

        //
        // IComparisonOperators
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static bool IComparisonOperators<nuint, nuint>.operator <(nuint left, nuint right)
            => left < right;

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static bool IComparisonOperators<nuint, nuint>.operator <=(nuint left, nuint right)
            => left <= right;

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static bool IComparisonOperators<nuint, nuint>.operator >(nuint left, nuint right)
            => left > right;

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static bool IComparisonOperators<nuint, nuint>.operator >=(nuint left, nuint right)
            => left >= right;

        //
        // IDecrementOperators
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IDecrementOperators<nuint>.operator --(nuint value)
            => --value;

        // [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        // static checked nuint IDecrementOperators<nuint>.operator --(nuint value)
        //     => checked(--value);

        //
        // IDivisionOperators
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IDivisionOperators<nuint, nuint, nuint>.operator /(nuint left, nuint right)
            => left / right;

        // [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        // static checked nuint IDivisionOperators<nuint, nuint, nuint>.operator /(nuint left, nuint right)
        //     => checked(left / right);

        //
        // IEqualityOperators
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static bool IEqualityOperators<nuint, nuint>.operator ==(nuint left, nuint right)
            => left == right;

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static bool IEqualityOperators<nuint, nuint>.operator !=(nuint left, nuint right)
            => left != right;

        //
        // IIncrementOperators
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IIncrementOperators<nuint>.operator ++(nuint value)
            => ++value;

        // [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        // static checked nuint IIncrementOperators<nuint>.operator ++(nuint value)
        //     => checked(++value);

        //
        // IMinMaxValue
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IMinMaxValue<nuint>.MinValue => MinValue;

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IMinMaxValue<nuint>.MaxValue => MaxValue;

        //
        // IModulusOperators
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IModulusOperators<nuint, nuint, nuint>.operator %(nuint left, nuint right)
            => left % right;

        // [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        // static checked nuint IModulusOperators<nuint, nuint, nuint>.operator %(nuint left, nuint right)
        //     => checked(left % right);

        //
        // IMultiplicativeIdentity
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IMultiplicativeIdentity<nuint, nuint>.MultiplicativeIdentity => 1;

        //
        // IMultiplyOperators
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IMultiplyOperators<nuint, nuint, nuint>.operator *(nuint left, nuint right)
            => left * right;

        // [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        // static checked nuint IMultiplyOperators<nuint, nuint, nuint>.operator *(nuint left, nuint right)
        //     => checked(left * right);

        //
        // INumber
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint INumber<nuint>.One => 1;

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint INumber<nuint>.Zero => 0;

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint INumber<nuint>.Abs(nuint value)
            => value;

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint INumber<nuint>.Clamp(nuint value, nuint min, nuint max)
            => Math.Clamp(value, min, max);

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static nuint INumber<nuint>.Create<TOther>(TOther value)
        {
            if (typeof(TOther) == typeof(byte))
            {
                return (byte)(object)value;
            }
            else if (typeof(TOther) == typeof(char))
            {
                return (char)(object)value;
            }
            else if (typeof(TOther) == typeof(decimal))
            {
                return checked((nuint)(decimal)(object)value);
            }
            else if (typeof(TOther) == typeof(double))
            {
                return checked((nuint)(double)(object)value);
            }
            else if (typeof(TOther) == typeof(short))
            {
                return checked((nuint)(short)(object)value);
            }
            else if (typeof(TOther) == typeof(int))
            {
                return checked((nuint)(int)(object)value);
            }
            else if (typeof(TOther) == typeof(long))
            {
                return checked((nuint)(long)(object)value);
            }
            else if (typeof(TOther) == typeof(nint))
            {
                return checked((nuint)(nint)(object)value);
            }
            else if (typeof(TOther) == typeof(sbyte))
            {
                return checked((nuint)(sbyte)(object)value);
            }
            else if (typeof(TOther) == typeof(float))
            {
                return checked((nuint)(float)(object)value);
            }
            else if (typeof(TOther) == typeof(ushort))
            {
                return (ushort)(object)value;
            }
            else if (typeof(TOther) == typeof(uint))
            {
                return (uint)(object)value;
            }
            else if (typeof(TOther) == typeof(ulong))
            {
                return checked((nuint)(ulong)(object)value);
            }
            else if (typeof(TOther) == typeof(nuint))
            {
                return (nuint)(object)value;
            }
            else
            {
                ThrowHelper.ThrowNotSupportedException();
                return default;
            }
        }

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static nuint INumber<nuint>.CreateSaturating<TOther>(TOther value)
        {
            if (typeof(TOther) == typeof(byte))
            {
                return (byte)(object)value;
            }
            else if (typeof(TOther) == typeof(char))
            {
                return (char)(object)value;
            }
            else if (typeof(TOther) == typeof(decimal))
            {
                var actualValue = (decimal)(object)value;
                return (actualValue > nuint.MaxValue) ? MaxValue :
                       (actualValue < 0) ? MinValue : (nuint)actualValue;
            }
            else if (typeof(TOther) == typeof(double))
            {
                var actualValue = (double)(object)value;
                return (actualValue > nuint.MaxValue) ? MaxValue :
                       (actualValue < 0) ? MinValue : (nuint)actualValue;
            }
            else if (typeof(TOther) == typeof(short))
            {
                var actualValue = (short)(object)value;
                return (actualValue < 0) ? MinValue : (nuint)actualValue;
            }
            else if (typeof(TOther) == typeof(int))
            {
                var actualValue = (int)(object)value;
                return (actualValue < 0) ? MinValue : (nuint)actualValue;
            }
            else if (typeof(TOther) == typeof(long))
            {
                var actualValue = (long)(object)value;

                return ((Size == 4) && (actualValue > uint.MaxValue)) ? MaxValue :
                       (actualValue < 0) ? MinValue : (nuint)actualValue;
            }
            else if (typeof(TOther) == typeof(nint))
            {
                var actualValue = (nint)(object)value;
                return (actualValue < 0) ? MinValue : (nuint)actualValue;
            }
            else if (typeof(TOther) == typeof(sbyte))
            {
                var actualValue = (sbyte)(object)value;
                return (actualValue < 0) ? MinValue : (nuint)actualValue;
            }
            else if (typeof(TOther) == typeof(float))
            {
                var actualValue = (float)(object)value;
                return (actualValue > nuint.MaxValue) ? MaxValue :
                       (actualValue < 0) ? MinValue : (nuint)actualValue;
            }
            else if (typeof(TOther) == typeof(ushort))
            {
                return (ushort)(object)value;
            }
            else if (typeof(TOther) == typeof(uint))
            {
                return (uint)(object)value;
            }
            else if (typeof(TOther) == typeof(ulong))
            {
                var actualValue = (ulong)(object)value;
                return (actualValue > nuint.MaxValue) ? MaxValue : (nuint)actualValue;
            }
            else if (typeof(TOther) == typeof(nuint))
            {
                return (nuint)(object)value;
            }
            else
            {
                ThrowHelper.ThrowNotSupportedException();
                return default;
            }
        }

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static nuint INumber<nuint>.CreateTruncating<TOther>(TOther value)
        {
            if (typeof(TOther) == typeof(byte))
            {
                return (byte)(object)value;
            }
            else if (typeof(TOther) == typeof(char))
            {
                return (char)(object)value;
            }
            else if (typeof(TOther) == typeof(decimal))
            {
                return (nuint)(decimal)(object)value;
            }
            else if (typeof(TOther) == typeof(double))
            {
                return (nuint)(double)(object)value;
            }
            else if (typeof(TOther) == typeof(short))
            {
                return (nuint)(short)(object)value;
            }
            else if (typeof(TOther) == typeof(int))
            {
                return (nuint)(int)(object)value;
            }
            else if (typeof(TOther) == typeof(long))
            {
                return (nuint)(long)(object)value;
            }
            else if (typeof(TOther) == typeof(nint))
            {
                return (nuint)(nint)(object)value;
            }
            else if (typeof(TOther) == typeof(sbyte))
            {
                return (nuint)(sbyte)(object)value;
            }
            else if (typeof(TOther) == typeof(float))
            {
                return (nuint)(float)(object)value;
            }
            else if (typeof(TOther) == typeof(ushort))
            {
                return (ushort)(object)value;
            }
            else if (typeof(TOther) == typeof(uint))
            {
                return (uint)(object)value;
            }
            else if (typeof(TOther) == typeof(ulong))
            {
                return (nuint)(ulong)(object)value;
            }
            else if (typeof(TOther) == typeof(nuint))
            {
                return (nuint)(object)value;
            }
            else
            {
                ThrowHelper.ThrowNotSupportedException();
                return default;
            }
        }

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static (nuint Quotient, nuint Remainder) INumber<nuint>.DivRem(nuint left, nuint right)
            => Math.DivRem(left, right);

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint INumber<nuint>.Max(nuint x, nuint y)
            => Math.Max(x, y);

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint INumber<nuint>.Min(nuint x, nuint y)
            => Math.Min(x, y);

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint INumber<nuint>.Parse(string s, NumberStyles style, IFormatProvider? provider)
            => Parse(s, style, provider);

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint INumber<nuint>.Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
            => Parse(s, style, provider);

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint INumber<nuint>.Sign(nuint value)
            => (nuint)((value == 0) ? 0 : 1);

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool INumber<nuint>.TryCreate<TOther>(TOther value, out nuint result)
        {
            if (typeof(TOther) == typeof(byte))
            {
                result = (byte)(object)value;
                return true;
            }
            else if (typeof(TOther) == typeof(char))
            {
                result = (char)(object)value;
                return true;
            }
            else if (typeof(TOther) == typeof(decimal))
            {
                var actualValue = (decimal)(object)value;

                if ((actualValue < 0) || (actualValue > nuint.MaxValue))
                {
                    result = default;
                    return false;
                }

                result = (nuint)actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(double))
            {
                var actualValue = (double)(object)value;

                if ((actualValue < 0) || (actualValue > nuint.MaxValue))
                {
                    result = default;
                    return false;
                }

                result = (nuint)actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(short))
            {
                var actualValue = (short)(object)value;

                if (actualValue < 0)
                {
                    result = default;
                    return false;
                }

                result = (nuint)actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(int))
            {
                var actualValue = (int)(object)value;

                if (actualValue < 0)
                {
                    result = default;
                    return false;
                }

                result = (nuint)actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(long))
            {
                var actualValue = (long)(object)value;

                if ((actualValue < 0) || ((Size == 4) && (actualValue > uint.MaxValue)))
                {
                    result = default;
                    return false;
                }

                result = (nuint)actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(nint))
            {
                var actualValue = (nint)(object)value;

                if (actualValue < 0)
                {
                    result = default;
                    return false;
                }

                result = (nuint)actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(sbyte))
            {
                var actualValue = (sbyte)(object)value;

                if (actualValue < 0)
                {
                    result = default;
                    return false;
                }

                result = (nuint)actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(float))
            {
                var actualValue = (float)(object)value;

                if ((actualValue < 0) || (actualValue > nuint.MaxValue))
                {
                    result = default;
                    return false;
                }

                result = (nuint)actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(ushort))
            {
                result = (ushort)(object)value;
                return true;
            }
            else if (typeof(TOther) == typeof(uint))
            {
                result = (uint)(object)value;
                return true;
            }
            else if (typeof(TOther) == typeof(ulong))
            {
                var actualValue = (ulong)(object)value;

                if (actualValue > nuint.MaxValue)
                {
                    result = default;
                    return false;
                }

                result = (nuint)actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(nuint))
            {
                result = (nuint)(object)value;
                return true;
            }
            else
            {
                ThrowHelper.ThrowNotSupportedException();
                result = default;
                return false;
            }
        }

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static bool INumber<nuint>.TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out nuint result)
            => TryParse(s, style, provider, out result);

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static bool INumber<nuint>.TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out nuint result)
            => TryParse(s, style, provider, out result);

        //
        // IParseable
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IParseable<nuint>.Parse(string s, IFormatProvider? provider)
            => Parse(s, provider);

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static bool IParseable<nuint>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out nuint result)
            => TryParse(s, NumberStyles.Integer, provider, out result);

        //
        // IShiftOperators
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IShiftOperators<nuint, nuint>.operator <<(nuint value, int shiftAmount)
            => value << (int)shiftAmount;

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IShiftOperators<nuint, nuint>.operator >>(nuint value, int shiftAmount)
            => value >> (int)shiftAmount;

        // [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        // static nuint IShiftOperators<nuint, nuint>.operator >>>(nuint value, int shiftAmount)
        //     => value >> (int)shiftAmount;

        //
        // ISpanParseable
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint ISpanParseable<nuint>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
            => Parse(s, NumberStyles.Integer, provider);

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static bool ISpanParseable<nuint>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out nuint result)
            => TryParse(s, NumberStyles.Integer, provider, out result);

        //
        // ISubtractionOperators
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint ISubtractionOperators<nuint, nuint, nuint>.operator -(nuint left, nuint right)
            => left - right;

        // [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        // static checked nuint ISubtractionOperators<nuint, nuint, nuint>.operator -(nuint left, nuint right)
        //     => checked(left - right);

        //
        // IUnaryNegationOperators
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IUnaryNegationOperators<nuint, nuint>.operator -(nuint value)
            => (nuint)0 - value;

        // [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        // static checked nuint IUnaryNegationOperators<nuint, nuint>.operator -(nuint value)
        //     => checked((nuint)0 - value);

        //
        // IUnaryPlusOperators
        //

        [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        static nuint IUnaryPlusOperators<nuint, nuint>.operator +(nuint value)
            => +value;

        // [RequiresPreviewFeatures(Number.PreviewFeatureMessage, Url = Number.PreviewFeatureUrl)]
        // static checked nuint IUnaryPlusOperators<nuint, nuint>.operator +(nuint value)
        //     => checked(+value);
#endif // FEATURE_GENERIC_MATH
    }
}
