using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameSceneManager gameSceneManager;

    [SerializeField]
    private float maxX;

    [SerializeField]
    private float maxZ;

    [SerializeField]
    private float minX;

    [SerializeField]
    private float minZ;

    [SerializeField] private Vector3 relativeCameraPosition;
    private Transform playerAvatar = null;

    void Start()
    {
        var playerObject = gameSceneManager.GetMyPlayerObject();
        playerAvatar = playerObject.transform.Find("PlayerView");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(playerAvatar.position.x + relativeCameraPosition.x, minX, maxX),
                                         playerAvatar.position.y + relativeCameraPosition.y,
                                         Mathf.Clamp(playerAvatar.position.z + relativeCameraPosition.z, minZ, maxZ));
    }
}
