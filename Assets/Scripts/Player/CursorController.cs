using UnityEngine;

public class CursorController : MonoBehaviour
{
   [Header("Cursor Settings")]
   [SerializeField] private bool startLooked = true;
   [SerializeField, ReadOnly] private bool isLooked;
   private int _isLookedCount;
   
   
   private void Awake()
   {
      if (startLooked) LockCursor();
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