using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_CharacterSaveSlot : MonoBehaviour
{
    SaveFileWriter saveFileWriter;

    [Header("Game Slot")]
    public SaveSlot saveSlot;

    [Header("Character Info")]
    public TextMeshProUGUI saveIndex;
    public TextMeshProUGUI timePlayed;
    public TextMeshProUGUI location;

    private void OnEnable()
    {
        LoadSaveSlot();
    }

    private void LoadSaveSlot()
    {
        saveFileWriter = new SaveFileWriter();
        saveFileWriter.saveDataDir = Application.persistentDataPath;

        if(saveSlot == SaveSlot.SaveSlot_01)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideFileNameBasedOnSaveSlot(saveSlot);

            if (saveFileWriter.CheckIfFileExists())
            {
                float secondsPlayed = WorldSaveGameManager.instance.saveSlot01.secondsPlayed;

                saveIndex.text = "SAVE 1";
                timePlayed.text = "00:00:00";
                location.text = "Debug";
            }
            else
            {
                saveIndex.text = "Empty Save";
                timePlayed.text = "";
                location.text = "";
            }
        }
        else if (saveSlot == SaveSlot.SaveSlot_02)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideFileNameBasedOnSaveSlot(saveSlot);

            if (saveFileWriter.CheckIfFileExists())
            {
                saveIndex.text = "SAVE 2";
                timePlayed.text = "00:00:00";
                location.text = "Debug";
            }
            else
            {
                saveIndex.text = "Empty Save";
                timePlayed.text = "";
                location.text = "";
            }
        }
        else if (saveSlot == SaveSlot.SaveSlot_03)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideFileNameBasedOnSaveSlot(saveSlot);

            if (saveFileWriter.CheckIfFileExists())
            {
                saveIndex.text = "SAVE 3";
                timePlayed.text = "00:00:00";
                location.text = "Debug";
            }
            else
            {
                saveIndex.text = "Empty Save";
                timePlayed.text = "";
                location.text = "";
            }
        }
        else if (saveSlot == SaveSlot.SaveSlot_04)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideFileNameBasedOnSaveSlot(saveSlot);

            if (saveFileWriter.CheckIfFileExists())
            {
                saveIndex.text = "SAVE 4";
                timePlayed.text = "00:00:00";
                location.text = "Debug";
            }
            else
            {
                saveIndex.text = "Empty Save";
                timePlayed.text = "";
                location.text = "";
            }
        }
        else if (saveSlot == SaveSlot.SaveSlot_05)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideFileNameBasedOnSaveSlot(saveSlot);

            if (saveFileWriter.CheckIfFileExists())
            {
                saveIndex.text = "SAVE 5";
                timePlayed.text = "00:00:00";
                location.text = "Debug";
            }
            else
            {
                saveIndex.text = "Empty Save";
                timePlayed.text = "";
                location.text = "";
            }
        }
    }

    public void LoadGameFromSlot()
    {
        WorldSaveGameManager.instance.currentSaveSlotBeingUsed = saveSlot;
        WorldSaveGameManager.instance.LoadGame();
    }

    public void SelectCurrentSlot()
    {
        TitleScreenManager.instance.SelectSaveSlot(saveSlot);
    }
}
