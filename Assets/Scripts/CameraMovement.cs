using UnityEngine;
using Fusion;
using System.Linq;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Vector3 relativeCameraPosition;
    public Transform PlayerAvatar;
    

    // Start is called before the first frame update


    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerAvatar != null)
        {
            transform.position = PlayerAvatar.position + relativeCameraPosition;
        }
    }
}
