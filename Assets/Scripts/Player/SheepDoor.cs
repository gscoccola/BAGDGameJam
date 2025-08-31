using UnityEngine;

namespace PixelMate.Gameplay
{
    public class SheepDoor : MonoBehaviour
    {
        [SerializeField]
        private Animator _doorAnimator;

        public void OpenDoor()
        {
            bool openBool = _doorAnimator.GetBool("Open");
            _doorAnimator.SetBool("Open", !openBool);
        }
    }
}
