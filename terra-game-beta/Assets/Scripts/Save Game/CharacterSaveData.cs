using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterSaveData
{
    [Header("Character Name")]
    public string characterName = "";

    [Header("Time Played")]
    public float secondsPlayed;

    [Header("World Coordinates")]
    public float xPosition;
    public float yPosition;
    public float zPosition;

    [Header("Stats")]
    public int essence;
    public int vitality;
    public int force;
    public int finesse;
    public int focus;
    public int precision;

    [Header("Resources")]
    public float currentHealth;
    public float currentStamina;
}
