using PixelMate.Gameplay;
using PixelMate.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelMate.Ritual
{
    [DisallowMultipleComponent]
    public sealed class SheepRitual : MonoBehaviour
    {
        [SerializeField]
        private SheepDoor _sheepDoor;
        [SerializeField]
        private DeliveryManager _deliverymanager;
        [SerializeField]
        private Grabber _playerGrabber;

        [Header("Spin Settings")]
        [SerializeField]
        private Transform _ritualPoint;
        [SerializeField]
        private float _radiusFromCenter = 4f;
        [SerializeField]
        private float spinAcceleration = 30f; // deg/sec^2
        [SerializeField]
        private float maxSpinSpeed = 180f;    // deg/sec
        [SerializeField]
        private float holdDuration = 3f;      // how long to spin at max before stopping
        [SerializeField]
        private float deceleration = 60f;     // deg/sec^2

        private Coroutine spinRoutine;

        private List<Vector3> _ritualPositions = new List<Vector3>();
        private List<Deliverable> _deliverables = new List<Deliverable>();

        private void Start()
        {
            _ritualPositions = MathHelper.GetEvenCirclePositions(_ritualPoint.position, _radiusFromCenter, _deliverymanager.RequiredDeliveries);
        }

        private void OnEnable()
        {
            _sheepDoor.OnDoorClose -= HandleDoorClose;
            _sheepDoor.OnDoorClose += HandleDoorClose;
        }

        private void OnDisable()
        {
            _sheepDoor.OnDoorClose -= HandleDoorClose;
        }

        private void HandleDoorClose()
        {
            if (!_deliverymanager.AreAllItemsDelivered())
                return;

            if (_playerGrabber != null)
                _playerGrabber.AllowGrabberState(false);

            for (int i = 0; i < _deliverymanager.DeliveredItems.Count; i++)
            {
                Deliverable deliverable = _deliverymanager.DeliveredItems[i];
                if (deliverable != null)
                {
                    deliverable.transform.position = _ritualPositions[i];
                    deliverable.transform.LookAt(_ritualPoint);
                    deliverable.transform.Rotate(0f, 270f, 0f);
                    deliverable.transform.SetParent(_ritualPoint);

                    _deliverables.Add(deliverable);
                }
            }

            if (spinRoutine != null)
                StopCoroutine(spinRoutine);

            spinRoutine = StartCoroutine(SpinRitualPoint());
        }

        private IEnumerator SpinRitualPoint()
        {
            float currentSpeed = 0f;

            // accelerate up to max
            while (currentSpeed < maxSpinSpeed)
            {
                currentSpeed += spinAcceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, maxSpinSpeed);
                _ritualPoint.Rotate(Vector3.up, currentSpeed * Time.deltaTime, Space.World);
                yield return null;
            }

            // hold spin for a while
            float t = 0f;
            while (t < holdDuration)
            {
                _ritualPoint.Rotate(Vector3.up, currentSpeed * Time.deltaTime, Space.World);
                t += Time.deltaTime;
                yield return null;
            }

            // decelerate to stop
            while (currentSpeed > 0f)
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0f);
                _ritualPoint.Rotate(Vector3.up, currentSpeed * Time.deltaTime, Space.World);
                yield return null;
            }

            for (int i = 0; i < _deliverables.Count; i++)
            {
                _deliverables[i].KillSheep();
            }
        }
    }
}
