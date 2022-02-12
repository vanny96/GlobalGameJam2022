using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private GameSceneManager gameSceneManager;

    [SerializeField] private Vector3 relativeCameraPosition;
    private Transform playerAvatar= null;

    [SerializeField]
    private float maxX;

    [SerializeField]
    private float maxZ;

    [SerializeField]
    private float minX;

    [SerializeField]
    private float minZ;


    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (playerAvatar == null)
        {
            var playerObject = gameSceneManager.GetMyPlayerObject();
            //Debug.Log(playerObject);
            if (playerObject == null) return;
        
            playerAvatar=playerObject.transform.Find("PlayerView");
        }
        else
        {
            transform.position = new Vector3(Mathf.Clamp(playerAvatar.position.x + relativeCameraPosition.x, minX, maxX),
                                         playerAvatar.position.y + relativeCameraPosition.y,
                                         Mathf.Clamp(playerAvatar.position.z + relativeCameraPosition.z, minZ, maxZ));
        }      
    }
}
