using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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

            foreach (var child in root.Children)
                child.Write(this, ref i);
            return true;
        }

        private TrieItem ReadTrie()
        {
            var i = 0;
            var root = new TrieItem(EmptyKey, null);
            while (i < _storage.Length - sizeof(ushort))
            {
                var itemRead = TrieItem.Read(_storage, ref i);
                if (itemRead == null)
                    break;
                root.AddChild(itemRead);
            }
            return root;
        }

        public override bool TryRead(string key, out long value)
        {
            var thekey = new Bufferpart(Encoding.GetBytes(key));
            return TryReadValue(thekey, out value, 0, _storage.Length);
        }

        private bool TryReadValue(Bufferpart key, out long value, int startAddress, int maxAddress)
        {
            value = 0;
            if (key.Length == 0)
                return false;
            while (startAddress < maxAddress - sizeof(byte))// a string should fit
            {
                int subtreeLength = 0;
                bool hasValue, hasChildren;
                var prefix = GetKey(_storage, ref startAddress, out hasValue, out hasChildren);
                if (prefix.Length == 0)
                    return false;
                if (hasValue)
                    value = ReadLong(startAddress, out startAddress);
                if (hasChildren)
                    subtreeLength = ReadUshort(startAddress, out startAddress);
                if (key.StartsWith(prefix))
                {
                    if (key.Length == prefix.Length)
                    {
                        return hasValue; // exact match
                    }
                    if (hasChildren)
                    {
                        return TryReadValue(key.Substring(prefix.Length), out value, startAddress,
                            startAddress + subtreeLength);
                    }
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

        private static Bufferpart GetKey(byte[] buffer, ref int address, out bool hasvalue, out bool haschildren)
        {
            var keylength = buffer[address];
            if (keylength == 0)
            {
                hasvalue = haschildren = false;
                return new Bufferpart(EmptyKey);
            }
            hasvalue = (keylength & 1 << 7) != 0;
            haschildren = (keylength & 1 << 6) != 0;
            keylength &= TrieItem.MaxKeylength;
            address++;
            var key = new Bufferpart(buffer, address, keylength);
            address += keylength;
            return key;
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

            public static readonly byte MaxKeylength = 0x3f;
            public bool HasChildren { get; private set; }
            public long? Value { get; set; }

            public IEnumerable<TrieItem> Children
            {
                get
                {
                    if (Payload != null)
                    {
                        ReadChildren();
                        Payload = null;
                    }
                    return _children;
                }
            }

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
                HasChildren = true;
            }

            public int PayloadSize
            {
                get
                {
                    if (Payload != null)
                        return Payload.Length;
                    var sum = 0;
                    for (int i = 0; i < _children.Count; i++)
                        sum += _children[i].ItemSize;
                    return sum;
                }
            }

            private byte[] _payload;
            public int ItemSize
            {
                get
                {
                    var size = sizeof(byte) //key length
                               + _key.Length
                               + (HasChildren ? sizeof(ushort) + PayloadSize : 0) //payload size
                               + (HasValue ? sizeof(long) : 0);
                    return size;
                }
            }

            public static TrieItem Read(byte[] buffer, ref int address)
            {
                bool hasvalue;
                bool haschildren;
                Bufferpart key = GetKey(buffer, ref address, out hasvalue, out haschildren);
                if (key.Length == 0)
                    return null;
                long? value = null;
                if (hasvalue)
                    value = buffer.ReadLong(ref address);
                var result = new TrieItem(key.GetBytes(), value);
                if (haschildren)
                {
                    var payloadLength = buffer.ReadUshort(ref address);
                    result.Payload = new Bufferpart(buffer, address, payloadLength).GetBytes();
                    address += payloadLength;
                }
                return result;
            }

            public byte[] Payload
            {
                get { return _payload; }
                set
                {
                    _payload = value;
                    HasChildren = true;
                }
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
            public void Write(OptimizedTrie storage, ref int address)
            {
                byte keylength = (byte)_key.Length;
                if (HasValue)
                    keylength |= 1 << 7;
                if (HasChildren)
                    keylength |= 1 << 6;
                storage._storage[address] = keylength;
                address++;
                storage.WriteBytes(address, _key, out address);
                if (Value.HasValue)
                    storage.WriteLong(address, Value.Value, out address);
                if (HasChildren)
                    storage.WriteUshort(address, (ushort)PayloadSize, out address);
                if (Payload != null)
                    storage.WriteBytes(address, Payload, out address);
                else
                    foreach (var child in Children)
                    {
                        child.Write(storage, ref address);
                    }
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

            private void ReadChildren()
            {
                var address = 0;
                while (address < Payload.Length)
                {
                    AddChild(Read(Payload, ref address));
                }
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
                if (keyLeft.Length == common)
                    commonItem.Value = value;
                else
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
                HasChildren = _children.Count > 0;
            }
        }

        private void WriteBytes(int address, Bufferpart bytes, out int next)
        {
            Array.Copy(bytes.Buffer, bytes.Offset, _storage, address, bytes.Length);
            next = address + bytes.Length;
        }
    }

    public static class ByteArrayExtensions
    {
        public static long ReadLong(this byte[] buffer, ref int address)
        {
            var result = BitConverter.ToInt64(buffer, address);
            address += sizeof(long);
            return result;
        }
        public static ushort ReadUshort(this byte[] buffer, ref int address)
        {
            var result = BitConverter.ToUInt16(buffer, address);
            address += sizeof(ushort);
            return result;
        }

        public static string ToStringUtf8(this byte[] bytes)
        {
            return Utf8.GetString(bytes);
        }
        public static string ToStringUtf8(this Bufferpart bytes)
        {
            return Utf8.GetString(bytes.Buffer, bytes.Offset, bytes.Length);
        }
        private static readonly Encoding Utf8 = new UTF8Encoding();
    }
}
