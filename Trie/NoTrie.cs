using System;
using System.IO;
using System.Text;

namespace Trie
{
    // An ITrie implementation that does not actually use a trie
    class NoTrie : ITrie
    {
        private static readonly Encoding Encoding = new UTF8Encoding();

        private readonly byte[] _storage; // required: this is the only field allowed

        public NoTrie() : this(new byte[32 * 1024])
        { }

        public NoTrie(byte[] storage)
        {
            if (storage.Length > 32 * 1024) throw new ArgumentOutOfRangeException(nameof(storage), "Array too big");
            _storage = storage;
        }


        private string ReadString(int address, out int next)
        {
            int startOfString;
            var length = ReadUshort(address, out startOfString);
            if (length == 0)
            {
                next = address;
                return string.Empty;
            }
            next = startOfString + length;
            return Encoding.GetString(_storage, startOfString, length );
        }

        private long ReadLong(int address, out int next)
        {
            next = address + sizeof(long);
            return BitConverter.ToInt64(_storage, address);
        }
        private ushort ReadUshort(int address, out int next)
        {
            next = address + sizeof(ushort);
            return BitConverter.ToUInt16(_storage, address);
        }

        public bool TryWrite(string key, long value)
        {
            if (string.IsNullOrEmpty(key)) return false; // not in requirements
            int i = 0;
            while (i < _storage.Length - sizeof(long))
            {
                var thiskey = ReadString(i, out i);
                if (thiskey == string.Empty) // end of list
                    return WriteKeyValue(i, key, value);
                if (thiskey == key) // key exists, overwrite
                {
                    WriteLong(i, value, out i);
                    return true;
                }
                ReadLong(i, out i);
            }
            return false;
        }

        private void WriteLong(int i, long value, out int next)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteBytes(i, bytes, out next);
        }

        private void WriteUshort(int i, ushort value, out int next)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteBytes(i, bytes, out next);
        }

        private void WriteBytes(int i, byte[] bytes, out int next)
        {
            next = i + bytes.Length;
            Array.Copy(bytes, 0, _storage, i, bytes.Length);
        }

        private bool WriteKeyValue(int i, string key, long value)
        {
            var bytes = Encoding.GetBytes(key);
            if (i + sizeof(ushort) + bytes.Length + sizeof(long) > _storage.Length)
                return false; // does not fit
            WriteUshort(i, (ushort)bytes.Length, out i);
            WriteBytes(i, bytes, out i);
            WriteLong(i, value, out i);
            return true;
        }

        public bool TryRead(string key, out long value)
        {
            value = -1L;
            int i = 0;
            while (i < _storage.Length)
            {
                var thiskey = ReadString(i, out i);
                if (thiskey == string.Empty)
                    return false;
                var thisvalue = ReadLong(i, out i);
                if (thiskey == key) // key exists, success!
                {
                    value = thisvalue;
                    return true;
                }
            }
            return false;
        }

        public void Delete(string key)
        {
            int i = 0;
            while (i < _storage.Length)
            {
                var thisAddress = i;
                var thiskey = ReadString(i, out i);
                if (thiskey == string.Empty) // end of list, key not found
                    return;
                ReadLong(i, out i);
                if (thiskey == key) // key found
                {
                    //Copy everything except this key/value to a new array
                    var newStorage = new byte[_storage.Length];
                    Array.Copy(_storage, 0, newStorage, 0, thisAddress);
                    Array.Copy(_storage, i, newStorage, thisAddress, _storage.Length - i);
                    Array.Copy(newStorage, 0, _storage, 0, _storage.Length);
                    return;
                }
            }
        }

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
                stream.Read(_storage, 0, _storage.Length);
            }
        }
    }
}
