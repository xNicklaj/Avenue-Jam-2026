using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public static class VisualComponentQueries
    {
        
        public static VisualElement First(this VisualElement e, string name)
        {
            return e.Query().Where(v => v.Name() == name).First();
        }
        
        public static List<VisualElement> WhereComponent<T>(this VisualElement e)
        {
            return e.Query().Where(v => v.GetComponents().Any(c => c.IsA<T>())).ToList();
        }
    
        public static VisualElement FirstComponent<T>(this VisualElement e)
        {
            return e.WhereComponent<T>().First();
        }
    
        public static VisualElement FirstOrDefaultComponent<T>(this VisualElement e)
        {
            return e.WhereComponent<T>().FirstOrDefault();
        } 
    
        public static List<T> SelectComponent<T>(this VisualElement e) where T : class
        {
            return e.WhereComponent<T>().SelectMany(v => v.GetComponents().OfType<T>()).ToList();
        }
    
        public static T SelectFirstComponent<T>(this VisualElement e) where T : class
        {
            return e.SelectComponent<T>().First();
        }
    
        public static T SelectFirstOrDefaultComponent<T>(this VisualElement e) where T : class
        {
            return e.SelectComponent<T>().FirstOrDefault();
        }
    
    }
}