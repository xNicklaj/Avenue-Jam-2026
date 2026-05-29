using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TabbyStudios
{
    public class IdWrapper
    {
        private int instanceId;
        
        #if UNITY_6000_3_OR_NEWER
        private EntityId id;
        
        public IdWrapper(EntityId id)
        {
            this.id = id;
        }

        public static List<IdWrapper> Create(EntityId[] ids)
        {
            return ids.Select(id => new IdWrapper(id)).ToList();
        }

        public static List<IdWrapper> CreateFromSelection()
        {
            return Create(Selection.entityIds);
        }

        public static List<IdWrapper> Create(object[] ids)
        {
            return ids.Select(id => new IdWrapper((EntityId)id)).ToList();
        }

        public bool Eq(IdWrapper other)
        {
            return other.id == id;
        }

        public static implicit operator EntityId(IdWrapper w) => w.id; 
        
        #else
        public static List<IdWrapper> Create(object[] ids)
        {
            return ids.Select(id => new IdWrapper((int)id)).ToList();
        }

        public bool Eq(IdWrapper other)
        {
            return other.instanceId == instanceId;
        }

        public static List<IdWrapper> CreateFromSelection()
        {
            return Create(Selection.instanceIDs);
        }
        
        public static implicit operator int(IdWrapper w) => w.instanceId; 
        #endif
        
        public IdWrapper(int instanceId)
        {
            this.instanceId = instanceId;
        }

        public static List<IdWrapper> Create(int[] ids)
        {
            return ids.Select(id => new IdWrapper(id)).ToList();
        }
    }
}