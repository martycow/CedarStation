using System;
using UnityEngine;

namespace Game.General
{
    [Serializable]
    public struct SerializableVector3 : IEquatable<SerializableVector3>
    {
        [SerializeField]
        public float x;
        
        [SerializeField]
        public float y;
        
        [SerializeField]
        public float z;

        public SerializableVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public bool Equals(SerializableVector3 other)
        {
            return Mathf.Approximately(x, other.x) && Mathf.Approximately(y, other.y) && Mathf.Approximately(z, other.z);
        }

        public override bool Equals(object obj)
        {
            return obj is SerializableVector3 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y, z);
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }

        public static implicit operator Vector3(SerializableVector3 serializableVector)
        {
            return serializableVector.ToVector3();
        }
        
        public static implicit operator SerializableVector3(Vector3 vector)
        {
            return new SerializableVector3(vector);
        }
    }
}