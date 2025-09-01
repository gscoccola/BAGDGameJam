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
        [SerializeField]
        private string _openStateName = "Puerta Open";

        public event Action OnDoorClose;

        private bool _isAnimating;

        public void MoveDoor()
        {
            if (_isAnimating)
                return;

            bool openBool = _doorAnimator.GetBool("Open");
            _doorAnimator.SetBool("Open", !openBool);

            string targetState = openBool ? _closeStateName : _openStateName;
            StartCoroutine(WaitForAnimation(targetState));
        }

        private System.Collections.IEnumerator WaitForAnimation(string stateName)
        {
            _isAnimating = true;

            // Wait until we’re in the correct state
            AnimatorStateInfo info = _doorAnimator.GetCurrentAnimatorStateInfo(0);
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

            _isAnimating = false;
        }
    }
}
