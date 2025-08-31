using UnityEngine;

namespace PixelMate.Gameplay
{
    public interface IGrabbable
    {
        public bool IsHeld { get; }

        void BeginGrab();
        void EndGrab(Vector3 releasePoint, Quaternion rotation);
    }
}