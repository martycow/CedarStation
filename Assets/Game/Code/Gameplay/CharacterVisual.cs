using System;
using System.Collections.Generic;
using Game.General;
using UnityEngine;

[Serializable]
public class CharacterVisual
{
    [SerializeField]
    [AutoAssign("Face", true)]
    public SkinnedMeshRenderer Face;
        
    [SerializeField]
    [AutoAssign("Eye_Left", true)]
    public SkinnedMeshRenderer EyeLeft;
        
    [SerializeField]
    [AutoAssign("Eye_Right", true)]
    public SkinnedMeshRenderer EyeRight;
        
    [SerializeField]
    [AutoAssign("Hair", true)]
    public SkinnedMeshRenderer Hair;
        
    [SerializeField]
    [AutoAssign("Brows", true)]
    public SkinnedMeshRenderer Brows;
        
    [SerializeField]
    [AutoAssign("Tongue", true)]
    public SkinnedMeshRenderer Tongue;
        
    [SerializeField]
    [AutoAssign("Ear_Left", true)]
    public SkinnedMeshRenderer EarLeft;
    
    [SerializeField]
    [AutoAssign("Ear_Right", true)]
    public SkinnedMeshRenderer EarRight;
    
    [SerializeField]
    [AutoAssign("Teeth_Top", true)]
    public SkinnedMeshRenderer TeethTop;
    
    [SerializeField]
    [AutoAssign("Teeth_Bottom", true)]
    public SkinnedMeshRenderer TeethBottom;
    
    [SerializeField]
    [AutoAssign("Body_Fully_Dressed", true)]
    public SkinnedMeshRenderer BodyFullyDressed;
        
    [SerializeField]
    [AutoAssign("Body_Naked", true)]
    public SkinnedMeshRenderer BodyNaked;
        
    [SerializeField]
    [AutoAssign("Body_UnderwearOnly", true)]
    public SkinnedMeshRenderer BodyUnderwearOnly;
        
    [SerializeField]
    [AutoAssign("Body_TshirtPantsBoots", true)]
    public SkinnedMeshRenderer BodyTShirtPantsBoots;
    
    [SerializeField]
    [AutoAssign("Outfit_Jacket", true)]
    public SkinnedMeshRenderer OutfitJacket;
        
    [SerializeField]
    [AutoAssign("Outfit_Tshirt", true)]
    public SkinnedMeshRenderer OutfitTShirt;
        
    [SerializeField]
    [AutoAssign("Outfit_Pants", true)]
    public SkinnedMeshRenderer OutfitPants;
        
    [SerializeField]
    [AutoAssign("Outfit_Shoes", true)]
    public SkinnedMeshRenderer OutfitBoots;
        
    [SerializeField]
    [AutoAssign("Outfit_Underwear", true)]
    public SkinnedMeshRenderer OutfitUnderwear;

    public readonly Dictionary<CharacterMeshType, List<SkinnedMeshRenderer>> Map = new();
    
    public CharacterVisual()
    {
        TryAdd(CharacterMeshType.Face, Face);
        TryAdd(CharacterMeshType.Eyes, EyeLeft);
        TryAdd(CharacterMeshType.Eyes, EyeRight);
        TryAdd(CharacterMeshType.Hair, Hair);
        TryAdd(CharacterMeshType.Brows, Brows);
        TryAdd(CharacterMeshType.Tongue, Tongue);
        TryAdd(CharacterMeshType.Ears, EarLeft);
        TryAdd(CharacterMeshType.Ears, EarRight);
        TryAdd(CharacterMeshType.Teeth, TeethTop);
        TryAdd(CharacterMeshType.Teeth, TeethBottom);
        TryAdd(CharacterMeshType.Body, BodyFullyDressed);
        TryAdd(CharacterMeshType.Body, BodyNaked);
        TryAdd(CharacterMeshType.Body, BodyUnderwearOnly);
        TryAdd(CharacterMeshType.Body, BodyTShirtPantsBoots);
        TryAdd(CharacterMeshType.Outfit, OutfitJacket);
        TryAdd(CharacterMeshType.Outfit, OutfitTShirt);
        TryAdd(CharacterMeshType.Outfit, OutfitPants);
        TryAdd(CharacterMeshType.Outfit, OutfitBoots);
        TryAdd(CharacterMeshType.Outfit, OutfitUnderwear);
    }

    private void TryAdd(CharacterMeshType meshType, SkinnedMeshRenderer renderer)
    {
        if (renderer == null)
            return;

        if (Map.TryGetValue(meshType, out var rendererList))
            rendererList.Add(renderer);
        else
            Map[meshType] = new List<SkinnedMeshRenderer> { renderer };
    }
}