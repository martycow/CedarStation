using System.Collections.Generic;
using Cedar.Core;
using Game.General;
using UnityEngine;

namespace Game.Gameplay
{
    public class CharacterEmotionContext
    {
        public readonly CharacterVisual Visual;
        public readonly ICedarLogger Logger;
        public readonly Dictionary<EmotionType, SkinnedMeshRenderer[]> EmotionRenderMap = new();
        public readonly Dictionary<EmotionType, int[]> EmotionBlendShapeMap = new();
        
        public EmotionType CurrentEmotion { get; private set; } = EmotionType.None;
        
        public CharacterEmotionContext(CharacterVisual visual, ICedarLogger logger)
        {
            Visual = visual;
            Logger = logger;

            BuildEmotionMap();
        }

        private void BuildEmotionMap()
        {
            if (Visual == null)
            {
                Logger.Error(SystemTag.Emotion, "Character Renderers is null");
                return;
            }
            
            EmotionRenderMap[EmotionType.Happy] = new[]
            {
                Visual.Face,
                Visual.Brows,
                Visual.TeethTop,
                Visual.TeethBottom,
                Visual.Tongue
            };
            
            EmotionBlendShapeMap[EmotionType.Happy] = new[]
            {
                Visual.Face.sharedMesh.GetBlendShapeIndex(Const.Character.BlendShapes.Happy),
                Visual.Brows.sharedMesh.GetBlendShapeIndex(Const.Character.BlendShapes.Happy),
                Visual.TeethTop.sharedMesh.GetBlendShapeIndex(Const.Character.BlendShapes.Happy),
                Visual.TeethBottom.sharedMesh.GetBlendShapeIndex(Const.Character.BlendShapes.Happy),
                Visual.Tongue.sharedMesh.GetBlendShapeIndex(Const.Character.BlendShapes.Happy)
            };
            
            EmotionRenderMap[EmotionType.Sad] = new[]
            {
                Visual.Face,
                Visual.Brows,
                Visual.TeethTop,
                Visual.TeethBottom,
                Visual.Tongue
            };
            
            EmotionBlendShapeMap[EmotionType.Sad] = new[]
            {
                Visual.Face.sharedMesh.GetBlendShapeIndex(Const.Character.BlendShapes.Sad),
                Visual.Brows.sharedMesh.GetBlendShapeIndex(Const.Character.BlendShapes.Sad),
                Visual.TeethTop.sharedMesh.GetBlendShapeIndex(Const.Character.BlendShapes.Sad),
                Visual.TeethBottom.sharedMesh.GetBlendShapeIndex(Const.Character.BlendShapes.Sad),
                Visual.Tongue.sharedMesh.GetBlendShapeIndex(Const.Character.BlendShapes.Sad)
            };
            
            EmotionRenderMap[EmotionType.Mad] = new[]
            {
                Visual.Face,
                Visual.Brows,
                Visual.TeethTop,
                Visual.TeethBottom,
                Visual.Tongue
            };
            
            EmotionBlendShapeMap[EmotionType.Mad] = new[]
            {
                Visual.Face.sharedMesh.GetBlendShapeIndex(Const.Character.BlendShapes.Mad),
                Visual.Brows.sharedMesh.GetBlendShapeIndex(Const.Character.BlendShapes.Mad),
                Visual.TeethTop.sharedMesh.GetBlendShapeIndex(Const.Character.BlendShapes.Mad),
                Visual.TeethBottom.sharedMesh.GetBlendShapeIndex(Const.Character.BlendShapes.Mad),
                Visual.Tongue.sharedMesh.GetBlendShapeIndex(Const.Character.BlendShapes.Mad)
            };
        }

        public void SetEmotion(EmotionType emotion, float value)
        {
            value = Mathf.Clamp(value, 0f, 100f);
            
            if (!EmotionRenderMap.TryGetValue(emotion, out var renderers))
            {
                Logger.Error(SystemTag.Emotion, $"Emotion {emotion} not found in EmotionMap");
                return;
            }

            for (var i = 0; i < renderers.Length; i++)
            {
                var indexList = EmotionBlendShapeMap[emotion];
                var blandShapeIndex = indexList[i];
                if (blandShapeIndex == -1)
                    continue;
                
                renderers[i].SetBlendShapeWeight(blandShapeIndex, value);
            }

            CurrentEmotion = value > 0f ? emotion : EmotionType.None;
        }
    }
}