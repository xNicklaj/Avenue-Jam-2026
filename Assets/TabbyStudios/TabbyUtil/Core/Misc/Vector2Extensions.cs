using System;
using UnityEngine;

namespace TabbyStudios
{
    public static class Vector2Extensions
    {
        //------------------------------------------------------------------------------------------------------------------
    
        public static Vector2 SetX(this Vector2 vector,float x)
        {
            vector.x = x;
            return vector;
        }
    
        public static Vector2 SetY(this Vector2 vector, float y)
        {
            vector.y = y;
            return vector;
        }
    
        //------------------------------------------------------------------------------------------------------------------
    
        public static Vector2 AddX(this Vector2 a, float x)
        {
            return new Vector2(a.x + x ,a.y);
        }
    
        public static Vector2 AddY(this Vector2 a, float y)
        {
            return new Vector2(a.x, a.y + y);
        }
        
        public static Vector2 Add(this Vector2 a, float f)
        {
            return new Vector2(a.x + f ,a.y + f);
        }
    
        //------------------------------------------------------------------------------------------------------------------
    
        public static Vector2 SubX(this Vector2 a, float x)
        {
            return new Vector2(a.x - x ,a.y);
        }
    
        public static Vector2 SubY(this Vector2 a, float y)
        {
            return new Vector2(a.x, a.y - y);
        }
        
        public static Vector2 Sub(this Vector2 a, float f)
        {
            return new Vector2(a.x - f ,a.y - f);
        }
    
        //------------------------------------------------------------------------------------------------------------------
    
        public static Vector2 Mul(this Vector2 a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }
        
        public static Vector2 Mul(this Vector2 a, float f)
        {
            return new Vector2(a.x * f, a.y * f);
        }
    
        public static Vector2 MulX(this Vector2 a, float x)
        {
            return new Vector2(a.x * x, a.y);
        }
    
        public static Vector2 MulY(this Vector2 a, float y)
        {
            return new Vector2(a.x, a.y * y);
        }
    
        //------------------------------------------------------------------------------------------------------------------
    
        public static Vector2 Div(this Vector2 a, Vector2 b)
        {
            return new Vector2(a.x / b.x, a.y / b.y);
        }
        
        public static Vector2 Div(this Vector2 a, float f)
        {
            return new Vector2(a.x / f, a.y / f);
        }
    
        public static Vector2 DivX(this Vector2 a, float x)
        {
            return new Vector2(a.x / x, a.y);
        }

        public static Vector2 DivY(this Vector2 a, float y)
        {
            return new Vector2(a.x, a.y / y);
        }
    
        //------------------------------------------------------------------------------------------------------------------
    
        public static Vector2 Apply(this Vector2 a, Func<float, float> f)
        { 
            return new Vector2(f(a.x),f(a.y));
        }
        
        public static Vector2 ApplyX(this Vector2 a, Func<float, float> f)
        { 
            return new Vector2(f(a.x), a.y);
        }
        
        public static Vector2 ApplyY(this Vector2 a, Func<float, float> f)
        { 
            return new Vector2(a.x ,f(a.y));
        }
        
        //------------------------------------------------------------------------------------------------------------------
        
        public static Vector2 Clamp(this Vector2 e, float min, float max)
        {
            return new Vector2(Mathf.Clamp(e.x,min,max),Mathf.Clamp(e.y,min,max));
        }
    
        public static Vector2 Clamp(this Vector2 e, Vector2 min, Vector2 max)
        {
            return new Vector2(Mathf.Clamp(e.x,min.x,max.x),Mathf.Clamp(e.y,min.y,max.y));
        }
    
    }
}