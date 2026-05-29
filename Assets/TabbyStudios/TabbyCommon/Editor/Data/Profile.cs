using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace TabbyStudios
{
    [Serializable]
    public class Profile : ISerializationCallbackReceiver
    {
        public string version;
        public List<ItemData> list;
        public ProfileVersion profileVersion;
        
        public Profile(List<ItemData> list, string version)
        {
            this.list = list;
            SetVersion(version);
        }

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            if (version.IsNullOrEmpty())
                version = "1.0.0";
            
            profileVersion = new ProfileVersion(version);
        }

        public void SetVersion(string version)
        {
            this.version = version;
            this.profileVersion = new ProfileVersion(version);
        }
    }

    public class ProfileVersion : IComparable<ProfileVersion>
    {
        private readonly int[] versionParts;
        private readonly string version;

        public ProfileVersion(string version)
        {
            this.version = version;
            if (string.IsNullOrEmpty(version))
            {
                versionParts = new[] { 0 };
                return;
            }

            string[] parts = version.Split('.');
            versionParts = new int[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                if (int.TryParse(parts[i], out int result))
                {
                    versionParts[i] = result;
                }
                else
                {
                    versionParts[i] = 0;
                }
            }
        }

        public int CompareTo(ProfileVersion other)
        {
            Assert.IsNotNull(other);
            
            int maxLength = Math.Max(versionParts.Length, other.versionParts.Length);

            for (int i = 0; i < maxLength; i++)
            {
                int thisPart = i < versionParts.Length ? versionParts[i] : 0;
                int otherPart = i < other.versionParts.Length ? other.versionParts[i] : 0;

                if (thisPart > otherPart) return 1;
                if (thisPart < otherPart) return -1;
            }

            return 0;
        }

        public override string ToString()
        {
            return version;
        }
        
        public static bool operator >(ProfileVersion a, string b) => a.CompareTo(new ProfileVersion(b)) > 0;
        public static bool operator <(ProfileVersion a, string b) => a.CompareTo(new ProfileVersion(b)) < 0;
        public static bool operator >=(ProfileVersion a, string b) => a.CompareTo(new ProfileVersion(b)) >= 0;
        public static bool operator <=(ProfileVersion a, string b) => a.CompareTo(new ProfileVersion(b)) <= 0;
        public static bool operator ==(ProfileVersion a, string b) => a == new ProfileVersion(b);
        public static bool operator !=(ProfileVersion a, string b) => !(a == new ProfileVersion(b));
        
        public static bool operator >(string b, ProfileVersion a) => a.CompareTo(new ProfileVersion(b)) > 0;
        public static bool operator <(string b, ProfileVersion a) => a.CompareTo(new ProfileVersion(b)) < 0;
        public static bool operator >=(string b, ProfileVersion a) => a.CompareTo(new ProfileVersion(b)) >= 0;
        public static bool operator <=(string b, ProfileVersion a) => a.CompareTo(new ProfileVersion(b)) <= 0;
        public static bool operator ==(string b, ProfileVersion a) => a == new ProfileVersion(b);
        public static bool operator !=(string b, ProfileVersion a) => !(a == new ProfileVersion(b));

        public static bool operator >(ProfileVersion a, ProfileVersion b) => a.CompareTo(b) > 0;
        public static bool operator <(ProfileVersion a, ProfileVersion b) => a.CompareTo(b) < 0;
        public static bool operator >=(ProfileVersion a, ProfileVersion b) => a.CompareTo(b) >= 0;
        public static bool operator <=(ProfileVersion a, ProfileVersion b) => a.CompareTo(b) <= 0;
        public static bool operator ==(ProfileVersion a, ProfileVersion b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.CompareTo(b) == 0;
        }
        public static bool operator !=(ProfileVersion a, ProfileVersion b) => !(a == b);

        public override bool Equals(object obj)
        {
            if (obj is ProfileVersion other)
                return CompareTo(other) == 0;
            return false;
        }

        public override int GetHashCode()
        {
            var s = version;
            while (s.EndsWith(".0"))
            {
                s = s.RemoveTrailing(".0");
            }
            return s.GetHashCode();
        }
    }
}