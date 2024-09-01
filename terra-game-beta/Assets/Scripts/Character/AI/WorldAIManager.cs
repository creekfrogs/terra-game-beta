using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

    [Header("DEBUG")]
    [SerializeField] bool despawnCharacters = false;
    [SerializeField] bool respawnCharacters = false;

    [Header("Characters")]
    [SerializeField] GameObject[] aiCharacters;
    [SerializeField] List<GameObject> spawnedCharacters;

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

    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            StartCoroutine(SpawnAfterLoad());
        }
    }

    private void Update()
    {
        if(respawnCharacters)
        {
            respawnCharacters = false;
            SpawnAllCharacters();
        }
        if(despawnCharacters)
        {
            despawnCharacters = false;
            DespawnAllCharacters();
        }
    }

    private IEnumerator SpawnAfterLoad()
    {
        while(!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
        SpawnAllCharacters();
    }

    private void SpawnAllCharacters()
    {
        foreach (var character in aiCharacters)
        {
            GameObject instantiatedCharacter = Instantiate(character);
            instantiatedCharacter.GetComponent<NetworkObject>().Spawn();
            spawnedCharacters.Add(instantiatedCharacter);
        }
    }

    private void DespawnAllCharacters()
    {
        foreach(var character in spawnedCharacters)
        {
            character.GetComponent<NetworkObject>().Despawn();
        }
    }

    private void DisableAllCharacters()
    {
        //Disable Character Gameobjects sync on network
        //Disable gameobjects for clients upon connecting if disabled status is true
        //Can be used to disable characters that are far from players to save memory
        //Characters can be split into areas.
    }
}
