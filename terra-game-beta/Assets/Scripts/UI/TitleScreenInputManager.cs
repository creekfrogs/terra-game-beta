using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenInputManager : MonoBehaviour
{
    PlayerControls playerControls;

    [Header("Title Screen Inputs")]
    [SerializeField] bool deleteSaveSlot = false;

    private void Update()
    {
        if(deleteSaveSlot)
        {
            deleteSaveSlot = false;
            TitleScreenManager.instance.AttemptDeleteSlot();
        }
    }

    private void OnEnable()
    {
        if(playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.UI.X.performed += i => deleteSaveSlot = true;
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
