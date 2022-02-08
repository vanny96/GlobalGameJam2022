using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class GhostBehaviour : NetworkBehaviour
{
    [Networked] [HideInInspector] public NetworkBool Angry { get; set; }
    [Networked] private Vector3 currentDestination { get; set; }

    [SerializeField] [Networked] private float normalSpeed { get; set; }
    [SerializeField] [Networked] private float angrySpeed { get; set; }

    [SerializeField] private float siphonCooldown;
    [Networked] private float currentSiphonCooldown { get; set; } = 0;

    [HideInInspector] public Bounds destinationBounds;

    [SerializeField] private GhostSightTrigger ghostSightTrigger;
    [SerializeField] private TargetColliderTrigger targetColliderTrigger;
    [SerializeField] private ParticleSystem coinEffect;

    [SerializeField] private NetworkPrefabRef movingCoinPrefab;

    private new Rigidbody rigidbody;
    private TreasureHolder treasureHolder;
    private Animator animator;
    private StunEntity stunEntity;

    private static readonly int Angry1 = Animator.StringToHash("Angry");
    private static readonly int Y = Animator.StringToHash("Y");
    private static readonly int X = Animator.StringToHash("X");

    private float animationXdirection = 1;
    private float animationYdirection = 1;


    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        currentDestination = GetNextDestination();
        treasureHolder = GetComponent<TreasureHolder>();
        stunEntity = GetComponent<StunEntity>();
    }

    public override void FixedUpdateNetwork()
    {
        if (stunEntity.IsStunned()) return;

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
        var emission = coinEffect.emission;
        emission.rateOverTime = treasureHolder.treasure;

        if (!treasureHolder.TreasureIsAtStartAmount())
        {
            Angry = true;
            if (ghostSightTrigger.ActiveTreasureHolder != null)
            {
                ghostSightTrigger.ActiveTreasureHolder.GetComponent<PlayerController>().OnTargeted();
            }
        }

        if (Angry && treasureHolder.TreasureIsAtStartAmount())
        {
            Angry = false;
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
        MoveToDestination(currentDestination, normalSpeed);

        if (transform.position == currentDestination)
            currentDestination = GetNextDestination();
    }


    private void FollowPlayer()
    {
        Vector3 playerPosition = ghostSightTrigger.ActiveTreasureHolder.transform.position;
        MoveToDestination(playerPosition, angrySpeed);
    }

    private void StealFromPlayer()
    {
        if (currentSiphonCooldown >= 0) currentSiphonCooldown -= Runner.DeltaTime;

        if (currentSiphonCooldown <= 0f)
        {
            TreasureHolder activeTreasureHolder = targetColliderTrigger.activeTarget?.treasureHolder;

            if (activeTreasureHolder != null)
            {
                int stolenAmount = activeTreasureHolder.GiveTreasure();
                treasureHolder.TakeTreasure(stolenAmount);

                currentSiphonCooldown = siphonCooldown;

                NetworkObject coinNO = Runner.Spawn(movingCoinPrefab, activeTreasureHolder.transform.position, Quaternion.identity);
                coinNO.GetComponent<StolenCoinBehaviour>().target = this.transform;
            }
        }
    }

    private void MoveToDestination(Vector3 destination, float speed)
    {
        Vector3 direction = (destination - transform.position);
        Vector3 movement = direction.normalized * speed * Runner.DeltaTime;

        if (movement.magnitude > direction.magnitude) movement = direction;

        var position = transform.position;
        Vector3 endPosition = position + movement;
        endPosition.y = position.y;
        UpdateAnimation((endPosition-position).normalized,Angry);
        if (!rigidbody)
        {
            rigidbody = GetComponent<Rigidbody>();
        }
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
