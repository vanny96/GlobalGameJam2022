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
    private Animator animator;
    private static readonly int Angry1 = Animator.StringToHash("Angry");
    private static readonly int Y = Animator.StringToHash("Y");
    private static readonly int X = Animator.StringToHash("X");

    private float animationXdirection = 1;
    private float animationYdirection = 1;
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

    public override void Spawned()
    {
        base.Spawned();
        GameObject.Find("SceneManager").GetComponent<GameSceneManager>().AddToDirectory(Object.InputAuthority, Object);
        animator = GetComponentInChildren<Animator>() ;

        
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
            if(ghostSightTrigger.ActiveTreasureHolder != null)
            {
                Debug.Log("Got Angry and has a target");
                ghostSightTrigger.ActiveTreasureHolder.GetComponent<PlayerController>().OnTargeted();
            }

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

        var position = transform.position;
        Vector3 endPosition = position + movement;
        endPosition.y = position.y;
        UpdateAnimation((endPosition-position).normalized,Angry);
        rigidbody.MovePosition(endPosition);
    }

    private void UpdateAnimation(Vector3 direction, bool angry)
    {

        if (direction.x > 0.1f) animationXdirection = 1;
        if (direction.x < -0.1f) animationXdirection = -1;
        if (direction.z > 0.1f) animationYdirection = 1;
        if (direction.z < -0.1f) animationYdirection = -1;
        animator.SetFloat(X,animationXdirection); 
        animator.SetFloat(Y,animationYdirection); 
        animator.SetBool(Angry1,angry);
    }

    private Vector3 GetNextDestination()
    {
        return RandomCoordinates.FromBoundsAndY(destinationBounds, transform.position.y);
    }
}
