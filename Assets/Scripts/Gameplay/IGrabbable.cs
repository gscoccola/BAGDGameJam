using UnityEngine;

namespace PixelMate.Gameplay
{
    public interface IGrabbable
    {
        public bool IsHeld { get; }

        void BeginGrab(Transform holdPoint);
        void EndGrab();
    }
}