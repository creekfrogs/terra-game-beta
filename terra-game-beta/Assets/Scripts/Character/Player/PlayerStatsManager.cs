using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : CharacterStatsManager
{
    PlayerManager player;
    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Start()
    {
        base.Start();

        CalculateHealthBasedOnLvl(player.playerNetworkManager.essence.Value);
        CalculateStaminaBasedOnLvl(player.playerNetworkManager.vitality.Value);
    }
}
