using UnityEngine;

namespace Game.General
{
    [AddComponentMenu("Cedar/Volume/Volume Box")]
    public sealed class VolumeBox : BaseView<VolumeData>
    {
        [SerializeField]
        private BoxCollider boxCollider;
        
        protected override void Init()
        {
            switch (Context.SubType)
            {
                case VolumeShape.Box:
                    if (boxCollider == null)
                        boxCollider = gameObject.AddComponent<BoxCollider>();
                    
                    boxCollider.isTrigger = Context.IsTrigger;
                    boxCollider.name = $"collider_volume_{Context.SubType}_{Context.TechName}";
                    break;
            }
        }

        public override void UpdateView()
        {
            if (boxCollider == null)
                return;
            
            transform.position = Context.Center;
            boxCollider.center = Context.Center;
            boxCollider.size = Context.Size;
        }
    }
}