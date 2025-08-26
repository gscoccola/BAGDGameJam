using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static CustomInputActionsMap Controls { get; private set; }

    private void Awake()
    {
        Controls = new CustomInputActionsMap();
        Controls.Enable();
        Controls.Gameplay.Enable();
    }
}