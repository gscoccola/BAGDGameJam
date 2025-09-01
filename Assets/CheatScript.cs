using PixelMate.Collectibles;
using PixelMate.Gameplay;
using PixelMate.Ritual;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatScript : MonoBehaviour
{
    
    [SerializeField] private SheepRitual _sheepRitual;
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private CollectiblesManager collectiblesManager;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha6))
        {
            if (!Input.GetKey(KeyCode.M)) return;
            
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
            {
                PlayerPrefs.DeleteAll();
                collectiblesManager.Load();
                mainMenu.LoadCollectibles();
            }
            else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(1))
            {
                DeliveryManager deliveryManager = _sheepRitual.GetComponentInChildren<DeliveryManager>();
                foreach (var VARIABLE in _sheepRitual.GetComponentsInChildren<OvejaAnimation>())
                {
                    VARIABLE.transform.position = new Vector3(0, 1, 60);
                    deliveryManager.DeliverItem(VARIABLE.GetComponent<Deliverable>());
                }
                deliveryManager.ForCheatScript();
                
            }
            else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(2))
            {
                SceneManager.LoadScene(1);
            }
        }
    }
}
