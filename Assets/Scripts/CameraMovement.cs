using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private GameSceneManager gameSceneManager;

    [SerializeField] private Vector3 relativeCameraPosition;
    private Transform playerAvatar= null;
    

    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (playerAvatar == null)
        {
            var playerObject = gameSceneManager.GetMyPlayerObject();
            if (playerObject == null) return;
        
            playerAvatar=playerObject.transform.Find("PlayerView");
        }
        else
        {
            transform.position = playerAvatar.position + relativeCameraPosition;
        }

        
    }
}
