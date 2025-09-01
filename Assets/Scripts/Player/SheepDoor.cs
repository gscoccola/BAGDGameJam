using System;
using UnityEngine;

namespace PixelMate.Gameplay
{
    public class SheepDoor : MonoBehaviour
    {
        [SerializeField]
        private Animator _doorAnimator;
        [SerializeField]
        private string _closeStateName = "Puerta Close";

        public event Action OnDoorClose;

        public void MoveDoor()
        {
            bool openBool = _doorAnimator.GetBool("Open");
            _doorAnimator.SetBool("Open", !openBool);

            if (openBool)
                StartCoroutine(WaitForAnimation(_closeStateName));
        }

        private System.Collections.IEnumerator WaitForAnimation(string stateName)
        {
            // 0 = base layer
            AnimatorStateInfo info = _doorAnimator.GetCurrentAnimatorStateInfo(0);

            // Wait until we’re in the correct state
            while (!info.IsName(stateName))
            {
                yield return null;
                info = _doorAnimator.GetCurrentAnimatorStateInfo(0);
            }

            // Now wait until the animation has completed one loop
            while (info.normalizedTime < 1f)
            {
                yield return null;
                info = _doorAnimator.GetCurrentAnimatorStateInfo(0);
            }

            Debug.Log($"{stateName} finished!");
            OnDoorClose?.Invoke();
        }
    }
}
