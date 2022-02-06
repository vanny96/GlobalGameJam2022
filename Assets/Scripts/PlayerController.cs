using UnityEngine;
using Fusion;
using System.Linq;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using TMPro;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float speed;

    [SerializeField] private float siphonCooldown;
    [Networked] private float currentSiphonCooldown { get; set; } = 0;
    [Networked] public NetworkBool siphoning { get; set; } = false;

    [SerializeField] private int treasureThresholdForBeacon;

    private NetworkCharacterController characterController;
    private StepsSoundController stepsSoundController;
    private TreasureHolder treasureHolder;
    public Animator animator;

    [SerializeField]private GameObject[] skins;

    [Networked]
    public int skin { get; set; } = -1;

    private MainSoundController mainSoundController;
    private GameUIManager gameUIManager;

    [SerializeField] private TreasureHolderTrigger treasureHolderTrigger;
    [SerializeField] StepsSoundController soundController;
    private static readonly int Y = Animator.StringToHash("Y");
    private static readonly int X = Animator.StringToHash("X");
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int Steal = Animator.StringToHash("Steal");
    private static readonly int Stunned = Animator.StringToHash("Stunned");

    [Networked(OnChanged = nameof(NameChangeCallBack))][Capacity(32)] public string playerName { get; set; }
    [Networked] private NetworkBool wasMoving { get; set; }


    private float animationXdirection = 1;
    private float animationYdirection = 1;

    [SerializeField] private ParticleSystem coinEffect;
    
    [SerializeField] private GameObject beaconTarget;
    [Networked(OnChanged = nameof(BeaconCallBack))] public NetworkBool isBeacon { get; set; }

    protected static void BeaconCallBack(Changed<PlayerController> changed)
    {
        changed.Behaviour.beaconTarget.SetActive(changed.Behaviour.isBeacon);
    }
    protected static void NameChangeCallBack(Changed<PlayerController> changed)
    {
        changed.Behaviour.ApplyName();
    }
    
    void Awake()
    {
        characterController = GetComponent<NetworkCharacterController>();
        stepsSoundController = GetComponent<StepsSoundController>();
        treasureHolder = GetComponent<TreasureHolder>();
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

        Debug.Log(skins[skin].name);
        var skinObject = skins[skin];
        Debug.Log(skinObject);
        skinObject.SetActive(true);
        //var skinObject = Instantiate(skins[skin],
        //    playerView.transform);
        
        //skinObject.transform.localPosition = new Vector3(0, -0.6f, 0);
        //skinObject.transform.Rotate(new Vector3(45, 0, 0));
        animator = skinObject.GetComponentInChildren<Animator>();

        GameObject.Find("SceneManager").GetComponent<GameSceneManager>().AddToDirectory(Object.InputAuthority, Object);
        animator = GetComponentInChildren<Animator>();
        //beaconTarget = skinObject.transform.Find("X").gameObject;
        beaconTarget.SetActive(isBeacon);
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

        Move(input.Buttons);
        Siphon(input.Buttons);
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
        siphoning = buttons.IsSet(PirateButtons.Steal);

        if (currentSiphonCooldown >= 0) currentSiphonCooldown -= Runner.DeltaTime;

        if(siphoning && currentSiphonCooldown <= 0f)
        {
            animator.SetTrigger("Steal");

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
