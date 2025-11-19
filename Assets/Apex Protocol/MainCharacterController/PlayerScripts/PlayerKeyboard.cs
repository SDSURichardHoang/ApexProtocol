using ApexProtocol.MainCharacterControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerKeyboard : MonoBehaviour, PlayerControls.IPlayerKeyBindsActions
{
    public PlayerControls PlayerControls;

    public Vector2 MovementInput;

    public Vector2 LookInput;

    private void OnEnable()
    {
        PlayerControls = new PlayerControls();
        PlayerControls.Enable();

        PlayerControls.PlayerKeyBinds.Enable();
        PlayerControls.PlayerKeyBinds.SetCallbacks(this);
    }


    private void OnDisable()
    {
        PlayerControls.PlayerKeyBinds.Disable();
        PlayerControls.PlayerKeyBinds.RemoveCallbacks(this);
    }




    public void OnMovement(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
        //print(MovementInput);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
        //print(LookInput);
    }
}
    

