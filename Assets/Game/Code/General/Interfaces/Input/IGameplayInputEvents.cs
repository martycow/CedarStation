using System;
using UnityEngine;

namespace Game.General
{
    public interface IGameplayInputEvents
    {
        event Action<Vector2> MoveStarted;
        event Action<Vector2> MovePerformed;
        event Action<Vector2> MoveCanceled;
        
        event Action<Vector2> MoveCamera;
        event Action<float> ZoomCamera;
        
        event Action Jump;
        event Action Tool;
        event Action Interact;
        event Action Kick;
        event Action Crouch;
        
        event Action OpenMenu;
    }
}