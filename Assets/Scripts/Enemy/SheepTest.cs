using UnityEngine;

public class SheepTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            EventManager.Instance.Trigger(Constants.OnSheepPickup);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            EventManager.Instance.Trigger(Constants.OnSheepNoise);
        }
    }
}
