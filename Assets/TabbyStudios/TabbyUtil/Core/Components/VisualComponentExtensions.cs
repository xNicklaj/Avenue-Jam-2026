using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public static class VisualComponentExtensions
    {
        private static Type cachedEditorRootType;
    
        private static Dictionary<VisualElement, List<VisualComponent>> elementMap = new();
        private static Dictionary<VisualElement, AbstractScreenSpace> rootSpaces = new();

        public static Dictionary<string, List<Func<VisualComponent>>> typeDict;
        
        static VisualComponentExtensions()
        {
            typeDict = TypeCache.GetTypesDerivedFrom<FastCacheSearch>().First(t => t.Name == "ComponentMap").GetFieldValue<Dictionary<string, List<Func<VisualComponent>>>>("components");
        }
    
        public static bool IsRoot(this VisualElement e)
        {
            return e.IsEditorRoot();
        }

        private static bool IsEditorRoot(this VisualElement e)
        {
            return e is not null && e.name.StartsWith("rootVisualContainer");
        }
        
        public static VisualElement Root(this VisualElement e)
        {
            return e is null ? null : e.IsRoot() ? e : Root(e.parent);
        }
        
        public static VisualElement NullParent(this VisualElement e)
        {
            return e.parent is null ? e : NullParent(e.parent);
        }
        
        public static VisualElement Topmost(this VisualElement e)
        {
            return e.parent is null || e.IsRoot() ? e : Topmost(e.parent);
        }
        
        public static bool IsEditorElement(this VisualElement e)
        {
            return e.Root().IsEditorRoot();
        }

        public static AbstractScreenSpace ContainingSpace(this VisualElement e)
        {
            return rootSpaces.ContainsKey(e) ? rootSpaces[e] : ContainingSpace(e.parent);
        }
        
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private static void InitComponent(VisualComponent t, VisualElement parent)
        {
            t.target = parent;
            SaveComponent(parent, t);
            t.Awake();
        }
        
        public static T AddComponent<T>(this VisualElement e) where T : VisualComponent
        {
            var t = (T)FastActivator.CreateInstance(typeof(T));
            InitComponent(t, e);
            return t;
        }
    
        public static T AddComponent<T>(this VisualElement e, params object[] args) where T : VisualComponent
        {
            var t = (T)FastActivator.CreateInstance(typeof(T),args);
            InitComponent(t, e);
            return t;
        }
    
        public static VisualComponent AddComponent(this VisualElement e, VisualComponent t)
        {
            InitComponent(t, e);
            return t;
        }
        
        public static void RemoveComponent(this VisualElement e, VisualComponent t)
        {
            if (elementMap.ContainsKey(e))
            {
                elementMap[e].Remove(t);
            }
        }
        
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        public static VisualElement Create(this VisualTreeAsset tree)
        {
            var templateContainer = tree.Instantiate();
            var elem = templateContainer.Children().First();
            for (int i = 0; i < templateContainer.styleSheets.count; i++)
            {
                elem.styleSheets.Add(templateContainer.styleSheets[i]);
            }
            
            var a = elem.ForEach(CreateComponents);
            return a;
        }
        
        public static void RemoveTemplateContainers(this VisualElement root)
        {
            if (root == null) return;
        
            for (int i = root.childCount - 1; i >= 0; i--)
            {
                var child = root[i];
            
                if (child is TemplateContainer)
                {
                    var children = new List<VisualElement>();
                    while (child.childCount > 0)
                    {
                        children.Add(child[0]);
                        child.RemoveAt(0);
                    }
                
                    root.RemoveAt(i);
                
                    for (int j = 0; j < children.Count; j++)
                        root.Insert(i + j, children[j]);
                
                    foreach (var grandchild in children)
                        RemoveTemplateContainers(grandchild);
                }
                else
                {
                    RemoveTemplateContainers(child);
                }
            }
        }
        
        public static VisualElement Create(this VisualElement tree)
        {
            return tree.ForEach(CreateComponents);
        }
        
        public static void Embed(this VisualElement e, AbstractScreenSpace space)
        {
            rootSpaces[e] = space;
        }

        private static void RemoveSelf(this TemplateContainer container)
        {
            var containerParent = container.parent;
            if (containerParent is null)
                return;
            
            containerParent.Remove(container);
            var containerChildren = container.Children().ToList();
            foreach (VisualElement child in containerChildren)
            {
                container.Remove(child);
                containerParent.Add(child);
            }
        }
    
        private static void CreateComponents(VisualElement e)
        {
            AddComponentsFromName(e);
        }
        
        private static void AddComponentsFromName(VisualElement e)
        {
            if (e.name.IsNullOrEmpty() || !e.name.Contains("-"))
                return;
            
            var typeList = typeDict.GetValueOrDefault(e.name);
            if (typeList is null)
            {
                return;
            }
            foreach (var type in typeList)
            {
                //var comp = FastActivator.CreateInstance(type);
                //if (comp is VisualComponent c)
                //{
                    e.AddComponent(type());
                //}
            }
        }
        
        public static VisualElement AddElement(this VisualElement parent, VisualElement e)
        {
            parent.Add(e);
            e.TrickleAttach();
            if (e.Topmost().IsRoot())
                e.TrickleStart();
            
            return e;
        }
        
        public static T AddElement<T>(this VisualElement parent, T e) where T : VisualElement
        {
            return (T)AddElement(parent, (VisualElement)e);
        }
    
        public static void TrickleAttach(this VisualElement e)
        {
            e.Trickle(c => c.GetComponents().ForEach(comp => comp.Attach()));
        }   
    
        public static void TrickleStart(this VisualElement e)
        {
            e.Trickle(c => c.GetComponents().ForEach(comp => comp.Build()));
        }
        
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    
        public static List<VisualComponent> GetComponents(this VisualElement e)
        {
            if (elementMap.ContainsKey(e))
                return elementMap[e];
            return new();
        }
    
        public static T GetComponent<T>(this VisualElement e) where T : class
        {
            return (T)(object)GetComponents(e).FirstOrDefault(t => t.IsA<T>());
        }
        
        public static bool HasComponent<T>(this VisualElement e) where T : class
        {
            return e.GetComponents().Any(c => c is T);
        }
    
        public static T FindComponent<T>(this VisualElement e) where T : class
        {
            return Root(e).GetComponentDownwards<T>();
        }
    
        public static T GetComponentUpwards<T>(this VisualElement e) where T : class
        {
            return e.GetComponent<T>() ?? e.parent?.GetComponentUpwards<T>();
        }
    
        public static T GetComponentDownwards<T>(this VisualElement e) where T : class
        {
            var temp = e.GetComponent<T>();
            if (temp is not null)
                return temp;

            foreach (VisualElement child in e.Children())
            {
                temp = child.GetComponentDownwards<T>();
                if (temp is not null)
                    return temp;
            }

            return default;
        }    
    
        public static object GetComponent(this VisualElement e, Type type)
        {
            return GetComponents(e).FirstOrDefault(t => t.IsA(type));
        }
        
        public static object GetComponentUpwards(this VisualElement e, Type type)
        {
            return e.GetComponent(type) ?? e.parent?.GetComponentUpwards(type);
        }
    
        public static object GetComponentDownwards(this VisualElement e, Type type)
        {
            var temp = e.GetComponent(type);
            if (temp is not null)
                return temp;

            foreach (VisualElement child in e.Children())
            {
                temp = child.GetComponentDownwards(type);
                if (temp is not null)
                    return temp;
            }

            return default;
        }
    
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    
        private static void SaveComponent(VisualElement e, VisualComponent c)
        {
            if (!elementMap.ContainsKey(e))
                elementMap[e] = new();
            elementMap[e].Add(c);
        }
    
    
        public static void Trickle(this VisualElement e, Action<VisualElement> action)
        {
            action(e);
            foreach (var child in e.Children().ToList())
            {
                child.Trickle(action);
            }
        }
    
    
        public static string Name(this VisualElement e)
        {
            return e.name.Split("-")[0];
        }
    
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        public static void Pluck(this VisualElement e)
        {
            e.parent.Remove(e);
        }
    
        public static List<VisualComponent> GetAllComponents()
        {
            return elementMap.SelectMany(k => k.Value).ToList();
        }
        
            
        public static List<AbstractScreenSpace> GetAllSpaces()
        {
            return rootSpaces.Select(k => k.Value).ToList();
        }
        
        public static void Disable(this VisualElement e)
        {
            var comps = e.SelectComponent<VisualComponent>();
            foreach (var c in comps)
            {
                c.DisableSelfOnly();
            }
            
            e.RemoveMappings();
            e.Clear();
        }
        
        private static void RemoveMappings(this VisualElement e)
        {
            e.OnRemoveMappings();
            e.Children().ForEach(RemoveMappings);
        }

        private static void OnRemoveMappings(this VisualElement e)
        {
            if (elementMap.ContainsKey(e))
                elementMap.Remove(e);
            
            if (rootSpaces.ContainsKey(e))
                rootSpaces.Remove(e);
        }
    
        public static VisualElement ForEach(this VisualElement e, Action<VisualElement> action)
        {
            action(e);
            e.Children().ForEach(c => c.ForEach(action));
            return e;
        }
    
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static IEnumerable<VisualElement> Where(this VisualElement e, Func<VisualElement, bool> func)
        {
            return e.Query().Build().Where(func);
        }
        
        public static IEnumerable<VisualElement> Where(this VisualElement e)
        {
            return e.Query().Build().Where(c => true);
        }

        

    }
}