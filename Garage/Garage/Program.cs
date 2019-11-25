using System;
using System.Diagnostics;
using System.IO;

namespace Garage
{
    class Program
    {
        static void Main()
        {
            var sp = Stopwatch.StartNew();

            const string path = @"data.txt";
            CreateSummaryOptimized(path);

            Console.WriteLine($"Took: {sp.ElapsedMilliseconds:#,#} ms with peak working set of {Process.GetCurrentProcess().PeakWorkingSet64 / 1024:#,#} kb");
        }

        const int DICT_BLOCK_SIZE = 1000;
        const int recordsize = 50;
        static int[][] dict = new int[9999999 + 1 / DICT_BLOCK_SIZE][];

        private static void CreateSummaryOptimized(string path)
        {
            var file = new FileInfo(path);

            var record = new byte[1000 * recordsize];
            using (var records = file.OpenRead())
            {
                for (; ; )
                {
                    var bytesRead = records.Read(record, 0, record.Length);
                    if (bytesRead <= 0) break;
                    for (int i = 0; i < bytesRead; i += recordsize)
                    {
                        var span = new ReadOnlySpan<byte>(record, i, recordsize);
                        ProcessLine(span);
                    }
                }
            }
            using (var output = File.CreateText("summary.txt"))
            {
                for (int i = 0; i < dict.Length; i++)
                {
                    if (dict[i] != null)
                    {
                        for (int j = 0; j < dict[i].Length; j++)
                        {
                            if (dict[i][j] > 0)
                                output.WriteLine($"{i * DICT_BLOCK_SIZE + j:D10} {TimeSpan.FromSeconds(dict[i][j]):c}");
                        }
                    }
                }
            }
        }

        private static void ProcessLine(ReadOnlySpan<byte> record)
        {
            int durationSeconds;
            if (!BufferEqual(record.Slice(0, 10), record.Slice(20, 10)))
            {
                var start = ParseDateTime(record, 0);
                var end = ParseDateTime(record, 20);
                durationSeconds = (int)(end - start).TotalSeconds;
            }
            else
            {
                var start = ParseTime(record, index: 11);
                var end = ParseTime(record, index: 31);
                durationSeconds = end - start;
            }
            var id = ParseInt(record, startIndex: 40, length: 8);
            int[] dictionary = dict[id / DICT_BLOCK_SIZE];
            //Interlocked.Add(ref dict[id], durationSeconds);
            if (dictionary == null) dict[id / DICT_BLOCK_SIZE] = dictionary = new int[DICT_BLOCK_SIZE];
            dictionary[id % DICT_BLOCK_SIZE] += durationSeconds;
        }

        private static DateTime ParseDateTime(ReadOnlySpan<byte> record, int offset)
        {
            return new DateTime(kind: DateTimeKind.Utc,
                year: ParseInt(record, offset, 4),
                month: ParseInt(record, offset + 5, 2),
                day: ParseInt(record, offset + 8, 2),
                hour: ParseInt(record, offset + 11, 2),
                minute: ParseInt(record, offset + 14, 2),
                second: ParseInt(record, offset + 17, 2)
                );
        }

        /// <summary>
        /// returns the number of seconds since midnight
        /// </summary>
        /// <param name="record"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static int ParseTime(ReadOnlySpan<byte> record, int index)
        {
            //hours
            var result = ParseUint(record, index);
            //minutes
            result = result * 60 + ParseUint(record, index + 3);
            //seconds
            result = result * 60 + ParseUint(record, index + 6);
            return result;
        }

        private static int ParseUint(ReadOnlySpan<byte> record, int startindex)
        {
            var result = record[startindex] - '0';
            result = result * 10 + record[startindex + 1] - '0';
            return result;
        }

        private static int ParseInt(ReadOnlySpan<byte> record, int startIndex, int length)
        {
            var result = 0;
            for (var i = startIndex; i < startIndex + length; i++)
            {
                result = result * 10 + record[i] - '0';
            }
            return result;
        }
        private static bool BufferEqual(ReadOnlySpan<byte> slice1, ReadOnlySpan<byte> slice2)
        {
            return slice1.SequenceCompareTo(slice2) == 0;
        }
    }
}
