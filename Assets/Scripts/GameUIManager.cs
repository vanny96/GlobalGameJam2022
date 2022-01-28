using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private GameSceneManager gameSceneManager;

    [SerializeField] private TextMeshProUGUI treasureText;

    [SerializeField] private PlayerController playerController= null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController == null)
        {
            var playerObject = gameSceneManager.GetMyPlayerObject();
            //Debug.Log(playerObject);
            if (playerObject == null) return;
        
            playerController=playerObject.GetComponent<PlayerController>();
        }
        else
        {
            treasureText.text = playerController.treasure.ToString();
        }
    }
}
