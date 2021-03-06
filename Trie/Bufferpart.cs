﻿using System;
using System.Diagnostics;

namespace Trie
{
    [DebuggerDisplay("l={Length} {AsUtf8String}")]
    public struct Bufferpart
    {
        public readonly byte[] Buffer;
        public readonly int Offset;
        public readonly int Length;
        
        public string AsUtf8String => this.ToStringUtf8();

        public Bufferpart(byte[] buffer)
            : this(buffer, 0, buffer.Length)
        { }

        public Bufferpart(byte[] buffer, int offset, int length)
        {
            this.Buffer = buffer;
            this.Offset = offset;
            this.Length = length;
        }

        public Bufferpart(byte[] buffer, int offset) : this(buffer, offset, buffer.Length - offset)
        {}

        public byte[] GetBytes()
        {
            var result = new byte[Length];
            Array.Copy(Buffer, Offset, result, 0, Length);
            return result;
        }

        public Bufferpart Substring(int startIndex)
        {
            return Substring(startIndex, Length-startIndex);
        }

        public Bufferpart Substring(int startIndex, int length)
        {
            if (startIndex + length > Length)
                throw new ArgumentException();
            return new Bufferpart(Buffer, Offset + startIndex, length);
        }

        public static Bufferpart operator +(Bufferpart left, Bufferpart right)
        {
            var result = new byte[left.Length + right.Length];
            Array.Copy(left.Buffer, left.Offset, result, 0, left.Length);
            Array.Copy(right.Buffer, right.Offset, result, left.Length, right.Length);
            return new Bufferpart(result);
        }
        
        public static bool operator ==(Bufferpart left, Bufferpart right)
        {
            return left.Length == right.Length && left.StartsWith(right);
        }

        public static bool operator !=(Bufferpart left, Bufferpart right)
        {
            return !(left == right);
        }

        public bool StartsWith(Bufferpart key)
        {
            if (key.Length > Length)
                return false;
            for (int i = 0; i < key.Length; i++)
            {
                if (this[i] != key[i])
                    return false;
            }
            return true;
        }

        public byte this[int i] => Buffer[Offset + i];
    }
}