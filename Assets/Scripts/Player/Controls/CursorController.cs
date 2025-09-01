using System;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
   [Header("Cursor Settings")]
   [SerializeField] private bool startLooked = true;
   [SerializeField, ReadOnly] private bool isLooked;
   private int _isLookedCount;
   public Image cursorImage;
   
   public Sprite cursorImageDefault;
   public Sprite cursorImageInteract;
   
   
   private void Awake()
   {
      if (startLooked) LockCursor();
      CancelWantInteract();
      EventManager.Instance.Subscribe(Constants.OnEnableControls, LockCursor);
      EventManager.Instance.Subscribe(Constants.OnDisableControls, UnLockCursor);
      
      
      EventManager.Instance.Subscribe(Constants.OnWantInteract, WantInteract);
      EventManager.Instance.Subscribe(Constants.OnCancelWantInteract, CancelWantInteract);
   }

   private void OnDestroy()
   {
      //EventManager.Instance.Unsubscribe(Constants.OnEnableControls, LockCursor);
      //EventManager.Instance.Unsubscribe(Constants.OnDisableControls, UnLockCursor);
   }

   private void WantInteract()
   {
      cursorImage.sprite = cursorImageInteract;
   }

   private void CancelWantInteract()
   {
      cursorImage.sprite = cursorImageDefault;
   }
   
   
   

   #region Cursor Methods
   private void LockCursor()
   {
      _isLookedCount++;
      if (_isLookedCount > 1) return;
      
      Cursor.lockState = CursorLockMode.Locked;
   }
    
   private void UnLockCursor()
   {
      _isLookedCount--;
      
      if (_isLookedCount < 0) _isLookedCount = 0;
      if (_isLookedCount != 0) return;
      
      Cursor.lockState = CursorLockMode.None;
   }
   #endregion
}