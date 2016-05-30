using System;
using System.CodeDom;
using System.IO;
using System.Text;

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
            throw new NotImplementedException();
        }


        private static readonly ushort HasValue = 1 << 0;
        private static readonly ushort HasChildren = 1 << 1;

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
