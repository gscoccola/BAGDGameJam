using UnityEngine;

public class Sheep : MonoBehaviour
{
    private Transform parent;

    private void Awake()
    {
        parent = transform.parent;
    }

    private void OnTriggerEnter(Collider col)
    {

        if (col.CompareTag("Player"))
        {
            col.GetComponent<TopDownPlayerController>()._sheepInContact = this;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {

            col.GetComponent<TopDownPlayerController>()._sheepInContact = null;
        }
    }

    public void OnPickup(Transform player)
    {
        EventManager.Instance.Trigger(Constants.OnSheepPickup, true);
        transform.parent = player;
    }

    public void OnDrop()
    {
        transform.parent = parent;
    }
}
