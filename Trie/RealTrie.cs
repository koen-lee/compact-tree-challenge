using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            var root = ReadTrie();
            if( root.Children.Any(c=>c.Key.Contains('\0')))
                throw new InvalidDataException();
            //add new item
            string keyLeft;
            var match = root.FindParentOf(key, out keyLeft);
            if (keyLeft.Length == 0) // key already exists 
            {
                match.Value = value;
            }
            else
            {
                match.CreateChild(keyLeft, value);
            }
            return WriteRoot(root);
        }

        private bool WriteRoot(TrieItem root)
        {
            if (root.Children.Sum(c => c.ItemSize) > _storage.Length) return false;
            var i = 0;
            return root.Children.All(item => item.Write(this, ref i));
        }

        private TrieItem ReadTrie()
        {
            var i = 0;
            var root = new TrieItem("", null);
            while (i < _storage.Length - sizeof(ushort))
            {
                var itemRead = TrieItem.Read(this, ref i);
                if (itemRead == null)
                    break;
                root.AddChild(itemRead);
            }
            return root;
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
            var root = ReadTrie();
            string keyLeft;
            var item = root.FindParentOf(key, out keyLeft);
            if (keyLeft.Length > 0) return; //key not found
            if (!item.HasValue) return; // no value to remove
            // otherwise, there is work to do
            var parentOfItem = root.FindParentOf(key.Substring(0, key.Length - 1), out keyLeft);
            parentOfItem.Delete(item);

            ClearStorage();
            WriteRoot(root);
        }

        private void ClearStorage()
        {
            Array.Copy(new byte[_storage.Length], _storage, _storage.Length);
        }

        [DebuggerDisplay("{Key} {Value}")]
        public class TrieItem
        {
            public string Key
            {
                get { return _key; }
                private set
                {
                    _key = value;
                    _keyBytes = Encoding.GetBytes(Key);
                }
            }

            public bool HasValue => Value.HasValue;
            private readonly List<TrieItem> _children;
            private byte[] _keyBytes;
            private string _key;
            public bool HasChildren => _children.Any();
            public long? Value { get; set; }
            public IEnumerable<TrieItem> Children => _children;

            public TrieItem(string key, long? value)
            {
                Key = key;
                Value = value;
                _children = new List<TrieItem>();
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

            public int ItemSize => sizeof(ushort) //key length
                                   + _keyBytes.Length
                                   + sizeof(ushort) //payload size
                                   + PayloadSize;

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

            /// <summary>
            /// Writes the item at the given address
            /// </summary>
            /// <param name="storage">the storage array</param>
            /// <param name="address">the address to start; is updated to the first free address</param>
            /// <returns>whether the item could be written</returns>
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

            public TrieItem FindParentOf(string key, out string keyLeft)
            {
                foreach (var child in Children)
                {
                    if (key.StartsWith(child.Key))
                        return child.FindParentOf(key.Substring(child.Key.Length), out keyLeft);
                }
                keyLeft = key;
                return this;
            }

            public void CreateChild(string keyLeft, long value)
            {
                foreach (var child in Children)
                {
                    var common = GetCommonPrefix(keyLeft, child.Key);
                    if (common > 0)
                    {
                        SplitChild(child, common, keyLeft, value);
                        return;
                    }
                }
                AddChild(new TrieItem(keyLeft, value));
            }

            private void SplitChild(TrieItem child, int common, string keyLeft, long value)
            {
                var commonKey = keyLeft.Substring(0, common);
                var commonItem = new TrieItem(commonKey, null);
                _children.Remove(child);
                child.Key = child.Key.Substring(common);
                commonItem.AddChild(child);
                commonItem.AddChild(new TrieItem(keyLeft.Substring(common), value));
                AddChild(commonItem);
            }

            private static int GetCommonPrefix(string left, string right)
            {
                if (left.Length > right.Length)
                    return GetCommonPrefix(right, left); //swap arguments
                for (int i = 0; i < left.Length; i++)
                {
                    if (left[i] != right[i])
                        return i;
                }
                return left.Length;
            }

            public void Delete(TrieItem item)
            {
                if (!item.HasChildren)
                    _children.Remove(item);
                else if (item._children.Count == 1)
                {
                    _children.Remove(item);
                    var orphan = item._children[0];
                    orphan.Key = item.Key + orphan.Key;
                    AddChild(orphan);
                }
                else if (item._children.Count > 1)
                    item.Value = null;
            }
        }
    }

}
