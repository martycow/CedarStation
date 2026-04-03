using System;
using UnityEngine;

namespace Game.General
{
    public interface IGameplayInputEvents
    {
        event Action<Vector2> OnPlayerMoveChanged;
        
        event Action<Vector2> OnCameraMoveChanged;
        event Action<float> OnCameraZoomChanged;
        
        event Action Jump;
        event Action Tool;
        event Action Interact;
        event Action Kick;
        event Action Crouch;
        
        event Action OpenMenu;
    }
}