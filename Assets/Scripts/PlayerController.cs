using UnityEngine;
using Fusion;
using System.Linq;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class PlayerController : NetworkBehaviour
{
    private static int counter = 0;
    [SerializeField] private float speed;

    [SerializeField] private float siphonCooldown;
    [Networked] private float currentSiphonCooldown { get; set; } = 0;
    [Networked] public NetworkBool siphoning { get; set; } = false; 

    private new NetworkCharacterController characterController;
    private StepsSoundController stepsSoundController;
    private TreasureHolder treasureHolder;
    public Animator animator;

    [SerializeField]private GameObject[] skins;

    [Networked(OnChanged = nameof(MyCallbackMethod))]
    public int skin { get; set; } = -1;
    [SerializeField] private TreasureHolderTrigger treasureHolderTrigger;
    [SerializeField] StepsSoundController soundController;
    private static readonly int Y = Animator.StringToHash("Y");
    private static readonly int X = Animator.StringToHash("X");
    private static readonly int Running = Animator.StringToHash("Running");
    
    [Networked] private NetworkBool wasMoving { get; set; }


    private float animationXdirection = 1;
    private float animationYdirection = 1;

    [SerializeField] private int treasureThresholdForBeacon;
    [Networked] public NetworkBool isBeacon { get; set; }

    protected static void MyCallbackMethod(Changed<PlayerController> changed)
    {
        //Debug.Log("changed skin");
        //changed.Behaviour.ApplySkin();

    }

    static int GetSkin(PlayerRef playerRef)
    {
        
        /*var playerSkins  = new Dictionary<PlayerRef,int>();
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            var playerController = player.GetComponent<PlayerController>();
            if (playerController.skin != -1)
            {
                playerSkins[playerController.Object.InputAuthority] = playerController.skin;
            }
            Debug.Log("playerController.skin:"+playerController.skin.ToString());

        }

        
        if (playerSkins.ContainsKey(playerRef))
        {
            return playerSkins[playerRef];
        }*/

        var skin = Random.Range(0, 4);//-playerSkins.Count());
        /*while (playerSkins.ContainsValue(skin))
        {

            skin += 1;
        }*/
        Debug.Log("rng: "+skin.ToString());
        //playerSkins[playerRef] = skin;
        return skin;
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
    }

    public override void Spawned()
    {
        base.Spawned();
        var manager=GameObject.Find("SceneManager").GetComponent<GameSceneManager>();
        manager.AddToDirectory(Object.InputAuthority, Object);

        manager.RetrieveSkin(Object.InputAuthority, this);


        /* if (Object.HasInputAuthority)
         {
             skin = GetSkin(Object.InputAuthority);
             
         }
         else
         {
             ApplySkin();
         }*/







        //Debug.Log("playerRef: " + Object.InputAuthority.ToString() + " ---- " + skin.ToString());
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
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput<PirateGameInput>(out PirateGameInput input) == false) return;

        Move(input.Buttons);
        Siphon(input.Buttons);
        Debug.Log(Object.InputAuthority.ToString()
                  +"hello");
    }

    public void OnGainedCoin()
    {
        if(treasureHolder.treasure > treasureThresholdForBeacon)
        {
            isBeacon = true;
        }
    }

    public void OnLostCoin()
    {
        if (treasureHolder.treasure < treasureThresholdForBeacon)
        {
            isBeacon = false;
        }

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

        movement.Normalize();

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

                Debug.Log("Siphon number " + ++counter);
            }
        }
    }

    private void UpdateSoundController(bool isMoving, NetworkBool wasMoving)
    {
        if (isMoving && !wasMoving) stepsSoundController.StartWalking();
        if (wasMoving && !isMoving) stepsSoundController.StopWalking();
    }
}
