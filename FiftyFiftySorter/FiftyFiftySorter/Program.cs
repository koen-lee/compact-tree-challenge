using System;
using System.Collections.Generic;
using System.Linq;

namespace SplitSorter
{
    public enum Destination
    {
        Left,
        Right
    }

    static class Program
    {
        struct Plant
        {
            public int Id;
            public int Height;
            public Destination Destination;

            public override string ToString()
            {
                return $"Id: {Id}, Height: {Height}, Destination: {Destination}";
            }
        }

        static void Main(string[] args)
        {
            var random = GetRandomizer(args);
            try
            {
                while (!Console.KeyAvailable)
                    RunSimulation(random);
            }
            catch (Exception e)
            {
                WriteLine(ConsoleColor.Red, e);
            }
            Console.ReadLine();
        }

        private static void RunSimulation(Random random)
        {
            var sorter = new Sorter();

            var plants = SortPlants(random, sorter);
            ReportResults(plants);
        }

        private static List<Plant> SortPlants(Random random, Sorter sorter)
        {
            var min = random.Next(0, 1000);
            var result = new List<Plant>();
            for (int plant = 0; plant < 500; plant++)
            {
                var height = min + random.Next(100) + random.Next(100);
                var destination = sorter.GetDestinationForPlant(height);
                var item = new Plant
                {
                    Id = plant,
                    Destination = destination,
                    Height = height
                };
                result.Add(item);
            }
            return result;
        }

        private static void ReportResults(List<Plant> plants)
        {
            var count = plants.Count(p => p.Destination == Destination.Left);
            var expectedCount = 300;
            if (count != expectedCount)
            {
                throw new Exception($"Expected {expectedCount} to Left, but was {count}");
            }
            var smallestPlants = plants.OrderBy(p => p.Height).Take(expectedCount);
            var sortedCorrectly = smallestPlants.Count(p => p.Destination == Destination.Left);
            WriteLine(ConsoleColor.Green, $"Sort quality: {sortedCorrectly}/{expectedCount}");
        }

        private static Random GetRandomizer(string[] args)
        {
            if (args.Length == 0)
                return new Random();
            return new Random(args[0].GetHashCode());
        }

        private static void WriteLine(ConsoleColor color, object text)
        {
            var oldcolor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = oldcolor;
        }
    }
}
