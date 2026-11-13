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

        private float _lastJumpTime = float.NegativeInfinity;
        private bool _jumpRequested;
        private bool _isGrounded;
        private bool _jumpAnimationPending;

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
            if (_jumpAnimationPending)
            {
                animator.SetTrigger(Const.Character.AnimationParameters.Jump);
                _jumpAnimationPending = false;
            }

            var speed = Context.Motion.Value;

            animator.SetFloat(Const.Character.AnimationParameters.MoveInputX, speed.x);
            animator.SetFloat(Const.Character.AnimationParameters.MoveInputY, speed.y);

            var velocityMagnitude = rigidBody.linearVelocity.magnitude;
            var verticalVelocity = rigidBody.linearVelocity.y;

            animator.SetFloat(Const.Character.AnimationParameters.MoveSpeed, velocityMagnitude);
            animator.SetFloat(Const.Character.AnimationParameters.VerticalVelocity, verticalVelocity);
            animator.SetBool(Const.Character.AnimationParameters.IsGrounded, _isGrounded);
        }
        
        protected override void FixedUpdate()
        {
            if (Context == null)
                return;

            _isGrounded = CheckIsGrounded();
            var force = Context.Motion.Value.ToVector3();
            if (force != Vector3.zero)
            {
                rigidBody.AddForce(force, ForceMode.VelocityChange);
                DebugTools.DrawArrow(rigidBody.position, rigidBody.position + force, Color.cyan);
            }

            // Jumping
            if (!_jumpRequested)
                return;
            
            _jumpRequested = false;
            if (!_isGrounded || _lastJumpTime + Context.JumpCooldown.Value > Time.fixedTime)
                return;

            _lastJumpTime = Time.fixedTime;
            rigidBody.AddForce(Vector3.up * Context.JumpForce.Value, ForceMode.Impulse);
            _isGrounded = false;
            _jumpAnimationPending = true;
        }

        protected override void OnDisable()
        {
            _jumpAnimationPending = false;
            _jumpRequested = false;
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
            var origin = groundCollider.transform.TransformPoint(groundCollider.center);
            var scale = groundCollider.transform.lossyScale;
            var radius = groundCollider.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));

            var overlapCount = Physics.OverlapSphereNonAlloc(origin, radius, _overlappingGroundColliders, Const.Physics.GroundLayer, QueryTriggerInteraction.Ignore);
            if (overlapCount == 0)
                return false;

            var foundGround = false;
            for (var i = 0; i < overlapCount; i++)
            {
                var other = _overlappingGroundColliders[i];
                if (other == null || other.attachedRigidbody == rigidBody)
                    continue;

                foundGround = true;
                Debug.DrawLine(origin, other.ClosestPoint(origin), Color.gold, 0.1f);
                DebugTools.DrawCircle(origin, radius, Color.gold, 16, 0.1f);
            }

            return foundGround;
        }

        private void OnJumpRequested()
        {
            if (isActiveAndEnabled)
                _jumpRequested = true;
        }
    }
}
