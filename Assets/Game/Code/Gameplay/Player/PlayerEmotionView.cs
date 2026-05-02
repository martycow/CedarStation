using System;
using System.Collections.Generic;
using Game.General;
using UnityEngine;

namespace Game.Gameplay
{
    public class PlayerEmotionView : BaseView<CharacterEmotionContext>
    {
        [SerializeField]
        [Range(0f, 100f)]
        private float madness;
        
        [SerializeField]
        [Range(0f, 100f)]
        private float sadness;
        
        [SerializeField]
        [Range(0f, 100f)]
        private float happiness;
        
        public override void UpdateView()
        {
            Context.SetEmotion(EmotionType.Happy, happiness);
            Context.SetEmotion(EmotionType.Sad, sadness);
            Context.SetEmotion(EmotionType.Mad, madness);
        }
    }
}