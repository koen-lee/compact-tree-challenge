using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Trie
{
    // An ITrie implementation that uses a trie and more optimized storage
    class OptimizedTrie : BaseTrie
    {
        static readonly byte[] EmptyKey = new byte[0];
        public OptimizedTrie() : base(new byte[32 * 1024])
        { }

        public OptimizedTrie(byte[] storage) : base(storage)
        { }

        public override bool TryWrite(string key, long value)
        {
            if (key == string.Empty)
                return false;
            // parse entire tree
            var root = ReadTrie();
            //add new item
            Bufferpart thekey = new Bufferpart(Encoding.GetBytes(key));
            Bufferpart keyLeft;
            var match = root.FindParentOf(thekey, out keyLeft);
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
            var root = new TrieItem(EmptyKey, null);
            while (i < _storage.Length - sizeof(ushort))
            {
                var itemRead = TrieItem.Read(this, ref i);
                if (itemRead == null)
                    break;
                root.AddChild(itemRead);
            }
            return root;
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
            while (startAddress < maxAddress - sizeof(byte))// a string should fit
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
            Bufferpart keyLeft;
            Bufferpart thekey = new Bufferpart(Encoding.GetBytes(key));
            var item = root.FindParentOf(thekey, out keyLeft);
            if (keyLeft.Length > 0) return; //key not found
            if (!item.HasValue) return; // no value to remove
            // otherwise, there is work to do
            var parentOfItem = root.FindParentOf(thekey.Substring(0, thekey.Length - 1), out keyLeft);
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
            public Bufferpart Key
            {
                get { return _key; }
                private set
                {
                    if (Key.Length > MaxKeylength) throw new ArgumentException();
                    _key = value;
                }
            }

            public bool HasValue => Value.HasValue;
            private readonly List<TrieItem> _children;
            private Bufferpart _key;
            private static readonly byte MaxKeylength = 0x3f;
            public bool HasChildren => _children.Any();
            public long? Value { get; set; }
            public IEnumerable<TrieItem> Children => _children;

            public TrieItem(byte[] key, long? value) : this(new Bufferpart(key), value)
            { }

            private TrieItem(Bufferpart key, long? value)
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
                    return Children.Sum(c => c.ItemSize);
                }
            }

            public int ItemSize => sizeof(byte) //key length
                                   + _key.Length
                                   + (HasChildren ? sizeof(ushort) + PayloadSize : 0) //payload size
                                   + (HasValue ? sizeof(long) : 0);

            public static TrieItem Read(OptimizedTrie storage, ref int address)
            {
                var keylength = storage._storage[address];
                if (keylength == 0) return null;
                var hasvalue = (keylength & 1 << 7) != 0;
                var haschildren = (keylength & 1 << 6) != 0;
                keylength &= MaxKeylength;
                address++;
                var key = new Bufferpart(storage._storage, address, keylength);
                address += keylength;
                int max = 0;
                if (haschildren)
                    max = address + storage.ReadUshort(address, out address);
                long? value = null;
                if (hasvalue)
                    value = storage.ReadLong(address, out address);
                var result = new TrieItem(key.GetBytes(), value);
                while (address < max)
                {
                    result.AddChild(Read(storage, ref address));
                }
                return result;
            }

            public static TrieItem Create(Bufferpart key, long? value)
            {
                if (key.Length <= MaxKeylength)
                {
                    return new TrieItem(key, value);
                }
                var parent = new TrieItem(key.Substring(0, MaxKeylength), null);
                parent.AddChild(Create(key.Substring(MaxKeylength), value));
                return parent;
            }

            /// <summary>
            /// Writes the item at the given address
            /// </summary>
            /// <param name="storage">the storage array</param>
            /// <param name="address">the address to start; is updated to the first free address</param>
            /// <returns>whether the item could be written</returns>
            public bool Write(OptimizedTrie storage, ref int address)
            {
                var size = ItemSize;
                if (address + size > storage._storage.Length)
                    return false;
                byte keylength = (byte)_key.Length;
                if (HasValue)
                    keylength |= 1 << 7;
                if (HasChildren)
                    keylength |= 1 << 6;
                storage._storage[address] = keylength;
                address++;
                storage.WriteBytes(address, _key.GetBytes(), out address);
                if (HasChildren)
                    storage.WriteUshort(address, (ushort)PayloadSize, out address);
                if (Value.HasValue)
                    storage.WriteLong(address, Value.Value, out address);
                foreach (var child in Children)
                {
                    child.Write(storage, ref address);
                }
                return true;
            }

            public TrieItem FindParentOf(Bufferpart key, out Bufferpart keyLeft)
            {
                foreach (var child in Children)
                {
                    if (key.StartsWith(child.Key))
                        return child.FindParentOf(key.Substring(child.Key.Length), out keyLeft);
                }
                keyLeft = key;
                return this;
            }

            public void CreateChild(Bufferpart keyLeft, long value)
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
                AddChild(Create(keyLeft, value));
            }

            private void SplitChild(TrieItem child, int common, Bufferpart keyLeft, long value)
            {
                var commonKey = keyLeft.Substring(0, common);
                var commonItem = new TrieItem(commonKey, null);
                _children.Remove(child);
                child.Key = child.Key.Substring(common);
                commonItem.AddChild(child);
                commonItem.AddChild(Create(keyLeft.Substring(common), value));
                AddChild(commonItem);
            }

            private static int GetCommonPrefix(Bufferpart left, Bufferpart right)
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
                    var newKey = item.Key + orphan.Key;
                    orphan.Key = newKey;
                    AddChild(orphan);
                }
                else if (item._children.Count > 1)
                    item.Value = null;
            }
        }
    }
}
