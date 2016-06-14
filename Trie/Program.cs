using System;
using System.Diagnostics;
using System.Threading;

namespace Trie
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var tester = new TrieComparisonTests();
            tester.GenerateTestData();
            var s = Stopwatch.StartNew();
            long total = 0;
            while (s.ElapsedMilliseconds < 1000)
            {
                total += tester.TestOptimizedTrie();
            }
            Console.WriteLine(total);
            Thread.Sleep(1000);
        }
    }
}