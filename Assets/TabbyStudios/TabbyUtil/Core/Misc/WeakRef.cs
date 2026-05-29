using System;

namespace TabbyStudios
{
    public class WeakRef<T> where T : class
    {
        private readonly WeakReference<T> weakReference;
        public readonly int targetHash;

        public bool isAlive => target is not null;
        public T target => Target();
        
        public WeakRef(T target)
        {
            weakReference = new WeakReference<T>(target);
            targetHash = target.GetHashCode();
        }
    
        public WeakRef(WeakReference<T> weakReference)
        {
            this.weakReference = weakReference;
            targetHash = target.GetHashCode();
        }
        
        private T Target()
        {
            weakReference.TryGetTarget(out T target);
            return target;
        }
        
        public static implicit operator WeakRef<T>(T target)
        {
            return new WeakRef<T>(target);
        }
        
        public static implicit operator T(WeakRef<T> weakRef)
        {
            return weakRef.target;
        }
        
        public static implicit operator int(WeakRef<T> weakRef)
        {
            return weakRef.targetHash;
        }
    
        public static implicit operator WeakRef<T>(WeakReference<T> weakReference)
        {
            return new WeakRef<T>(weakReference);
        }
    
        public static implicit operator WeakReference<T>(WeakRef<T> weakRef)
        {
            return weakRef.weakReference;
        }
        
        public static bool operator==(WeakRef<T> a, WeakRef<T> b)
        {
            if (ReferenceEquals(a, b)) 
                return true;
    
            return a?.targetHash == b?.targetHash;
        }
        
        public static bool operator !=(WeakRef<T> a, WeakRef<T> b)
        {
            return !(a == b);
        }
    
        protected bool Equals(WeakRef<T> other)
        {
            return target == other.target;
        }
        
        protected bool Equals(int other)
        {
            return targetHash == other;
        }
    
        public override bool Equals(object obj)
        {
            if (obj is null) 
                return false;
    
            return targetHash == obj.GetHashCode();
    
        }
    
        public override int GetHashCode()
        {
            return targetHash;
        }
    }
}