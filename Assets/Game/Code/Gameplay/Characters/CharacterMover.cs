using Game.General;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class CharacterMover : ContextView<PlayerMovementContext>
    {
        [SerializeField, AutoAssign]
        private Rigidbody rigidBody;

        [SerializeField, AutoAssign]
        private SphereCollider groundCollider;
        
        [SerializeField, AutoAssign]
        private BoxCollider hitCollider;
        
        [SerializeField, AutoAssign] 
        private Animator animator;
        
        private readonly Collider[] _overlappingGroundColliders = new Collider[4];
        
        private float _lastJumpTime;

        protected override void Init()
        {
            if (Context == null)
                return;

            InitColliders();
            
            Context.OnJumpRequested += OnJumpRequested;
        }

        protected override void OnDestroy()
        {
            if (Context == null)
                return;
            
            Context.OnJumpRequested -= OnJumpRequested;
        }

        protected override void UpdateView()
        {
            var speed = Context.Motion.Value;
            
            animator.SetFloat(Const.Character.AnimationParameters.MoveInputX, speed.x);
            animator.SetFloat(Const.Character.AnimationParameters.MoveInputY, speed.y);

            var force = speed.ToVector3();
            if (force != Vector3.zero)
            {
                rigidBody.AddForce(force, ForceMode.VelocityChange);
                DebugTools.DrawArrow(rigidBody.position, rigidBody.position + force, Color.cyan);
            }

            var velocityMagnitude = rigidBody.linearVelocity.magnitude;
            var verticalVelocity = rigidBody.linearVelocity.y;
            var isGrounded = CheckIsGrounded();
            
            animator.SetFloat(Const.Character.AnimationParameters.MoveSpeed, velocityMagnitude);
            animator.SetFloat(Const.Character.AnimationParameters.VerticalVelocity, verticalVelocity);
            animator.SetBool(Const.Character.AnimationParameters.IsGrounded, isGrounded);
        }
        
        private void InitColliders()
        {
            // Hit
            hitCollider.center = new Vector3(0f, Context.Height.Value / 2f, 0.15f);
            hitCollider.size = new Vector3(0.4f, Context.Height.Value, 0.4f);
            hitCollider.isTrigger = true;
            
            // Ground check
            groundCollider.center = new Vector3(0f, 0.35f, 0.2f);
            groundCollider.radius = 0.4f;
            groundCollider.isTrigger = false;
        }

        private bool CheckIsGrounded()
        {
            var origin = groundCollider.center;
            var radius = groundCollider.radius;
            
            var overlapCount = Physics.OverlapSphereNonAlloc(origin, radius, _overlappingGroundColliders, Const.Physics.GroundLayer, QueryTriggerInteraction.Ignore);
            if (overlapCount == 0) 
                return false;
            
            for (var i = 0; i < overlapCount; i++)
            {
                var other = _overlappingGroundColliders[i];
                if (other == null) 
                    continue;
                    
                Debug.DrawLine(origin, other.ClosestPoint(origin), Color.gold, 0.1f);
                DebugTools.DrawCircle(origin, radius, Color.gold, 16, 0.1f);
            }

            return true;
        }
        
        private void OnJumpRequested()
        {
            if (_lastJumpTime + Context.JumpCooldown.Value > Time.time)
                return;
            
            _lastJumpTime = Time.time;
            rigidBody.AddForce(Vector3.up * Context.JumpForce.Value, ForceMode.Impulse);
            animator.SetTrigger(Const.Character.AnimationParameters.Jump);
        }
    }
}