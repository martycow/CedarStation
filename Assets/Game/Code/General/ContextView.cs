using System;
using UnityEngine;

namespace Game.General
{
    [Flags]
    public enum ContextViewUpdateType
    {
        Manually = 0,
        OnSetup = 1 << 0,
        EveryFrame = 1 << 1,
        EveryFixedUpdate = 1 << 2,
    }
    
    public abstract class ContextView<TContext> : MonoBehaviour where TContext: class
    {
        public TContext Context { get; private set; }
        
        public Transform Transform => transform;
        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;
        
        private bool _isInitialized;
        private ContextViewUpdateType _updateType;
        
        public virtual void Setup(TContext context, ContextViewUpdateType updateFlag)
        {
            Context = context;
            _updateType = updateFlag;

            if (!_isInitialized)
            {
                Init();
                _isInitialized = true;
            }

            if (_updateType.HasFlag(ContextViewUpdateType.OnSetup))
                UpdateView();
        }

        protected abstract void UpdateView();
        
        protected virtual void Init() { }
        protected virtual void Awake() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        protected virtual void OnDestroy() { }
        
        protected virtual void Update()
        {
            if (!_isInitialized)
                return;

            if (_updateType.HasFlag(ContextViewUpdateType.EveryFrame))
                UpdateView();
        }

        protected virtual void FixedUpdate()
        {
            if (!_isInitialized)
                return;
            
            if (_updateType.HasFlag(ContextViewUpdateType.EveryFixedUpdate))
                UpdateView();
        }
    }
}