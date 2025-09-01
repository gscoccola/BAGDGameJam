using UnityEngine;

[RequireComponent(typeof(PlayerCameraController))]
public class PlayerCameraLand : MonoBehaviour
{
    [Header("Land Animation")]
    [SerializeField, Range(0f, 1f)] private float maxOffset = 1;
    [SerializeField, Range(0f, 5f)] private float maxOffsetInSeconds = 3;
    [SerializeField, ReadOnly] private Vector3 currentOffset;
    [SerializeField, ReadOnly] private Vector3 targetOffset;
    [SerializeField] private AnimationCurve landCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    private PlayerCameraController _playerCameraController;
    private PlayerController _playerController;
    private PlayerLanded _landedState;
    
    [Header("Recovery Animation")]
    [SerializeField, Range(0f, 20f)] private float recoverySmoothness = 7f;
    
    
    private void Awake()
    {
        _playerCameraController = GetComponent<PlayerCameraController>();
        _playerController = GetComponentInParent<PlayerController>();
        EventManager.Instance.Subscribe(Constants.OnPlayerLanded, OnStartLanded);
    }
    
    private void OnDestroy()
    {
        //EventManager.Instance.Unsubscribe(Constants.OnPlayerLanded, OnStartLanded);
    }

    private void Update()
    {
        UpdateLanded();
    }


    #region Land Methods
    private void OnStartLanded(params object[] parameters)
    {
        float fallTime = (float) parameters[0];
        float scale = Mathf.InverseLerp(0f, maxOffsetInSeconds, fallTime);
        targetOffset = new Vector3(0, -maxOffset * scale, 0);
    }

    private void UpdateLanded()
    {
        if (_playerController.CurrentPlayerState == PlayerState.Landed)
        {
            if (_landedState == null)
            {
                _landedState = (PlayerLanded) _playerController.Fsm.GetState(PlayerState.Landed);
                return;
            }
            
            float t = landCurve.Evaluate(_landedState.GetCurrentDuration / _playerController.Preset.LandDuration);
            currentOffset = Vector3.LerpUnclamped(Vector3.zero, targetOffset, t);
            _playerCameraController.UpdateCameraPositionOffset(currentOffset);
        }
        else
        {
            if (currentOffset == Vector3.zero) return;
            currentOffset = Vector3.Lerp(currentOffset, Vector3.zero, Time.deltaTime * recoverySmoothness);
            if (Vector3.Distance(currentOffset, Vector3.zero) <= 0.0001f)
            {
                currentOffset = Vector3.zero;
                targetOffset = Vector3.zero;
            }
            _playerCameraController.UpdateCameraPositionOffset(currentOffset);
        }
    }
    #endregion
}