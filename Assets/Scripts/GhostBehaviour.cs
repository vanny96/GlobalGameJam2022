using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class GhostBehaviour : NetworkBehaviour
{
    [Networked] [HideInInspector] public NetworkBool Angry { get; set; }
    [Networked] private Vector3 currentDestination { get; set; }
    [SerializeField] [Networked] private float speed { get; set; }

    [SerializeField] private float siphonCooldown;
    [Networked] private float currentSiphonCooldown { get; set; } = 0;

    [HideInInspector] public Bounds destinationBounds;

    [SerializeField] private GhostSightTrigger ghostSightTrigger;
    [SerializeField] private TreasureHolderTrigger treasureHolderTrigger;

    private Rigidbody rigidbody;
    private TreasureHolder treasureHolder;



    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        currentDestination = GetNextDestination();
        treasureHolder = GetComponent<TreasureHolder>();
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (Angry)
        {
            AngryPattern();
        }
        else
        {
            CalmPattern();
        }
    }


    public void OnTreasureChange()
    {
        if (Angry && treasureHolder.TreasureIsAtStartAmount())
        {
            Angry = false;
        }
        else if (treasureHolder.TreasureIsEmpty())
        {
            Angry = true;
        }
    }

    private void AngryPattern()
    {
        if (ghostSightTrigger.ActiveTreasureHolder == null)
        {
            CalmPattern();
        }
        else
        {
            FollowPlayer();
            StealFromPlayer();
        }

    }

    private void CalmPattern()
    {
        MoveToDestination(currentDestination);

        if (transform.position == currentDestination)
            currentDestination = GetNextDestination();
    }


    private void FollowPlayer()
    {
        Vector3 playerPosition = ghostSightTrigger.ActiveTreasureHolder.transform.position;
        MoveToDestination(playerPosition);
    }

    private void StealFromPlayer()
    {
        if (currentSiphonCooldown >= 0) currentSiphonCooldown -= Runner.DeltaTime;

        if (currentSiphonCooldown <= 0f)
        {
            TreasureHolder activeTreasureHolder = treasureHolderTrigger.ActiveTreasureHolder;

            if (activeTreasureHolder != null)
            {
                int stolenAmount = activeTreasureHolder.GiveTreasure();
                treasureHolder.TakeTreasure(stolenAmount);

                currentSiphonCooldown = siphonCooldown;
            }
        }

    }

    private void MoveToDestination(Vector3 destination)
    {
        Vector3 direction = (destination - transform.position);
        Vector3 movement = direction.normalized * speed * Runner.DeltaTime;

        if (movement.magnitude > direction.magnitude) movement = direction;

        Vector3 endPosition = transform.position + movement;
        endPosition.y = transform.position.y;

        rigidbody.MovePosition(endPosition);
    }


    private Vector3 GetNextDestination()
    {
        return RandomCoordinates.FromBoundsAndY(destinationBounds, transform.position.y);
    }
}
