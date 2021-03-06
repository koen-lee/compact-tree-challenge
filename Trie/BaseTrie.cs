﻿using System;
using System.IO;
using System.Text;

namespace Trie
{
    // An ITrie implementation that does not actually use a trie
    abstract class BaseTrie : ITrie
    {
        protected static readonly Encoding Encoding = new UTF8Encoding();

        protected readonly byte[] _storage; // required: this is the only field allowed

        protected BaseTrie(byte[] storage)
        {
            if (storage.Length - 1 > ushort.MaxValue) throw new ArgumentOutOfRangeException(nameof(storage), "Array too big");
            _storage = storage;
        }

        protected long ReadLong(int address, out int next)
        {
            next = address + sizeof(long);
            return BitConverter.ToInt64(_storage, address);
        }
        protected ushort ReadUshort(int address, out int next)
        {
            next = address + sizeof(ushort);
            return BitConverter.ToUInt16(_storage, address);
        }

        public abstract bool TryWrite(string key, long value);

        protected void WriteLong(int i, long value, out int next)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteBytes(i, bytes, out next);
        }

        protected void WriteUshort(int i, ushort value, out int next)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteBytes(i, bytes, out next);
        }

        protected void WriteBytes(int i, byte[] bytes, out int next)
        {
            next = i + bytes.Length;
            Array.Copy(bytes, 0, _storage, i, bytes.Length);
        }

        public abstract bool TryRead(string key, out long value);

        public abstract void Delete(string key);

        public void Save(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                stream.Write(_storage, 0, _storage.Length);
            }
        }

        public void Load(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var lastAddress = 0;
                do
                {
                    var read = stream.Read(_storage, lastAddress, _storage.Length - lastAddress);
                    if (read == 0)
                        throw new ArgumentException("file too small");
                    lastAddress += read;
                } while (lastAddress < _storage.Length);
            }
        }
    }
}
