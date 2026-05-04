using System;
using UnityEngine;

namespace Game.General
{
    [Serializable]
    public struct SerializableGuid : IEquatable<SerializableGuid>
    {
        [SerializeField] 
        private uint a;
        
        [SerializeField]
        private uint b;
        
        [SerializeField]
        private uint c;
        
        [SerializeField]
        private uint d;
        
        public SerializableGuid(Guid guid)
        {
            var bytes = guid.ToByteArray();
            a = BitConverter.ToUInt32(bytes, 0);
            b = BitConverter.ToUInt32(bytes, 4);
            c = BitConverter.ToUInt32(bytes, 8);
            d = BitConverter.ToUInt32(bytes, 12);
        }

        public Guid ToGuid()
        {
            var bytes = new byte[16];
            BitConverter.GetBytes(a).CopyTo(bytes, 0);
            BitConverter.GetBytes(b).CopyTo(bytes, 4);
            BitConverter.GetBytes(c).CopyTo(bytes, 8);
            BitConverter.GetBytes(d).CopyTo(bytes, 12);
            return new Guid(bytes);
        }

        public static SerializableGuid NewGuid()
        {
            return new SerializableGuid(Guid.NewGuid());
        }

        public static SerializableGuid Empty()
        {
            return new SerializableGuid(Guid.Empty);
        }

        public bool Equals(SerializableGuid other)
        {
            return a == other.a && b == other.b && c == other.c && d == other.d;
        }

        public override bool Equals(object obj)
        {
            return obj is SerializableGuid other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(a, b, c, d);
        }

        public override string ToString()
        {
            return ToGuid().ToString();
        }
        
        public static implicit operator Guid(SerializableGuid serializableGuid)
        {
            return serializableGuid.ToGuid();
        }
        
        public static implicit operator SerializableGuid(Guid guid)
        {
            return new SerializableGuid(guid);
        }
    }
}