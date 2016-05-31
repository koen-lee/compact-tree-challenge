using System;

namespace Trie
{
    // An ITrie implementation that actually uses a trie
    class RealTrie : BaseTrie
    {
        public RealTrie() : base(new byte[32 * 1024])
        { }

        public RealTrie(byte[] storage) : base(storage)
        { }

        public override bool TryWrite(string key, long value)
        {
            var i = 0;
            while (i < _storage.Length - sizeof (ushort))
            {
                int next;
                var readkey = ReadString(i, out next);
                if (readkey == key || readkey == string.Empty)
                    return WriteItem(i, key, value);
                var length = ReadUshort(next, out next);
                i = next + length;
            }
            return false;
        }

        private bool WriteItem(int address, string key, long value)
        {
            var bytes = Encoding.GetBytes(key);
            var size = sizeof (ushort) // string length
                       + bytes.Length // string
                       + sizeof (ushort) //subtree length
                       + sizeof (byte) // flags
                       + sizeof (long); // value
            if (address
                + size
                > _storage.Length)
                return false; // does not fit
            WriteUshort(address, (ushort) bytes.Length, out address);
            WriteBytes(address, bytes, out address);
            WriteUshort(address, sizeof (byte) + sizeof (long), out address);
            WriteFlags(address, true, false, out address);
            WriteLong(address, value, out address);
            return true;
        }

        private void WriteFlags(int address, bool hasvalue, bool haschildren, out int i)
        {
            _storage[address] = (byte) ((haschildren ? HasChildren : 0) | (hasvalue ? HasValue : 0));
            i = address + 1;
        }

        private static readonly byte HasValue = 1 << 0;
        private static readonly byte HasChildren = 1 << 1;

        private void ReadFlags(ref int address, out bool hasValue, out bool hasChildren )
        {
            var flags = _storage[address];
            hasValue = (flags & HasValue)!=0;
            hasChildren = (flags & HasChildren)!=0;
            address++;
        }

        public override bool TryRead(string key, out long value)
        {
            return TryReadValue(key, out value, 0, _storage.Length);
        }

        private bool TryReadValue(string key, out long value, int startAddress, int maxAddress)
        {
            value = 0;
            if (key.Length == 0)
                return false;
            while (startAddress < maxAddress - sizeof(ushort))// a string should fit
            {
                var prefix = ReadString(startAddress, out startAddress);
                if (prefix == string.Empty)
                    return false;
                var subtreeLength = ReadUshort(startAddress, out startAddress);
                if (key.StartsWith(prefix))
                {
                    bool hasValue, hasChildren;
                    ReadFlags(ref startAddress, out hasValue, out hasChildren);
                    if (hasValue)
                    {
                        value = ReadLong(startAddress, out startAddress);
                        if (key.Length == prefix.Length)
                        {
                            return true; // exact match
                        }
                    }
                    if (hasChildren)
                        return TryReadValue(key.Substring(prefix.Length), out value, startAddress,
                            startAddress + subtreeLength);
                    return false;
                }
                startAddress += subtreeLength;
            }
            return false;
        }

        public override void Delete(string key)
        {
            throw new NotImplementedException();
        }
    }
}
