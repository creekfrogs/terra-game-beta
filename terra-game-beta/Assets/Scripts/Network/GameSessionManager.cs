using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager instance;
    
    [Header("Active Players In Session")]
    public List<PlayerManager> players = new List<PlayerManager>();

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

    public void AddPlayerToActivePlayers(PlayerManager player)
    {
        if(!players.Contains(player))
        {
            players.Add(player);
        }

        for (int i = players.Count - 1; i > -1; i--)
        {
            if(players[i] == null)
            {
                players.RemoveAt(i);
            }
        }
    }

    public void RemovePlayerFromActivePlayers(PlayerManager player)
    {
        if(players.Contains(player))
        {
            players.Remove(player);
        }

        for (int i = players.Count - 1; i > -1; i--)
        {
            if (players[i] == null)
            {
                players.RemoveAt(i);
            }
        }
    }
}
