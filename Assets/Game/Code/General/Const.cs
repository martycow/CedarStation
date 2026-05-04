using System;
using UnityEngine;

namespace Game.General
{
    public static class Const
    {
        public static class Main
        {
            public const string ApplicationScene = "Application";
            public const string GameplayScene = "Gameplay";
            public const string MenuScene = "Menu";
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
                public static readonly int Move = Animator.StringToHash("MoveSpeed");
                public static readonly int MoveX = Animator.StringToHash("MoveSpeedX");
                public static readonly int MoveZ = Animator.StringToHash("MoveSpeedZ");
                public static readonly int Jump = Animator.StringToHash("Jump");
                public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
                public static readonly int Fall = Animator.StringToHash("Falling");
                public static readonly int JumpFall = Animator.StringToHash("JumpFalling");
            }
        }
    }
}