using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabbyStudios
{
    public class DictList<K,V> : IEnumerable<V>
    {
        private Dictionary<K,V> dict = new ();
        private Func<V,K> prop;
    
        public V this[K key] => dict[key];
    
        public DictList(Func<V,K> prop)
        {
            this.prop = prop;
        }
    
        public DictList(Func<V,K> prop, IEnumerable<V> list)
        {
            this.prop = prop;
            Set(list);
        }
    
        public void Set(IEnumerable<V> list)
        {
            this.dict = new Dictionary<K, V>(list.Select(v => new KeyValuePair<K, V>(prop(v), v)));
        }
    
        public void Add(V v)
        {
            dict[prop(v)] = v;
        }

        public void Clear()
        {
            dict.Clear();
        }

        public bool Contains(V value)
        {
            return dict.ContainsValue(value);
        }
        
        public bool ContainsKey(K key)
        {
            return dict.ContainsKey(key);
        }
        
        public IEnumerator<V> GetEnumerator()
        {
            return dict.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}