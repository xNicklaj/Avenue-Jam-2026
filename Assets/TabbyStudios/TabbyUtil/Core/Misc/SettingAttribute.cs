using System;

namespace TabbyStudios
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SettingAttribute : Attribute
    {
        public object defaultValue;
    
        public SettingAttribute(object defaultValue)
        {
            this.defaultValue = defaultValue;
        }
        
    }
}