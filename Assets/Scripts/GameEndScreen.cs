using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class GameEndScreen : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        NetworkRunner networkRunner = FindObjectOfType<NetworkRunner>();
        Destroy(networkRunner.gameObject);
    }

    public void BackToMain()
    {
        SceneManager.LoadScene(0);
    }
}
