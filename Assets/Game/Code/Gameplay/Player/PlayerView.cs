using Game.General;
using UnityEngine;

namespace Game.Gameplay
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerView : BaseView<PlayerContext>
    {
        [SerializeField, AutoAssign] private CharacterController characterController;
        [SerializeField, AutoAssign] private Animator animator;
        [SerializeField] private CharacterRenderers character;

        public bool IsGrounded { get; private set; }
        
        private int _hoodieOffIndex;

        private float lastJumpTime;
        
        protected override void Init()
        {
            base.Init();
            
            _hoodieOffIndex = character.OutfitJacket.sharedMesh.GetBlendShapeIndex("Hoodie_Off");
        }

        public override void UpdateView()
        {
            if (Context.MoveInput == Vector2.zero)
            {
                animator.SetFloat(Const.CharacterAnimator.Move, 0f);
            }
            else
            {
                var moveInput = Context.MoveInput.ToVector3();
                var motion = moveInput * (Context.MoveSpeed * Time.deltaTime);
            
                characterController.Move(motion);
                animator.SetFloat(Const.CharacterAnimator.Move, motion.magnitude);
            
                Utilities.DebugTools.DrawArrow(transform.position, transform.position + motion, Color.green);
            }

            if (Context.JumpInput && Time.time - lastJumpTime > Context.JumpCooldown)
            {
                characterController.Move(Vector3.up * (Context.JumpForce * Time.deltaTime));
                animator.SetTrigger(Const.CharacterAnimator.Jump);
                lastJumpTime = Time.time;
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            IsGrounded = characterController.isGrounded;
            animator.SetBool(Const.CharacterAnimator.IsGrounded, IsGrounded);
        }

        public void TurnOnHoodie()
        {
            character.OutfitJacket.SetBlendShapeWeight(_hoodieOffIndex, 0f);
        }

        public void TurnOffHoodie()
        {
            character.OutfitJacket.SetBlendShapeWeight(_hoodieOffIndex, 100f);
        }
    }
}