using System;
using UnityEngine;

namespace Game.General
{
    public static partial class Const
    {
        public static class Scope
        {
            public const string ApplicationScope = "Application";
            public const string GameplayScope = "Gameplay";
            public const string MenuScope = "Menu";
        }

        public static class Physics
        {
            public static readonly int PlayerHitLayer = LayerMask.NameToLayer("PlayerHit");
            public static readonly int GroundLayer = LayerMask.NameToLayer("Ground");
        }

        public static class Level
        {
            public static Guid DefaultLevelID = new Guid("2b9defa2-cdbb-4da0-ae42-b925aff76fab");
        }

        public static class Save
        {
            public const int MaxSlots = 3;
            public const string SaveDataPlayerPrefsKey = "CedarStationSaveData";
        }

        public static class Character
        {
            public static class BlendShapes
            {
                public const string HoodieOff =  "Hoodie_Off";
                
                public const string Mad = "Exp_Mad";
                public const string Sad = "Exp_Sad";
                public const string Happy = "Exp_Smile";
            }

            public static class AnimationParameters
            {
                public static readonly int MoveInputX = Animator.StringToHash("MoveInputX");
                public static readonly int MoveInputY = Animator.StringToHash("MoveInputY");
                
                public static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
                public static readonly int VerticalVelocity = Animator.StringToHash("VerticalVelocity");
                
                public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
                public static readonly int IsCrouching  = Animator.StringToHash("IsCrouching");
                public static readonly int IsPlayingDead = Animator.StringToHash("IsPlayingDead");
                
                public static readonly int Jump = Animator.StringToHash("Jump");
                public static readonly int Land = Animator.StringToHash("Land");
                public static readonly int Hide = Animator.StringToHash("Hide");
                public static readonly int Interact = Animator.StringToHash("Interact");
                public static readonly int Detected = Animator.StringToHash("Detected");
                public static readonly int Died = Animator.StringToHash("Died");

                public static readonly int ItemHoldType = Animator.StringToHash("ItemHoldType");
            }
        }
    }
}