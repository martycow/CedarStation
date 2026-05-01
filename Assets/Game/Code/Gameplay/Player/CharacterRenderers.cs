using System;
using UnityEngine;

[Serializable]
public struct CharacterRenderers
{
    [Header("Head")]
    [SerializeField]
    public SkinnedMeshRenderer Face;
        
    [SerializeField]
    public SkinnedMeshRenderer EyeLeft;
        
    [SerializeField]
    public SkinnedMeshRenderer EyeRight;
        
    [SerializeField]
    public SkinnedMeshRenderer Hair;
        
    [SerializeField]
    public SkinnedMeshRenderer Brows;
        
    [SerializeField]
    public SkinnedMeshRenderer Tongue;
        
    [SerializeField]
    public SkinnedMeshRenderer EarRightRenderer;
        
    [SerializeField]
    public SkinnedMeshRenderer EarLeftRenderer;
        
    [Header("Body")]
    [SerializeField]
    public SkinnedMeshRenderer BodtFullyDressed;
        
    [SerializeField]
    public SkinnedMeshRenderer BodyNaked;
        
    [SerializeField]
    public SkinnedMeshRenderer BodyUnderwearOnly;
        
    [SerializeField]
    public SkinnedMeshRenderer BodyTShirtPantsBoots;
        
    [Header("Outfit")]
    [SerializeField]
    public SkinnedMeshRenderer OutfitJacket;
        
    [SerializeField]
    public SkinnedMeshRenderer OutfitTShirt;
        
    [SerializeField]
    public SkinnedMeshRenderer OutfitPants;
        
    [SerializeField]
    public SkinnedMeshRenderer OutfitBoots;
        
    [SerializeField]
    public SkinnedMeshRenderer OutfitUnderwear;
}