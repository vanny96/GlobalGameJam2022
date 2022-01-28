using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using System.Linq;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float speed;
    private new Rigidbody rigidbody;
    private StepsSoundController stepsSoundController;

    [SerializeField] private PlayerTriggerDetection leftTrigger;
    [SerializeField] private PlayerTriggerDetection rightTrigger;
    [SerializeField] private PlayerTriggerDetection forwardTrigger;
    [SerializeField] private PlayerTriggerDetection backwardTrigger;
    [SerializeField] StepsSoundController soundController;

    [Networked] private NetworkBool wasMoving { get; set; }


    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        stepsSoundController = GetComponent<StepsSoundController>();
    }

    void Start()
    {
        wasMoving = true;
    }

    public override void Spawned()
    {
        base.Spawned();
        GameObject.Find("SceneManager").GetComponent<GameSceneManager>().AddToDirectory(Object.InputAuthority, Object);
    }

    public override void FixedUpdateNetwork()
    {
       // Debug.Log(treasure);
        if (GetInput<PirateGameInput>(out PirateGameInput input) == false) return;

        Move(input.Buttons);
    }

    private void Move(NetworkButtons buttons)
    {
        Vector3 movement = new Vector3();

        if (buttons.IsSet(PirateButtons.Forward)) movement.z++;
        if (buttons.IsSet(PirateButtons.Backward)) movement.z--;
        if (buttons.IsSet(PirateButtons.Right)) movement.x++;
        if (buttons.IsSet(PirateButtons.Left)) movement.x--;

        bool isMoving = movement != Vector3.zero;

        UpdateSoundController(isMoving, wasMoving);

        if (isMoving && !IsBlocked(movement))
        {
            Vector3 newPosition = transform.position + (movement * speed * Runner.DeltaTime);
            rigidbody.MovePosition(newPosition);
        }

        wasMoving = isMoving;
    }

    private bool IsBlocked(Vector3 movement)
    {
        if (movement.x > 0) return ContainsBlocking(rightTrigger);
        if (movement.x < 0) return ContainsBlocking(leftTrigger);
        if (movement.z > 0) return ContainsBlocking(forwardTrigger);
        if (movement.z < 0) return ContainsBlocking(backwardTrigger);

        return false;
    }

    private bool ContainsBlocking(PlayerTriggerDetection trigger)
    {
        return trigger.CollidersInTrigger
            .Any(collider => !collider.isTrigger);
    
    }

    private void UpdateSoundController(bool isMoving, NetworkBool wasMoving)
    {
        if (isMoving && !wasMoving) stepsSoundController.StartWalking();
        if (wasMoving && !isMoving) stepsSoundController.StopWalking();
    }
}
