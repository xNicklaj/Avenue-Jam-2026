using System;
using System.Collections.Generic;

namespace TabbyStudios
{
    [Serializable]
    public class SerializableList<T>
    {
        public List<T> list;

        public T this[int i] => list[i];
    
        public SerializableList(List<T> list)
        {
            this.list = list;
        }
    
        public SerializableList()
        {
            list = new();
        }
    }
}