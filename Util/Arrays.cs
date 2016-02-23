using System;

namespace Barbar.TreeDistance.Util
{
    public static class Arrays
    {
        // Note: start is inclusive, end is exclusive (as is conventional
        // in computer science)
        public static void Fill<T>(T[] array, T value)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }

        public static T[][] Allocate<T>(int firstDimension, int secondDimension)
        {
            var result = new T[firstDimension][];
            for (var i = 0; i < firstDimension; i++)
            {
                result[i] = new T[secondDimension];
            }
            return result;
        }
        public static T[][][] Allocate<T>(int firstDimension, int secondDimension, int thirdDimension)
        {
            var result = new T[firstDimension][][];
            for (var i = 0; i < firstDimension; i++)
            {
                result[i] = new T[secondDimension][];
                for (var j = 0; j < secondDimension; j++)
                {
                    result[i][j] = new T[thirdDimension];
                }
            }

            return result;
        }
    }
}
