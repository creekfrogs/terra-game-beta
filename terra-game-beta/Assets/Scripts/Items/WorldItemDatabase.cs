using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemDatabase : MonoBehaviour
{
    public static WorldItemDatabase instance;

    [SerializeField] List<Item> items = new List<Item>();
    [SerializeField] List<WeaponItem> weapons = new List<WeaponItem>();

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

        foreach (var weapon in weapons)
        {
            items.Add(weapon);
        }

        for (int i = 0; i < items.Count; i++)
        {
            items[i].itemID = i;
        }
    }
}
