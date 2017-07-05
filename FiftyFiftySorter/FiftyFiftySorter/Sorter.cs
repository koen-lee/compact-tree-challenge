using System;
using System.Collections.Generic;

namespace FiftyFiftySorter
{
    class Sorter
    {
        private int _smallPlantsLeft = 50;
        private readonly List<int> _heights = new List<int>();

        public Destination GetDestinationForPlant(int height)
        {
            Destination result;
            if (_smallPlantsLeft == 0 || _heights.Count == 0)
                result = Destination.Right;
            else
            {
                var plantsToDo = 100 - _heights.Count;
                if (plantsToDo == _smallPlantsLeft)
                    result = Destination.Left;
                else
                {
                    var quantile = _smallPlantsLeft / (double)plantsToDo;
                    var borderIndex = (int)(quantile * (_heights.Count - 1));
                    var borderValue = _heights[borderIndex];
                    result = height > borderValue ? Destination.Right : Destination.Left;
                }
            }

            _heights.Add(height);
            _heights.Sort();

            if (result == Destination.Left)
                _smallPlantsLeft--;
            return result;
        }
    }
}