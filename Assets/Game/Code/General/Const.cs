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

        public static class CharacterAnimator
        {
            public static readonly int Move = Animator.StringToHash("Move");
            public static readonly int Jump = Animator.StringToHash("Jump");
            public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        }
    }
}