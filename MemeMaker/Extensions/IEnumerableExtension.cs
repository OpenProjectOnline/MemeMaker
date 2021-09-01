using System.Collections.Generic;
using System.Linq;

namespace MemeMaker.Extensions
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int size) 
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / size)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}