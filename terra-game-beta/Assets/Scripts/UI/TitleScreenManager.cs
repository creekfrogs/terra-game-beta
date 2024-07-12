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
    [SerializeField] GameObject loadSaveMenu;

    [Header("Buttons")]
    [SerializeField] Button mainMenuLoadGameButton;
    [SerializeField] Button loadMenuReturnButton;

    [Header("Pop Ups")]
    [SerializeField] GameObject noSlotPopUp;
    [SerializeField] Button noSlotOKButton;
    [SerializeField] GameObject deleteSlotPopUp;
    [SerializeField] Button deleteSlotYESButton;
    [SerializeField] Button deleteSlotNOButton;

    [Header("Save Slots")]
    public SaveSlot currentSaveSlot = SaveSlot.NO_SLOT;
    
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
        WorldSaveGameManager.instance.AttemptNewGame();
    }

    public void OpenLoadGameMenu()
    {
        titleScreenMainMenu.SetActive(false);
        loadSaveMenu.SetActive(true);
        loadMenuReturnButton.Select();
    }

    public void CloseLoadGameMenu()
    {
        titleScreenMainMenu.SetActive(true);
        loadSaveMenu.SetActive(false);
        mainMenuLoadGameButton.Select();
    }

    public void DisplayNoSlotMessage()
    {
        noSlotPopUp.SetActive(true);
        noSlotOKButton.Select();
    }

    public void SelectSaveSlot(SaveSlot saveSlot)
    {
        currentSaveSlot = saveSlot;
    }

    public void SelectNoSlot()
    {
        currentSaveSlot = SaveSlot.NO_SLOT;
    }

    public void AttemptDeleteSlot()
    {
        if(currentSaveSlot != SaveSlot.NO_SLOT)
        {
            deleteSlotPopUp.SetActive(true);
            deleteSlotNOButton.Select();
        }
    }

    public void DeleteCharacterSlot()
    {
        deleteSlotPopUp.SetActive(false);
        WorldSaveGameManager.instance.DeleteGame(currentSaveSlot);
        loadSaveMenu.SetActive(false);
        loadSaveMenu.SetActive(true);
        loadMenuReturnButton.Select();
    }
}
