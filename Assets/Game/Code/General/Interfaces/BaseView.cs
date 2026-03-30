using System;
using UnityEngine;

namespace Game.General
{
    [Flags]
    public enum ViewUpdateType
    {
        Manually = 0,
        OnSetup = 1 << 0,
        EveryFrame = 1 << 1,
        EveryFixedUpdate = 1 << 2,
    }
    
    public abstract class BaseView<TContext> : MonoBehaviour where TContext: class
    {
        public TContext Context { get; protected set; }
        
        private bool _isInitialized;
        private ViewUpdateType _updateType;
        
        public virtual void Setup(TContext context,  ViewUpdateType updateType)
        {
            Context = context;
            _updateType = updateType;

            if (!_isInitialized)
            {
                Init();
                _isInitialized = true;
            }

            if (_updateType.HasFlag(ViewUpdateType.OnSetup))
                UpdateView();
        }

        public abstract void UpdateView();
        
        protected virtual void Init() { }

        protected virtual void Awake() { }

        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        
        protected virtual void Update()
        {
            if (!_isInitialized)
                return;

            if (_updateType.HasFlag(ViewUpdateType.EveryFrame))
                UpdateView();
        }

        protected virtual void FixedUpdate()
        {
            if (!_isInitialized)
                return;
            
            if (_updateType.HasFlag(ViewUpdateType.EveryFixedUpdate))
                UpdateView();
        }
        protected virtual void OnDestroy() { }
    }
}