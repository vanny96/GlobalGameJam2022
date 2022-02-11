using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private GameSceneManager gameSceneManager;
    [SerializeField] private TextMeshProUGUI treasureText;
    [SerializeField] private TreasureHolder playerTreasure= null;

    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float messageTextSpeed;
    [SerializeField] private float messageTextDuration;
    [SerializeField] private float messageTextPosition;
    private float messageTextStartPosition;

    void Start()
    {
        messageTextStartPosition = messageText.transform.position.y;
    }

    void Update()
    {
        if (playerTreasure == null)
        {
            var playerObject = gameSceneManager.GetMyPlayerObject();

            if (playerObject == null) return;
        
            playerTreasure=playerObject.GetComponent<TreasureHolder>();
        }
        else
        {
            treasureText.text = playerTreasure.treasure.ToString();
        }
    }

    public IEnumerator ShowMessage(string message)
    {
        messageText.text = message;

        Transform messageTransform = messageText.transform;
        while (messageTransform.position.y < messageTextPosition)
        {
            MoveAndAdjustMessage(messageTextPosition, Vector2.up);
            yield return 1;
        }

        yield return new WaitForSeconds(messageTextDuration);

        while (messageText.transform.position.y > messageTextStartPosition)
        {
            MoveAndAdjustMessage(messageTextStartPosition, Vector2.down);
            yield return 1;
        }

    }

    private void MoveAndAdjustMessage(float position, Vector2 direction)
    {
        Transform messageTransform = messageText.transform;
        messageTransform.Translate(direction * Time.deltaTime * messageTextSpeed);

        bool overcommittedDown = direction == Vector2.down && messageTransform.position.y < position;
        bool overcommitedUp = direction == Vector2.up && messageTransform.position.y > position;

        if (overcommittedDown || overcommitedUp)
        {
            messageTransform.position = new Vector2(messageTransform.position.x, position);
        }
    }
}
