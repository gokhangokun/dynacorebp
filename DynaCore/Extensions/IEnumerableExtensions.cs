using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DynaCore.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsEmpty<T>(this T[] source)
        {
            return source == null || !source.Any();
        }

        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        public static bool HasElements<T>(this T[] source)
        {
            return !IsEmpty(source);
        }

        public static bool HasElements<T>(this IEnumerable<T> source)
        {
            return !IsEmpty(source);
        }

        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> source, int size)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source", "source cannot be null.");
            }

            T[] array = null;
            int count = 0;

            foreach (T item in source)
            {
                if (array == null)
                {
                    array = new T[size];
                }

                array[count] = item;

                count++;

                if (count == size)
                {
                    yield return new ReadOnlyCollection<T>(array);
                    array = null;
                    count = 0;
                }
            }

            if (array != null)
            {
                Array.Resize(ref array, count);
                yield return new ReadOnlyCollection<T>(array);
            }
        }
    }

}