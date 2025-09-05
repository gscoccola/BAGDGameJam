using System.Collections;
using System.Collections.Generic;
using PixelMate.Collectibles;
using UnityEngine;
using UnityEngine.EventSystems;
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
    public static bool languageChoose;
    public AudioClip clickButtonSound;
    public AudioClip clickPlayButtonSound;
    private Dictionary<MenuState, GameObject> _defaultButtonDictionary = new();

    private void Awake()
    {
        _menuDictionary.Add(MenuState.Main, _mainMenu);
        _menuDictionary.Add(MenuState.HowToPlay, _howToPlayMenu);
        _menuDictionary.Add(MenuState.Credits, _creditsMenu);
        _menuDictionary.Add(MenuState.LanguageMenu, _languageMenu);

        _defaultButtonDictionary.Add(MenuState.Main, _mainMenu.transform.GetChild(0).GetChild(0).gameObject);
        _defaultButtonDictionary.Add(MenuState.HowToPlay, _howToPlayMenu.transform.GetChild(1).GetChild(0).gameObject);
        _defaultButtonDictionary.Add(MenuState.Credits, _creditsMenu.transform.GetChild(1).GetChild(0).gameObject);
        _defaultButtonDictionary.Add(MenuState.LanguageMenu, _languageMenu.transform.GetChild(0).GetChild(0).gameObject);
    }

    private void Start()
    {
        LoadCollectibles();
        if (LocaleInfo.Instance.IsLocaleSet) ChangeMenu(MenuState.Main);
        else ChangeMenu(MenuState.LanguageMenu);
    }

    public void ButtonSounds()
    {
        AudioSystem.Instance.PlaySound(clickButtonSound);
    }
    
    public void PlayButtonSounds()
    {
        AudioSystem.Instance.PlaySound(clickPlayButtonSound);
    }

    public void ChangeMenu(int index)
    {
        ChangeMenu((MenuState)index);
    }

    public void ChangeMenu(MenuState menuState)
    {
        
        if (_menuDictionary[_currentMenuState] != null) _menuDictionary[_currentMenuState].gameObject.SetActive(false);
        _menuDictionary[menuState].gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_defaultButtonDictionary[menuState]);
        _currentMenuState = menuState;
        ButtonSounds();
    }

    public void ChangeScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void ChooseLocale(string locale)
    {
        StartCoroutine(ISetLocale(locale));
        ButtonSounds();
    }

    public IEnumerator ISetLocale(string locale)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales.Find(x => x.Identifier.Code == locale);
        LocaleInfo.Instance.CurrentLocale = locale;
        LocaleInfo.Instance.IsLocaleSet = true;
        ChangeMenu(MenuState.Main);
        languageChoose = true;
    }

    public void LoadCollectibles()
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
