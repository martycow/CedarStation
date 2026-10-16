using System;
using UnityEngine;

namespace Game.General
{
    public interface IMenuInputEvents
    {
        event Action<Vector2> Navigate;
        event Action Confirm;
        event Action Back;
        event Action CloseMenu;
    }
}