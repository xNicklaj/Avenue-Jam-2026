using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TabbyStudios
{
    [Serializable]
    public class ConfigDictionary : SerializableDictionary<string>
    {
        public override void OnAfterDeserialize()
        {
            Init();
        }

        public void Init()
        {
            LoadDefaults();
            PasteLoadedValues();
        }

        private void LoadDefaults()
        {
            var defaults = GetDefaults();
            dict = new Dictionary<string, object>();
            foreach (var t in defaults)
            {
                dict[t.Key] = dict.GetValueOrDefault(t.Key, defaults[t.Key]);
            }
        }

        private void PasteLoadedValues()
        {
            base.OnAfterDeserialize();
        }

        public Dictionary<string,object> GetDefaults()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.StartsWith("Tabby"));
            var types = assemblies.SelectMany(a => a.GetTypes()).ToList();
            var fields = types.SelectMany(t => t.GetFieldInfos()).Where(f => f.GetCustomAttributes(typeof(SettingAttribute)).Any()).ToList();
            var tuples = fields.Select(f => (f,f.GetCustomAttributes<SettingAttribute>().First())).ToList();
            var result = new Dictionary<string,object>(tuples.Select(t => new KeyValuePair<string, object>(t.f.Name, t.Item2.defaultValue)));
            return result;
        }
    }
}