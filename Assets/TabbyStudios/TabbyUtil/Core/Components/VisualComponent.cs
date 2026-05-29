using UnityEngine;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public class VisualComponent : ITransform
    {
        public VisualElement target;
        public bool didGeometry;

        public Vector3 position
        {
            get => target.transform.position;
            set => value = target.transform.position;
        }

        public Quaternion rotation
        {
            get => target.transform.rotation;
            set => value = target.transform.rotation;
        }

        public Vector3 scale
        {
            get => target.transform.scale;
            set => value = target.transform.scale;
        }

        public Matrix4x4 matrix => target.transform.matrix;
        
        protected bool built,attached;
    
        public virtual void Awake()
        {
            //Awake is called when this is added to target
        }
    
        public virtual void OnAttach()
        {
            //OnAttach is called when target is added to a parent
        }
    
        public virtual void Start()
        {
            //Start is called when an ancestor of target (or target itself) is added to the root
            //If an ancestor is already attached to the root Start is called after OnAttach is called on every component that is connected to this
        }
        
        public virtual void OnDisable()
        {
            //OnDisable is called when OnDisable is called on the parent window
        }
        
        public virtual void OnBeforeDisable()
        {
            //OnBeforeDisable is called before OnDisable is called on any connect component
        }
    
    
        public void Attach()
        {
            if (attached)
                return;
        
            OnAttach();
            attached = true;
        }
    
    
        public void Build()
        {
            if (built)
                return;
        
            Start();
            built = true;
        }
    
        public T GetComponent<T>() where T : VisualComponent
        {
            return target.GetComponent<T>();
        }
    
        public void Hide()
        {
            target.SetVisible(false);
        }

        public void Show()
        {
            target.SetVisible(true);
        }

        public virtual void SetVisible(bool visible)
        {
            target.SetVisible(visible);
        }

        private void RemoveCallbacks()
        {
            UnregisterGeometryChanged();
            UnregisterMouseDown();
            UnregisterMouseEnter();
            UnregisterMouseLeave();
            UnregisterMouseMove();
            UnregisterMouseUp();
            UnregisterWheel();
            UnregisterKeyDown();
        }
        
        public void DisableSelfOnly()
        {
            RemoveCallbacks();
            OnBeforeDisable();
            OnDisable();
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        public void RegisterGeometryChangedOnce()
        {
            target.RegisterCallback<GeometryChangedEvent>(PrivateOnGeometryChangedOnce);
        }
        
        public void RegisterGeometryChanged()
        {
            target.RegisterCallback<GeometryChangedEvent>(PrivateOnGeometryChanged);
        }
        
        public void RegisterMouseDown()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }
        
        public void RegisterMouseUp()
        {
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }
        
        public void RegisterMouseEnter()
        {
            target.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        }
        
        public void RegisterMouseLeave()
        {
            target.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }
        
        public void RegisterMouseMove()
        {
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        }
        
        public void RegisterWheel()
        {
            target.RegisterCallback<WheelEvent>(OnWheel);
        }
        
        public void RegisterKeyDown()
        {
            target.RegisterCallback<KeyDownEvent>(OnKeyDown);
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        public void UnregisterGeometryChanged()
        {
            target.UnregisterCallback<GeometryChangedEvent>(PrivateOnGeometryChanged);
        }

        public void UnregisterMouseDown()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }

        public void UnregisterMouseUp()
        {
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        public void UnregisterMouseEnter()
        {
            target.UnregisterCallback<MouseEnterEvent>(OnMouseEnter);
        }

        public void UnregisterMouseLeave()
        {
            target.UnregisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }
        
        public void UnregisterMouseMove()
        {
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        }
        
        public void UnregisterWheel()
        {
            target.UnregisterCallback<WheelEvent>(OnWheel);
        }
        
        public void UnregisterKeyDown()
        {
            target.UnregisterCallback<KeyDownEvent>(OnKeyDown);
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        private void PrivateOnGeometryChanged(GeometryChangedEvent e)
        {
            OnGeometryChanged(e);
            didGeometry = true;
        }
        
        private void PrivateOnGeometryChangedOnce(GeometryChangedEvent e)
        {
            OnGeometryChanged(e);
            didGeometry = true;
            target.UnregisterCallback<GeometryChangedEvent>(PrivateOnGeometryChangedOnce);
        }
        
        public virtual void OnGeometryChanged(GeometryChangedEvent e)
        {
            
        }
        
        public virtual void OnMouseDown(MouseDownEvent e)
        {
            
        }
        
        public virtual void OnMouseUp(MouseUpEvent e)
        {
            
        }
        
        public virtual void OnMouseEnter(MouseEnterEvent e)
        {
            
        }
        
        public virtual void OnMouseLeave(MouseLeaveEvent e)
        {
            
        }
        
        public virtual void OnMouseMove(MouseMoveEvent e)
        {
            
        }
        
        public virtual void OnWheel(WheelEvent e)
        {
            
        }

        public virtual void OnKeyDown(KeyDownEvent e)
        {
            
        }
        
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
        
    }
}