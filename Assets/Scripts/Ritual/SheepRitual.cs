using PixelMate.Gameplay;
using PixelMate.Utilities;
using System.Collections;
using System.Collections.Generic;
using PixelMate.Collectibles;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PixelMate.Ritual
{
    [DisallowMultipleComponent]
    public sealed class SheepRitual : MonoBehaviour
    {
        [SerializeField] private CollectiblesManager collectiblesManager;
        [SerializeField] private TextMeshProUGUI collecionablesText;
        
        
        
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
        [SerializeField]
        private float _waitBeforeRitualEnd = 2f;

        [Header("Audio")]
        [SerializeField]
        private AudioClip _ritualSpinClip;
        [SerializeField]
        private AudioClip _thunderSpinClip;
        [SerializeField]
        private AudioClip _boyComingOutClip;
        [SerializeField]
        private AudioClip _boyVoiceClip;
        [SerializeField]
        private AudioClip _farmerVoiceClip;

        [Header("Ritual End")]
        [SerializeField]
        private Image _screenCoverPanel;
        [SerializeField]
        private GameObject _farmerChild;
        [SerializeField]
        private float _fadeDuration = 1.5f;
        [SerializeField]
        private float _farmerRiseDuration = 1.25f;
        [SerializeField]
        private float _farmerSpawnDepth = 2.0f; // how far below ground to start
        [SerializeField]
        private TMP_Text _endText;
        [SerializeField] private GameObject menuButton;
        [SerializeField]
        private GameObject enemyPArticles;
        

        private Coroutine spinRoutine;
        private List<Deliverable> _deliverables = new List<Deliverable>();
        private bool _ritualEndingStarted;

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

            List<Vector3> _ritualPositions = MathHelper.GetEvenCirclePositions(_ritualPoint.position,
                _radiusFromCenter,
                _deliverymanager.RequiredDeliveries);

            for (int i = 0; i < _deliverymanager.DeliveredItems.Count; i++)
            {
                Deliverable deliverable = _deliverymanager.DeliveredItems[i];
                if (deliverable != null)
                {
                    deliverable.transform.position = _ritualPositions[i];
                    deliverable.transform.LookAt(_ritualPoint);
                    deliverable.transform.Rotate(0f, 270f, 0f);
                    deliverable.transform.SetParent(_ritualPoint);
                    deliverable.GetComponent<OvejaAnimation>().StartWalkAnimation();

                    _deliverables.Add(deliverable);
                }
            }

            if (spinRoutine != null)
                StopCoroutine(spinRoutine);

            spinRoutine = StartCoroutine(SpinRitualPoint());
        }

        private IEnumerator SpinRitualPoint()
        {
            AudioSystem.Instance.PlaySound(_ritualSpinClip, _ritualPoint.position);
            float currentSpeed = 0f;

            // accelerate up to max
            while (currentSpeed < maxSpinSpeed)
            {
                if (currentSpeed >= maxSpinSpeed / 2) 
                    enemyPArticles.SetActive(true);
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

            float decelerationStartTime = Time.time; // For debugging, it takes 2.997 seconds to finish
            bool twoSecondMessageShown = false; // Flag to show message only once

            // decelerate to stop
            while (currentSpeed > 0f)
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0f);
                _ritualPoint.Rotate(Vector3.up, currentSpeed * Time.deltaTime, Space.World);

                // Check if 2 seconds have elapsed and message hasn't been shown yet
                if (!twoSecondMessageShown && (Time.time - decelerationStartTime) >= _waitBeforeRitualEnd)
                {
                    Debug.Log($"[SheepRitual] 2 seconds elapsed during deceleration (Speed: {currentSpeed:F1} deg/s)");
                    twoSecondMessageShown = true;
                    TriggeRitualEnd();
                }

                yield return null;
            }

            // DEBUG: Calculate and log the deceleration time
            //float decelerationDuration = Time.time - decelerationStartTime;
            //Debug.Log($"[SheepRitual] Deceleration took {decelerationDuration:F3} seconds");

            for (int i = 0; i < _deliverables.Count; i++)
            {
                _deliverables[i].KillSheep();
            }
        }

        #region Ritual End Sequence

        private void TriggeRitualEnd()
        {
            if (_ritualEndingStarted)
                return;

            _ritualEndingStarted = true;

            AudioSystem.Instance.PlaySound(_thunderSpinClip, _ritualPoint.position);
            StartCoroutine(RitualEndSequence());
        }

        private IEnumerator RitualEndSequence()
        {
            // Setup: enable cover, ensure it starts fully opaque (alpha = 1)
            if (_screenCoverPanel != null)
            {
                _screenCoverPanel.gameObject.SetActive(true);
                var c = _screenCoverPanel.color;
                c.a = 1f;
                _screenCoverPanel.color = c;
            }

            // Prep farmer: position below ground, then rise to current ground position
            Vector3 farmerTargetPos = _farmerChild.transform.position;
            Vector3 farmerStartPos = farmerTargetPos + Vector3.down * _farmerSpawnDepth;
            _farmerChild.transform.position = farmerStartPos;
            _farmerChild.SetActive(true);

            // Run fade and rise in parallel
            IEnumerator fade = FadeImageAlpha(_screenCoverPanel, 1f, 0f, _fadeDuration);
            IEnumerator rise = MoveOverTime(_farmerChild.transform, farmerStartPos, farmerTargetPos, _farmerRiseDuration);

            // Start a �coming out� stinger when the farmer's son begins rising
            AudioSystem.Instance.PlaySound(_boyComingOutClip, _ritualPoint.position);

            // Start both
            Coroutine fadeCo = StartCoroutine(fade);
            Coroutine riseCo = StartCoroutine(rise);

            // Wait for both to finish
            yield return fadeCo;
            yield return riseCo;

            // Sequential VO: boy, then farmer
            yield return PlayVoiceSequential(_boyVoiceClip, _farmerVoiceClip);

            // TODO: trigger next phase/gameplay here if needed
        }

        private IEnumerator FadeImageAlpha(Image img, float from, float to, float duration)
        {
            if (img == null || duration <= 0f) yield break;
            float t = 0f;
            Color c = img.color;
            c.a = from;
            img.color = c;

            yield return new WaitForSeconds(1f);
            
            while (t < duration)
            {
                t += Time.deltaTime;
                float a = Mathf.Lerp(from, to, t / duration);
                c.a = a;
                img.color = c;
                yield return null;
            }
            // snap to final
            c.a = to;
            img.color = c;
        }

        private IEnumerator MoveOverTime(Transform target, Vector3 from, Vector3 to, float duration)
        {
            if (target == null || duration <= 0f) yield break;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                target.position = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t / duration));
                yield return null;
            }
            target.position = to;
        }

        private IEnumerator PlayVoiceSequential(AudioClip first, AudioClip second)
        {
            yield return new WaitForSeconds(0.7f);
            // 1) Boy voice
            if (first != null)
            {
                AudioSystem.Instance.PlaySound(first, _ritualPoint.position);
                PauseManager.Instance._localizeStringEvent.StringReference.TableEntryReference = "Dad";
                yield return new WaitForSeconds(first.length);
            }

            // 2) Fade to black (0 -> 1 over _fadeDuration)
            if (_screenCoverPanel != null)
            {
                _screenCoverPanel.gameObject.SetActive(true);
                EventSystem.current.SetSelectedGameObject(_screenCoverPanel.transform.GetChild(0).GetChild(1).gameObject);

                // force full black, alpha 0 to start the fade-in
                Color c = _screenCoverPanel.color;
                c.r = 0f; c.g = 0f; c.b = 0f; c.a = 0f;
                _screenCoverPanel.color = c;

                // reuse your helper to fade to opaque
                yield return FadeImageAlpha(_screenCoverPanel, 0f, 1f, _fadeDuration);
            }

            
            string currentCollecionablesText = $"{collectiblesManager.CollectedCount}/5";
            LocalizedString info = new LocalizedString("Eng-Esp", "COLECCIONABLES");
            info.Arguments = new object[] { new {TOTAL = currentCollecionablesText} };
            collecionablesText.text = info.GetLocalizedString();
            
            // 4) Show end text
            if (_endText != null)
                _endText.gameObject.SetActive(true);
            menuButton.gameObject.SetActive(true);
            collecionablesText.gameObject.SetActive(true);
            
            EventManager.Instance.Trigger(Constants.OnDisableControls);
            
            // 3) Farmer voice
            if (second != null)
            {
                AudioSystem.Instance.PlaySound(second);
                //PauseManager.Instance._localizeStringEvent.StringReference.TableEntryReference = "Osvaldo";
                yield return new WaitForSeconds(second.length);
            }

        }

        #endregion
        
        public void LoadMenu()
        {
            SceneManager.LoadScene(0);
        }
    }
}
