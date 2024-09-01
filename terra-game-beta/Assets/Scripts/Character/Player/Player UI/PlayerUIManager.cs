using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PlayerUIManager : MonoBehaviour
{
    [HideInInspector] public PlayerManager player;

    [Header("NETWORK JOIN")]
    [SerializeField] bool startGameAsClient;

    public static PlayerUIManager instance;

    [HideInInspector] public PlayerUIHudManager playerUIHudManager;
    [HideInInspector] public PlayerUIPopUpManager playerUIPopUpManager;

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

        player = GetComponent<PlayerManager>();
        playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>();
        playerUIPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>();
    }

    private void Start()
    {

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (startGameAsClient)
        {
            startGameAsClient = false;
            NetworkManager.Singleton.Shutdown();
            NetworkManager.Singleton.StartClient();
        }
    }
}
