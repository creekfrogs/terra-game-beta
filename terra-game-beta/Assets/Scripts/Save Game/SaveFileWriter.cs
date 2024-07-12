using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SaveFileWriter
{
    public string saveDataDir = "";
    public string saveFileName = "";

    public bool CheckIfFileExists()
    {
        if (File.Exists(Path.Combine(saveDataDir, saveFileName)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DeleteSaveFile()
    {
        string deletePath = Path.Combine(saveDataDir, saveFileName);
        Debug.Log("DELETING SAVE FILE AT: " + deletePath);
        File.Delete(deletePath);
    }

    public void InitializeSaveFile(CharacterSaveData characterData)
    {
        string savePath = Path.Combine(saveDataDir, saveFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            Debug.Log("CREATING SAVE FILE AT: " + savePath);

            string dataToStore = JsonUtility.ToJson(characterData, true);
            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("ERROR WHILST TRYING TO SAVE CHARACTER DATA, GAME NOT SAVED" + savePath + "\n" + ex);
        }
    }

    public CharacterSaveData LoadSaveFile()
    {
        CharacterSaveData characterData = null;
        string loadPath = Path.Combine(saveDataDir, saveFileName);

        if(File.Exists(loadPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);

            }
            catch(Exception ex)
            {
                Debug.LogError("ERROR WHILST TRYING TO LOAD CHARACTER DATA, GAME NOT LOADED" + loadPath + "\n" + ex);
            }
        }

        return characterData;
    }
}
