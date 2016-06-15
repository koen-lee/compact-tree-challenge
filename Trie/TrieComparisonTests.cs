using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Trie
{
    [TestFixture]
    public class TrieComparisonTests
    {

        private KeyValuePair<string, long>[] _testdata;

        [TestFixtureSetUp]
        public void GenerateTestData()
        {
            _testdata = Shuffle(GetTestData().Take(32 * 1024 / 8)).ToArray();
        }

        private IEnumerable<T> Shuffle<T>(IEnumerable<T> items)
        {
            var r = new Random();
            return items
                .Select(i => new { item = i, order = r.NextDouble() })
                .OrderBy(kv => kv.order)
                .Select(kv => kv.item);
        }
        
        [Test]
        public int TestOptimizedTrie()
        {
            return TestTrie(new OptimizedTrie(), _testdata);
        }

        private static int TestTrie(ITrie trie, KeyValuePair<string, long>[] testdata)
        {
            var items = 0;
            Console.WriteLine($"Testing {trie.GetType()}");
            var stopwatch = Stopwatch.StartNew();
            foreach (var kv in testdata)
            {
                if (!trie.TryWrite(kv.Key, kv.Value))
                    break;
                items++;
            }
            Console.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds} milliseconds");
            Console.WriteLine($"Items in {trie.GetType()}: {items}");
            stopwatch.Restart();
            foreach (var kv in testdata.Take(items))
            {
                long value;
                if (!trie.TryRead(kv.Key, out value))
                    throw new InvalidDataException($"could not read back {kv.Key}: key not found");
                if ( value != kv.Value)
                    throw new InvalidDataException($"could not read back {kv.Key}: expected {kv.Value}, got {value}");
            }
            Console.WriteLine($"Readback Elapsed: {stopwatch.ElapsedMilliseconds} milliseconds");
            return items;
        }

        IEnumerable<KeyValuePair<string, long>> GetTestData()
        {
            return EnumerateFilesRecursively(new DirectoryInfo(@"C:\Program Files"))
                .Select(file => new KeyValuePair<string, long>(file.FullName, file.Length));
        }

        private IEnumerable<FileInfo> EnumerateFilesRecursively(DirectoryInfo directoryInfo)
        {
            return directoryInfo.EnumerateFiles()
                .Concat(directoryInfo.EnumerateDirectories()
                    .SelectMany(EnumerateFilesRecursively));
        }
    }
}