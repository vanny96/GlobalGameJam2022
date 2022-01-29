using UnityEngine;
using Fusion;
using System.Linq;
using System;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float speed;

    [SerializeField] private float siphonCooldown;
    [Networked] private float currentSiphonCooldown { get; set; } = 0;
    [Networked] public NetworkBool siphoning { get; set; } = false; 

    private new NetworkCharacterController characterController;
    private StepsSoundController stepsSoundController;
    private TreasureHolder treasureHolder;
    private Animator animator;

   /* [SerializeField] private PlayerTriggerDetection leftTrigger;
    [SerializeField] private PlayerTriggerDetection rightTrigger;
    [SerializeField] private PlayerTriggerDetection forwardTrigger;
    [SerializeField] private PlayerTriggerDetection backwardTrigger;*/

    [SerializeField] private TreasureHolderTrigger treasureHolderTrigger;
    [SerializeField] StepsSoundController soundController;
    private static readonly int Y = Animator.StringToHash("Y");
    private static readonly int X = Animator.StringToHash("X");
    private static readonly int Running = Animator.StringToHash("Running");

    [Networked] private NetworkBool wasMoving { get; set; }


    void Awake()
    {
        characterController = GetComponent<NetworkCharacterController>();
        stepsSoundController = GetComponent<StepsSoundController>();
        treasureHolder = GetComponent<TreasureHolder>();
    }

    void Start()
    {
        wasMoving = true;
    }

    public override void Spawned()
    {
        base.Spawned();
        GameObject.Find("SceneManager").GetComponent<GameSceneManager>().AddToDirectory(Object.InputAuthority, Object);
        animator = GetComponentInChildren<Animator>() ;

        
    }

    public override void FixedUpdateNetwork()
    {
       // Debug.Log(treasure);
       //if (Object.HasInputAuthority)
       {
           if (GetInput<PirateGameInput>(out PirateGameInput input) == false) return;

           Move(input.Buttons);
           Siphon(input.Buttons);
       }
    }

    public void OnLostCoin()
    {
        if (treasureHolder.treasure == 0)
            CallGameOver();
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void CallGameOver()
    {
        Runner.SetActiveScene(1);
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
        movement.Normalize();
        characterController.Move( movement);
        UpdateAnimation(movement, isMoving);
        wasMoving = isMoving;
        
    }

    private void UpdateAnimation(Vector3 direction, bool moving)
    {
        animator.SetInteger(X,Mathf.RoundToInt(direction.x)); 
        animator.SetInteger(Y,Mathf.RoundToInt(direction.z)); 
        animator.SetBool(Running,moving);
    }

    private void Siphon(NetworkButtons buttons)
    {
        siphoning = buttons.IsSet(PirateButtons.Space);

        if (currentSiphonCooldown >= 0) currentSiphonCooldown -= Runner.DeltaTime;

        if(siphoning && currentSiphonCooldown <= 0f)
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

    private void UpdateSoundController(bool isMoving, NetworkBool wasMoving)
    {
        if (isMoving && !wasMoving) stepsSoundController.StartWalking();
        if (wasMoving && !isMoving) stepsSoundController.StopWalking();
    }
}
