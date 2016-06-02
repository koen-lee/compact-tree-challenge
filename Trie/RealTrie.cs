using System;
using System.Collections.Generic;
using System.Linq;
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
            if (key == string.Empty)
                return false;
            // parse entire tree
            int i = 0;
            var roots = new List<TrieItem>();
            while (i < _storage.Length)
            {
                var itemRead = TrieItem.Read(this, ref i);
                if (itemRead == null)
                    break;
                roots.Add(itemRead);
            }
            //add new item
            roots.Add(new TrieItem(key, value));
            // write back
            i = 0;
            return roots.All(item => item.Write(this, ref i));
        }
        
        private int GetCommonPrefix(string left, string right)
        {
            if (left.Length > right.Length)
                return GetCommonPrefix(right, left); //swap arguments
            for (int i = 0; i < left.Length; i++)
            {
                if (left[0] != right[0])
                    return i;
            }
            return left.Length;
        }

        private bool WriteItem(int address, string key, long value)
        {
            var bytes = Encoding.GetBytes(key);
            var size = sizeof(ushort) // string length
                       + bytes.Length // string
                       + sizeof(ushort) //subtree length
                       + sizeof(byte) // flags
                       + sizeof(long); // value
            if (address
                + size
                > _storage.Length)
                return false; // does not fit
            WriteUshort(address, (ushort)bytes.Length, out address);
            WriteBytes(address, bytes, out address);
            WriteUshort(address, sizeof(byte) + sizeof(long), out address);
            WriteFlags(address, true, false, out address);
            WriteLong(address, value, out address);
            return true;
        }

        private void WriteFlags(int address, bool hasvalue, bool haschildren, out int i)
        {
            _storage[address] = (byte)((haschildren ? HasChildrenFlag : 0) | (hasvalue ? HasValueFlag : 0));
            i = address + 1;
        }

        private static readonly byte HasValueFlag = 1 << 0;
        private static readonly byte HasChildrenFlag = 1 << 1;

        private void ReadFlags(ref int address, out bool hasValue, out bool hasChildren)
        {
            var flags = _storage[address];
            hasValue = (flags & HasValueFlag) != 0;
            hasChildren = (flags & HasChildrenFlag) != 0;
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


        public class TrieItem
        {
            public string Key { get; private set; }
            public bool HasValue => Value.HasValue;
            private readonly List<TrieItem> _children;
            private byte[] _keyBytes;
            public bool HasChildren => _children.Any();
            public long? Value { get; }
            public IEnumerable<TrieItem> Children => _children;

            public TrieItem(string key, long? value)
            {
                Key = key;
                Value = value;
                _children = new List<TrieItem>();
                _keyBytes = new UTF8Encoding().GetBytes(Key);
            }

            public void AddChild(TrieItem child)
            {
                _children.Add(child);
            }

            public int PayloadSize
            {
                get
                {
                    return sizeof(byte) //flags
                           + (HasValue ? sizeof(long) : 0)
                           + Children.Sum(c => c.ItemSize);
                }
            }

            public int ItemSize
            {
                get
                {
                    return sizeof(ushort) //key length
                           + _keyBytes.Length
                           + sizeof(ushort) //payload size
                           + PayloadSize;
                }
            }

            public static TrieItem Read(RealTrie storage, ref int address)
            {
                var key = storage.ReadString(address, out address);
                if (key == string.Empty) return null;
                var max = address + storage.ReadUshort(address, out address);
                bool hasChildren, hasValue;
                storage.ReadFlags(ref address, out hasValue, out hasChildren);
                long? value = null;
                if (hasValue)
                    value = storage.ReadLong(address, out address);
                var result = new TrieItem(key, value);
                while (address < max)
                {
                    result.AddChild(Read(storage, ref address));
                }
                return result;
            }

            public bool Write(RealTrie storage, ref int address)
            {
                var size = ItemSize;
                if (address + size > storage._storage.Length)
                    return false;
                storage.WriteUshort(address, (ushort)_keyBytes.Length, out address);
                storage.WriteBytes(address, _keyBytes, out address);
                storage.WriteUshort(address, (ushort)PayloadSize, out address);
                storage.WriteFlags(address, HasValue, HasChildren, out address);
                if (HasValue)
                    storage.WriteLong(address, Value.Value, out address);
                foreach (var child in Children)
                {
                    child.Write(storage, ref address);
                }
                return true;
            }
        }
    }

}
