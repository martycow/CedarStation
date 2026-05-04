using UnityEngine;

namespace Game.General
{
    [AddComponentMenu("Cedar/Volume/Volume Sphere")]
    public sealed class VolumeSphere : BaseView<VolumeData>
    {
        [SerializeField]
        private SphereCollider sphereCollider;
        
        protected override void Init()
        {
            switch (Context.SubType)
            {
                case VolumeShape.Sphere:
                    if (sphereCollider == null)
                        sphereCollider = gameObject.AddComponent<SphereCollider>();
                    
                    sphereCollider.isTrigger = Context.IsTrigger;
                    sphereCollider.name = $"collider_volume_{Context.SubType}_{Context.TechName}";
                    break;
            }
        }

        public override void UpdateView()
        {
            if (sphereCollider == null)
                return;
            
            transform.position = Context.Center;
            sphereCollider.center = Context.Center;
            sphereCollider.radius = Context.Size.ToVector3().magnitude / 2f;
        }
    }
}