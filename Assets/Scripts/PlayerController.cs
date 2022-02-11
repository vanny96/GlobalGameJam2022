using UnityEngine;
using Fusion;
using System;
using TMPro;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float speed;

    [SerializeField] private float siphonCooldown;
    [Networked] private float currentSiphonCooldown { get; set; } = 0;

    [SerializeField] private int treasureThresholdForBeacon;

    private NetworkCharacterController characterController;
    private StepsSoundController stepsSoundController;
    private TreasureHolder treasureHolder;
    private StunEntity stunEntity;
    public Animator animator;

    [SerializeField]private GameObject[] skins;

    [Networked] public int skin { get; set; } = -1;

    private MainSoundController mainSoundController;
    private GameUIManager gameUIManager;

    [SerializeField] private TargetColliderTrigger targetColliderTrigger;
    [SerializeField] StepsSoundController soundController;

    [Networked(OnChanged = nameof(NameChangeCallBack))][Capacity(32)] public string playerName { get; set; }
    [Networked] private NetworkBool wasMoving { get; set; }

    [SerializeField] private ParticleSystem coinEffect;
    
    [SerializeField] private GameObject beaconTarget;
    [Networked(OnChanged = nameof(BeaconCallBack))] public NetworkBool isBeacon { get; set; }

    private static readonly int Y = Animator.StringToHash("Y");
    private static readonly int X = Animator.StringToHash("X");
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int Steal = Animator.StringToHash("Steal");
    private static readonly int Stunned = Animator.StringToHash("Stunned");
    private float animationXdirection = 1;
    private float animationYdirection = -1;

    void Awake()
    {
        characterController = GetComponent<NetworkCharacterController>();
        stepsSoundController = GetComponent<StepsSoundController>();
        treasureHolder = GetComponent<TreasureHolder>();
        stunEntity = GetComponent<StunEntity>();
    }

    void Start()
    {
        wasMoving = true;

        if (Object.HasInputAuthority)
        {
            gameObject.AddComponent<AudioListener>();
            Destroy(FindObjectOfType<Camera>().GetComponent<AudioListener>());

            mainSoundController = GameObject.Find("MainSoundController").GetComponent<MainSoundController>();

            if (Object.HasInputAuthority)
            {
                mainSoundController.musicPlay();
            }
        }

        gameUIManager = FindObjectOfType<GameUIManager>();
    }

    public override void Spawned()
    {
        base.Spawned();

        var manager=GameObject.Find("SceneManager").GetComponent<GameSceneManager>();
        manager.AddToDirectory(Object.InputAuthority, Object);
        if (Object.HasInputAuthority)
        {
            manager.RetrieveName(this);
        }

        manager.RetrieveSkin(Object.InputAuthority, this);
    }

    public void ApplySkin()
    {
        var playerView = transform.Find("PlayerView");
        for (var i =0;i<playerView.childCount;i++)
        {
            for (var j = 0; j < skins.Length; j++)
            {
                if (skins[j].name == playerView.GetChild(i).gameObject.name)
                {
                    skins[j] = playerView.GetChild(i).gameObject;
                    skins[j].SetActive(false);
                }
            }
        }

        var skinObject = skins[skin];
        skinObject.SetActive(true);

        GameObject.Find("SceneManager").GetComponent<GameSceneManager>().AddToDirectory(Object.InputAuthority, Object);
        animator = GetComponentInChildren<Animator>();

        beaconTarget.SetActive(isBeacon);
    }

    protected static void BeaconCallBack(Changed<PlayerController> changed)
    {
        changed.Behaviour.beaconTarget.SetActive(changed.Behaviour.isBeacon);
    }

    protected static void NameChangeCallBack(Changed<PlayerController> changed)
    {
        changed.Behaviour.ApplyName();
    }

    public void ApplyName()
    {
        var nameText = transform.Find("NameTag").GetChild(0);
        nameText.GetComponent<TextMeshPro>().text = playerName;
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_UpdatePlayerName(string newName){
        playerName = newName;

    }


    public override void FixedUpdateNetwork()
    {
        if (GetInput<PirateGameInput>(out PirateGameInput input) == false) return;

        if (stunEntity.IsStunned()) return;
        
        Move(input.Buttons);
        Siphon(input.Buttons);
        Stun(input.Buttons);
    }

    public void OnGainedCoin()
    {
        if (treasureHolder.treasure >= treasureThresholdForBeacon)
        {
            isBeacon = true;
            RPC_NotifyBeacon("playername");
        }

        var emission = coinEffect.emission;
        emission.rateOverTime = treasureHolder.treasure;
        
        if (Object.HasInputAuthority)
        {
            mainSoundController.SiphonSound();
        }

    }

    public void OnLostCoin()
    {
        if (Object.HasInputAuthority)
        {
            mainSoundController.CoinSteal();
        }

        if (treasureHolder.treasure < treasureThresholdForBeacon)
        {
            isBeacon = false;
        }

        var emission = coinEffect.emission;
        emission.rateOverTime = treasureHolder.treasure;

        if (treasureHolder.treasure == 0)
            RPC_CallGameOver();
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    public void RPC_NotifyBeacon(String player)
    {
        gameUIManager.StartCoroutine(gameUIManager.ShowMessage(player + " got the beacon"));
    }

    public void OnTargeted()
    {
        if (Object.HasInputAuthority)
        {
            mainSoundController.GhostLockOn();
        }
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    private void RPC_CallGameOver()
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

        movement.Normalize();
        characterController.Velocity = new Vector3(characterController.Velocity.x, 0, characterController.Velocity.z);
        UpdateSoundController(isMoving, wasMoving);
        characterController.Move( movement);
        UpdateAnimation(movement, isMoving);
        
        wasMoving = isMoving;
        
    }

    private void UpdateAnimation(Vector3 direction, bool moving)
    {
        if (animator != null)
        {
            if (direction.x > 0.1f) animationXdirection = 1;
            if (direction.x < -0.1f) animationXdirection = -1;
            if (direction.z > 0.1f) animationYdirection = 1;
            if (direction.z < -0.1f) animationYdirection = -1;
            
            animator.SetFloat(X,animationXdirection); 
            animator.SetFloat(Y,animationYdirection); 
            animator.SetBool(Running,moving);
        }

    }

    private void Siphon(NetworkButtons buttons)
    {
        if (currentSiphonCooldown >= 0) currentSiphonCooldown -= Runner.DeltaTime;

        bool stealing = buttons.IsSet(PirateButtons.Steal);
        bool giving = buttons.IsSet(PirateButtons.Give);

        TreasureHolder targetTreasureHolder = targetColliderTrigger.activeTarget?.treasureHolder;

        if(targetTreasureHolder != null && currentSiphonCooldown <= 0f)
        {
            if (stealing)
            {
                animator.SetTrigger("Steal");
                currentSiphonCooldown = siphonCooldown;

                treasureHolder.MoveMoney(targetTreasureHolder, treasureHolder);
            }
            else if (giving)
            {
                currentSiphonCooldown = siphonCooldown;

                treasureHolder.MoveMoney(treasureHolder, targetTreasureHolder);
            }
        }
    }

    private void Stun(NetworkButtons buttons)
    {
        bool stealing = buttons.IsSet(PirateButtons.Stun);

        StunEntity targetStunEntity = targetColliderTrigger.activeTarget?.stunEntity;

        if(stealing && targetStunEntity != null)
        {
            bool targetStunned = stunEntity.Stun(targetStunEntity);

            if(targetStunned && Object.HasInputAuthority)
            {
                mainSoundController.StunSound();
            }
        }
    }

    public void OnStunned()
    {
        animator.SetFloat(X, animationXdirection);
        animator.SetFloat(Y, animationYdirection);
        animator.SetBool(Stunned, stunEntity.IsStunned());
    }

    private void UpdateSoundController(bool isMoving, NetworkBool wasMoving)
    {
        if (isMoving && !wasMoving) stepsSoundController.StartWalking();
        if (wasMoving && !isMoving) stepsSoundController.StopWalking();
    }
}
