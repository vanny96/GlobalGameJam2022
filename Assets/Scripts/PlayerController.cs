using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float speed;

    public override void FixedUpdateNetwork()
    {
        if (GetInput<PirateGameInput>(out PirateGameInput input) == false) return;

        Move(input.Buttons);
    }

    private void Move(NetworkButtons buttons)
    {
        Vector3 movement = new Vector3();

        if (buttons.IsSet(PirateButtons.Forward)) movement.y++;
        if (buttons.IsSet(PirateButtons.Backward)) movement.y--;
        if (buttons.IsSet(PirateButtons.Right)) movement.x++;
        if (buttons.IsSet(PirateButtons.Left)) movement.x--;

        Debug.Log(movement);

        transform.Translate(movement * speed * Runner.DeltaTime);
    }
}
