using System;
using UnityEngine;

namespace Game.General
{
    public struct SerializableQuaternion : IEquatable<SerializableQuaternion>
    {
        [SerializeField] 
        public float x;

        [SerializeField] 
        public float y;

        [SerializeField] 
        public float z;

        [SerializeField] 
        public float w;

        public SerializableQuaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public SerializableQuaternion(Quaternion quaternion)
        {
            x = quaternion.x;
            y = quaternion.y;
            z = quaternion.z;
            w = quaternion.w;
        }

        public Quaternion ToQuaternion()
        {
            return new Quaternion(x, y, z, w);
        }

        public bool Equals(SerializableQuaternion other)
        {
            return Mathf.Approximately(x, other.x) && Mathf.Approximately(y, other.y) &&
                   Mathf.Approximately(z, other.z) && Mathf.Approximately(w, other.w);
        }

        public override bool Equals(object obj)
        {
            return obj is SerializableQuaternion other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y, z, w);
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z}, {w})";
        }

        public static implicit operator Quaternion(SerializableQuaternion serializableQuaternion)
        {
            return serializableQuaternion.ToQuaternion();
        }

        public static implicit operator SerializableQuaternion(Quaternion quaternion)
        {
            return new SerializableQuaternion(quaternion);
        }
    }
}