using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField, ReadOnly] private MenuState _currentMenuState;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _howToPlayMenu;
    [SerializeField] private GameObject _creditsMenu;
    private Dictionary<MenuState, GameObject> _menuDictionary = new();

    private void Awake()
    {
        _menuDictionary.Add(MenuState.Main, _mainMenu);
        _menuDictionary.Add(MenuState.HowToPlay, _howToPlayMenu);
        _menuDictionary.Add(MenuState.Credits, _creditsMenu);
    }

    private void Start()
    {
        ChangeMenu(MenuState.Main);
    }

    public void ChangeMenu(int index)
    {
        ChangeMenu((MenuState)index);
    }

    public void ChangeMenu(MenuState menuState)
    {
        _menuDictionary[_currentMenuState].gameObject.SetActive(false);
        _menuDictionary[menuState].gameObject.SetActive(true);
        _currentMenuState = menuState;
    }

    public void ChangeScene(int index)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }

}

public enum MenuState
{
    Main,
    HowToPlay,
    Credits,
}
