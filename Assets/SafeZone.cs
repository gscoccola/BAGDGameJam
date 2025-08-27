using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {

        if (col.CompareTag("Player"))
        {
            EventManager.Instance.Trigger("EnterSafeZone", true);
        }
    }

    private void OnTriggerExit(Collider col)
    {

        if (col.CompareTag("Player"))
        {
            EventManager.Instance.Trigger("ExitSafeZone", false);
        }
    }
}
