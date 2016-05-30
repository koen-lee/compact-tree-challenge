using System;
using System.IO;
using System.Text;

namespace Trie
{
    // An ITrie implementation that does not actually use a trie
    class NoTrie : BaseTrie
    {
        public NoTrie() : this(new byte[32 * 1024])
        { }

        public NoTrie(byte[] storage): base(storage)
        { }
        
        public override bool TryWrite(string key, long value)
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

        public override bool TryRead(string key, out long value)
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

        public override void Delete(string key)
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
    }
}
