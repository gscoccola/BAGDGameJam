using System.Collections;
using System.Collections.Generic;
using PixelMate.Collectibles;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField, ReadOnly] private MenuState _currentMenuState;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _howToPlayMenu;
    [SerializeField] private GameObject _creditsMenu;
    [SerializeField] private GameObject _languageMenu;
    [SerializeField] private Image[] _portraits;
    private Dictionary<MenuState, GameObject> _menuDictionary = new();

    private void Awake()
    {
        _menuDictionary.Add(MenuState.Main, _mainMenu);
        _menuDictionary.Add(MenuState.HowToPlay, _howToPlayMenu);
        _menuDictionary.Add(MenuState.Credits, _creditsMenu);
        _menuDictionary.Add(MenuState.LanguageMenu, _languageMenu);
    }

    private void Start()
    {
        LoadCollectibles();
        if (LocaleInfo.Instance.IsLocaleSet) ChangeMenu(MenuState.Main);
        else ChangeMenu(MenuState.LanguageMenu);
    }

    public void ChangeMenu(int index)
    {
        ChangeMenu((MenuState)index);
    }

    public void ChangeMenu(MenuState menuState)
    {
        if (_menuDictionary[_currentMenuState] != null) _menuDictionary[_currentMenuState].gameObject.SetActive(false);
        _menuDictionary[menuState].gameObject.SetActive(true);
        _currentMenuState = menuState;
    }

    public void ChangeScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void ChooseLocale(string locale)
    {
        StartCoroutine(ISetLocale(locale));
    }

    public IEnumerator ISetLocale(string locale)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales.Find(x => x.Identifier.Code == locale);
        LocaleInfo.Instance.CurrentLocale = locale;
        LocaleInfo.Instance.IsLocaleSet = true;
        ChangeScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadCollectibles()
    {
        for(int i = 0; i < _portraits.Length; i++)
        {
            _portraits[i].enabled = CollectiblesManager.Instance.IsCollected((i+1).ToString());
        }
    }
}

    


public enum MenuState
{
    Main,
    HowToPlay,
    Credits,
    LanguageMenu,
}
