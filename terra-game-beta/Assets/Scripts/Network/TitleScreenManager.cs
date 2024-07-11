using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    public static TitleScreenManager instance;

    [Header("Menus")]
    [SerializeField] GameObject titleScreenMainMenu;
    [SerializeField] GameObject loadCharacterMenu;

    [Header("Buttons")]
    [SerializeField] Button mainMenuLoadGameButton;
    [SerializeField] Button loadMenuReturnButton;

    [Header("Pop Ups")]
    [SerializeField] GameObject noSlotPopUp;
    [SerializeField] Button noSlotOKButton;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartNewGame()
    {
        WorldSaveGameManager.instance.NewGame();
    }

    public void OpenLoadGameMenu()
    {
        titleScreenMainMenu.SetActive(false);
        loadCharacterMenu.SetActive(true);
        loadMenuReturnButton.Select();
    }

    public void CloseLoadGameMenu()
    {
        titleScreenMainMenu.SetActive(true);
        loadCharacterMenu.SetActive(false);
        mainMenuLoadGameButton.Select();
    }

    public void DisplayNoSlotMessage()
    {
        noSlotPopUp.SetActive(true);
        noSlotOKButton.Select();
    }
}
