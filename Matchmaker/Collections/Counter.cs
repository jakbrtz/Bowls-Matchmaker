using System.Collections;
using System.Collections.Generic;

namespace Matchmaker.Collections
{
    public class Counter<TKey> : IEnumerable<KeyValuePair<TKey, int>>
    {
        readonly Dictionary<TKey, int> counts = new Dictionary<TKey, int>();

        public int this[TKey key]
        {
            get => counts.TryGetValue(key, out int value) ? value : 0;
            set => counts[key] = value;
        }

        public IEnumerator<KeyValuePair<TKey, int>> GetEnumerator()
        {
            return counts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TKey key, int value) { this[key] = value; }
    }
}
