using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable UnusedType.Global

namespace TabbyStudios
{
    
    public class SerializedSetting<T> : VisualComponent
    {
        public INotifyValueChanged<T> t;
        public virtual string setting => target.Name().Uncapitalize();

        protected bool isWaitingToApply;
        protected T queuedValue;

        public override void Start()
        {
            t = GetT();
            t.value = Config.Subscribe<T>(setting, OnExternalChange);
            target.RegisterCallback<ChangeEvent<T>>(OnChange);
        }
        
        public virtual void OnChange(ChangeEvent<T> e)
        {
            queuedValue = e.newValue;
            if(!isWaitingToApply)
                EditorApplication.delayCall += ApplyChanges;
            
            isWaitingToApply = true;
        }
        
        protected virtual void ApplyChanges()
        {
            if (!isWaitingToApply)
                return;
            
            if (target?.focusController?.focusedElement != target)
            {
                T validated = Validate(queuedValue);
                Config.SetSetting(setting,validated);
                isWaitingToApply = false; 
            }
            else
            {
                EditorApplication.delayCall += ApplyChanges;
            }
        }
        
        public override void OnBeforeDisable()
        {
            EditorApplication.delayCall -= ApplyChanges;
            ApplyChanges();
        }
        
        private void OnExternalChange(T newValue)
        {
            t.SetValueWithoutNotify(newValue);
        }
        
        protected virtual T Validate(T value)
        {
            return value;
        }

        private INotifyValueChanged<T> GetT()
        {
            var result = target.Where(c => c.GetComponent<ValueChanger>() is not null).FirstOrDefault() as INotifyValueChanged<T>;
            //Assert.NotNull(result, $"Broken ValueChanger at {target.name}");
            return result;
        }
    
    }

    public class BoolSetting : SerializedSetting<bool>
    {
        public override void OnChange(ChangeEvent<bool> e)
        {
            Config.SetSetting(setting, e.newValue);
        }
    }
    
    public class FloatSetting : SerializedSetting<float> { }
    public class StringSetting : SerializedSetting<string> { }

    public class IntSetting : SerializedSetting<int>
    {
        private int min, max;
        
        public IntSetting()
        {

        }
        
        public IntSetting(object min, object max)
        {
            this.min = int.Parse((string)min);
            this.max = int.Parse((string)max);
        }
        
        protected override int Validate(int value)
        {
            return max > min ? Mathf.Clamp(value, min, max) : value;
        }
    }

    public class SliderSetting : SerializedSetting<float>
    {
        public override void OnChange(ChangeEvent<float> e)
        {
            Config.SetSetting(setting, e.newValue);
        }
        
    }

}