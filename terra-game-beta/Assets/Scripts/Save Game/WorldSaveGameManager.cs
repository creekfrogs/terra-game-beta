using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager instance;

    [HideInInspector] public PlayerManager player;

    [Header("SAVE/LOAD")]
    [SerializeField] bool saveGame;
    [SerializeField] bool loadGame;

    [Header("World Scene Index")]
    [SerializeField] int worldSceneIndex = 1;

    [Header("Save File Writer")]
    private SaveFileWriter saveFileWriter;

    [Header("Current Save")]
    public SaveSlot currentSaveSlotBeingUsed;
    public CharacterSaveData currentCharacterData;
    private string saveFileName;

    [Header("Save Slots")]
    public CharacterSaveData saveSlot01;
    public CharacterSaveData saveSlot02;
    public CharacterSaveData saveSlot03;
    public CharacterSaveData saveSlot04;
    public CharacterSaveData saveSlot05;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadAllSaveSlots();
    }

    private void Update()
    {
        if(saveGame)
        {
            saveGame = false;
            SaveGame();
        }

        if(loadGame)
        {
            loadGame = false;
            LoadGame();
        }
    }

    public string DecideFileNameBasedOnSaveSlot(SaveSlot saveSlot)
    {
        string fileName = "";
        switch(saveSlot)
        {
            case SaveSlot.SaveSlot_01:
                fileName = "SaveSlot_01";
                break;
            case SaveSlot.SaveSlot_02:
                fileName = "SaveSlot_02";
                break;
            case SaveSlot.SaveSlot_03:
                fileName = "SaveSlot_03";
                break;
            case SaveSlot.SaveSlot_04:
                fileName = "SaveSlot_04";
                break;
            case SaveSlot.SaveSlot_05:
                fileName = "SaveSlot_05";
                break;   
        }
        return fileName;
    }

    public void AttemptNewGame()
    {
        saveFileWriter = new SaveFileWriter();
        saveFileWriter.saveDataDir = Application.persistentDataPath;

        
        saveFileWriter.saveFileName = DecideFileNameBasedOnSaveSlot(SaveSlot.SaveSlot_01);


        if (!saveFileWriter.CheckIfFileExists())
        {
            currentSaveSlotBeingUsed = SaveSlot.SaveSlot_01;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        saveFileWriter.saveFileName = DecideFileNameBasedOnSaveSlot(SaveSlot.SaveSlot_02);

        if (!saveFileWriter.CheckIfFileExists())
        {
            currentSaveSlotBeingUsed = SaveSlot.SaveSlot_02;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        saveFileWriter.saveFileName = DecideFileNameBasedOnSaveSlot(SaveSlot.SaveSlot_03);

        if (!saveFileWriter.CheckIfFileExists())
        {
            currentSaveSlotBeingUsed = SaveSlot.SaveSlot_03;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        saveFileWriter.saveFileName = DecideFileNameBasedOnSaveSlot(SaveSlot.SaveSlot_04);

        if (!saveFileWriter.CheckIfFileExists())
        {
            currentSaveSlotBeingUsed = SaveSlot.SaveSlot_04;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        saveFileWriter.saveFileName = DecideFileNameBasedOnSaveSlot(SaveSlot.SaveSlot_05);

        if (!saveFileWriter.CheckIfFileExists())
        {
            currentSaveSlotBeingUsed = SaveSlot.SaveSlot_05;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        TitleScreenManager.instance.DisplayNoSlotMessage();
    }

    private void NewGame()
    {
        SaveGame();
        StartCoroutine(LoadWorld());
    }

    public void LoadGame()
    {
        saveFileName = DecideFileNameBasedOnSaveSlot(currentSaveSlotBeingUsed);

        saveFileWriter = new SaveFileWriter();

        saveFileWriter.saveDataDir = Application.persistentDataPath;
        saveFileWriter.saveFileName = saveFileName;
        currentCharacterData = saveFileWriter.LoadSaveFile();

        StartCoroutine(LoadWorld());
    }

    public void DeleteGame(SaveSlot saveSlot)
    {
        saveFileWriter = new SaveFileWriter();
        saveFileWriter.saveDataDir = Application.persistentDataPath;
        saveFileWriter.saveFileName = saveFileName;
        saveFileWriter.saveFileName = DecideFileNameBasedOnSaveSlot(saveSlot);

        saveFileWriter.DeleteSaveFile();
    }

    public void SaveGame()
    {
        saveFileName = DecideFileNameBasedOnSaveSlot(currentSaveSlotBeingUsed);

        saveFileWriter = new SaveFileWriter();
        saveFileWriter.saveDataDir = Application.persistentDataPath;
        saveFileWriter.saveFileName = saveFileName;

        player.SaveToCharacterData(ref currentCharacterData);

        saveFileWriter.InitializeSaveFile(currentCharacterData);
    }

    public void LoadAllSaveSlots()
    {
        saveFileWriter = new SaveFileWriter();
        saveFileWriter.saveDataDir = Application.persistentDataPath;

        saveFileWriter.saveFileName = DecideFileNameBasedOnSaveSlot(SaveSlot.SaveSlot_01);
        saveSlot01 = saveFileWriter.LoadSaveFile();
        saveFileWriter.saveFileName = DecideFileNameBasedOnSaveSlot(SaveSlot.SaveSlot_02);
        saveSlot02 = saveFileWriter.LoadSaveFile();
        saveFileWriter.saveFileName = DecideFileNameBasedOnSaveSlot(SaveSlot.SaveSlot_03);
        saveSlot03 = saveFileWriter.LoadSaveFile();
        saveFileWriter.saveFileName = DecideFileNameBasedOnSaveSlot(SaveSlot.SaveSlot_04);
        saveSlot04 = saveFileWriter.LoadSaveFile();
        saveFileWriter.saveFileName = DecideFileNameBasedOnSaveSlot(SaveSlot.SaveSlot_05);
        saveSlot05 = saveFileWriter.LoadSaveFile();
    }

    public IEnumerator LoadWorld()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        player.LoadToCharacterData(ref currentCharacterData);

        yield return null;
    }

    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }
}
