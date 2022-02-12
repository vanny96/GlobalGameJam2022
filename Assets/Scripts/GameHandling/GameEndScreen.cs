using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using UnityEngine.UI;

public class GameEndScreen : MonoBehaviour
{
    [SerializeField] private Text playerNameText;

    void Start()
    {
        if (playerNameText != null)
        {
            string playerName = FindObjectOfType<DataHolder>().GetData<string>("playerName");
            playerNameText.text = playerName;
        }


        NetworkRunner networkRunner = FindObjectOfType<NetworkRunner>();
        Destroy(networkRunner.gameObject);
    }

    public void BackToMain()
    {
        SceneManager.LoadScene(0);
    }
}
