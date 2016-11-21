using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Garage
{
    public class Record
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Id { get; set; }
        public TimeSpan Duration => End - Start;
    }

    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.MonitoringIsEnabled = true;
            var sp = Stopwatch.StartNew();

            const string path = @"data.txt";
            CreateSummaryOptimized(path);

            Console.WriteLine($"Took: {sp.ElapsedMilliseconds:#,#} ms and allocated {AppDomain.CurrentDomain.MonitoringTotalAllocatedMemorySize / 1024:#,#} kb with peak working set of {Process.GetCurrentProcess().PeakWorkingSet64 / 1024:#,#} kb");
        }

        const int DICT_BLOCK_SIZE = 1000;
        const int recordsize = 50;

        private static void CreateSummaryOptimized(string path)
        {
            var dict = new int[9999999 + 1 / DICT_BLOCK_SIZE][];
            var file = new FileInfo(path);

            var record = new byte[1000 * recordsize];
            using (var records = file.OpenRead())
            {
                for (;;)
                {
                    var bytesRead = records.Read(record, 0, record.Length);
                    if (bytesRead <= 0) break;
                    for (int i = 0; i < bytesRead; i += recordsize)
                        ProcessLine(record, i, dict);
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

        private static void ProcessLine(byte[] record, int offset, int[][] dict)
        {
            int durationSeconds;
            if (!BufferEqual(record, startIndex1: offset + 0, startIndex2: offset + 20, length: 10)) // Same day check
            {
                var start = ParseDateTime(record, offset + 0);
                var end = ParseDateTime(record, offset + 20);
                durationSeconds = (int)(end - start).TotalSeconds;
            }
            else
            {
                var start = ParseTime(record, index: offset + 11);
                var end = ParseTime(record, index: offset + 31);
                durationSeconds = end - start;
            }
            var id = ParseInt(record, startIndex: offset + 40, length: 8);
            int[] dictionary = dict[id / DICT_BLOCK_SIZE];
            //Interlocked.Add(ref dict[id], durationSeconds);
            if (dictionary == null) dict[id / DICT_BLOCK_SIZE] = dictionary = new int[DICT_BLOCK_SIZE];
            dictionary[id % DICT_BLOCK_SIZE] += durationSeconds;
        }

        private static DateTime ParseDateTime(byte[] record, int offset)
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
        private static int ParseTime(byte[] record, int index)
        {
            if (record[index - 1] != 'T') throw new FormatException(ToString(record).Substring(index, 8) + " is not a time");
            //hours
            var result = ParseInt(record, index, 2);
            //minutes
            result = result * 60 + ParseInt(record, index + 3, 2);
            //seconds
            result = result * 60 + ParseInt(record, index + 6, 2);
            return result;
        }

        private static int ParseInt(byte[] record, int startIndex, int length)
        {
            var result = 0;
            for (var i = startIndex; i < startIndex + length; i++)
            {
                result = result * 10 + record[i] - '0';
            }
            return result;
        }

        private static bool BufferEqual(byte[] record, int startIndex1, int startIndex2, int length)
        {
            for (int i = length - 1; i >= 0; i--)
            {
                if (record[startIndex1 + i] != record[startIndex2 + i]) return false;
            }
            return true;
        }

        static string ToString(byte[] buffer)
        {
            return Encoding.UTF8.GetString(buffer);
        }
    }
}
