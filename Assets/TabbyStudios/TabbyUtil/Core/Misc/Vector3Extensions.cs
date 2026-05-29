using System;
using UnityEngine;
using static UnityEngine.Mathf;

namespace TabbyStudios
{
    public static class Vector3Extensions
    {
    
        //------------------------------------------------------------------------------------------------------------------
    
        public static Vector3 SetX(this Vector3 vector,float newX)
        {
            vector.x = newX;
            return vector;
        }
    
        public static Vector3 SetY(this Vector3 vector, float newY)
        {
            vector.y = newY;
            return vector;
        }

        public static Vector3 SetZ(this Vector3 vector, float newZ)
        {
            vector.z = newZ;
            return vector;
        }

        public static Vector3 SetXY(this Vector3 vector, float newX, float newY)
        {
            vector.x = newX;
            vector.y = newY;
            return vector;
        }

        public static Vector3 SetXZ(this Vector3 vector, float newX, float newZ)
        {
            vector.x = newX;
            vector.z = newZ;
            return vector;
        }

        public static Vector3 SetYZ(this Vector3 vector, float newY, float newZ)
        {
            vector.y = newY;
            vector.z = newZ;
            return vector;
        }
        
            
        //------------------------------------------------------------------------------------------------------------------
    
        public static Vector3 AddX(this Vector3 a, float x)
        {
            return new Vector3(a.x + x ,a.y, a.z);
        }
    
        public static Vector3 AddY(this Vector3 a, float y)
        {
            return new Vector3(a.x, a.y + y, a.z);
        }
        
        public static Vector3 AddZ(this Vector3 a, float f)
        {
            return new Vector3(a.x, a.y, a.z + f);
        }
    
        //------------------------------------------------------------------------------------------------------------------
    
        public static Vector3 SubX(this Vector3 a, float x)
        {
            return new Vector3(a.x - x , a.y, a.z);
        }
    
        public static Vector3 SubY(this Vector3 a, float y)
        {
            return new Vector3(a.x, a.y - y, a.z);
        }
        
        public static Vector3 Sub(this Vector3 a, float f)
        {
            return new Vector3(a.x, a.y, a.z-f);
        }
    
        //------------------------------------------------------------------------------------------------------------------
    
        //------------------------------------------------------------------------------------------------------------------
    
        public static Vector3 Div(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }
    
        //------------------------------------------------------------------------------------------------------------------
    
        public static Vector3 Mul(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }
    
        public static Vector3 MulX(this Vector3 a, float b)
        {
            return new Vector3(a.x * b, a.y, a.z);
        }
    
        public static Vector3 MulY(this Vector3 a, float b)
        {
            return new Vector3(a.x, a.y * b, a.z);
        }
    
        public static Vector3 MulZ(this Vector3 a,  float b)
        {
            return new Vector3(a.x, a.y, a.z * b);
        }
    
        //------------------------------------------------------------------------------------------------------------------

        public static Vector3 Apply(this Vector3 v, Func<float, float> f)
        { 
            return new Vector3(f(v.x),f(v.y),f(v.z));
        }
    
        public static Vector3 ApplyX(this Vector3 v, Func<float, float> f)
        { 
            return new Vector3(f(v.x),v.y,v.z);
        }
    
        public static Vector3 ApplyY(this Vector3 v, Func<float, float> f)
        { 
            return new Vector3(v.x,f(v.y),v.z);
        }
    
        public static Vector3 ApplyZ(this Vector3 v, Func<float, float> f)
        { 
            return new Vector3(v.x,v.y,f(v.z));
        }
    
        //------------------------------------------------------------------------------------------------------------------
    
        public static Vector3 Zip(this Vector3 v, Vector3 w, Func<float,float, float> f)
        { 
            return new Vector3(f(v.x,w.x),f(v.y,w.y),f(v.z,w.z));
        }
    
        //------------------------------------------------------------------------------------------------------------------

        public static bool IsClose(this Vector3 v, Vector3 v2)
        {
            return (v - v2).sqrMagnitude < 0.01f;
        }

        private static bool IsClose(float a, float b)
        {
            return Abs(a - b) < 0.01f;
        }
    
        public static bool IsCloseXZ(this Vector3 v, Vector3 v2)
        {
            return IsClose(v.x, v2.x) && IsClose(v.z, v2.z);
        }
    
        //------------------------------------------------------------------------------------------------------------------
    
        public static Vector3 xzy(this Vector3 v)
        { 
            return new Vector3(v.x,v.z,v.y);
        }
    
        public static Vector3 yxz(this Vector3 v)
        { 
            return new Vector3(v.y,v.x,v.z);
        }
    
        public static Vector3 yzx(this Vector3 v)
        { 
            return new Vector3(v.y,v.z,v.x);
        }
    
        public static Vector3 zxy(this Vector3 v)
        { 
            return new Vector3(v.z,v.x,v.y);
        }
    
        public static Vector3 zyx(this Vector3 v)
        { 
            return new Vector3(v.z,v.y,v.x);
        }
    
        //------------------------------------------------------------------------------------------------------------------
    
        public static Vector3 Uniform(this Vector3 v, float value)
        { 
            return new Vector3(value,value,value);
        }
    
        //------------------------------------------------------------------------------------------------------------------
    
        public static float L1Dist(this Vector3 v, Vector3 w)
        {
            return v.Zip(w, (vi, wi) => Abs(vi - wi)).Sum();
        }
    
        //------------------------------------------------------------------------------------------------------------------
    
        public static float Sum(this Vector3 v)
        {
            return v.x + v.y + v.z;
        }
        
    }
}
