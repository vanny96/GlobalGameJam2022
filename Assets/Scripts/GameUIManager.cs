using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    private Queue<string> messagesQueue = new Queue<string>();
    private bool showingMessage = false;

    void Start()
    {
        messageTextStartPosition = messageText.transform.position.y;
    }

    void Update()
    {
        UpdateTreasure();
        CheckForMessages();
    }

    private void UpdateTreasure()
    {
        if (playerTreasure == null)
        {
            var playerObject = gameSceneManager.GetMyPlayerObject();

            if (playerObject == null) return;

            playerTreasure = playerObject.GetComponent<TreasureHolder>();
        }
        else
        {
            treasureText.text = playerTreasure.treasure.ToString();
        }
    }

    public new void BroadcastMessage(string message)
    {
        messagesQueue.Enqueue(message);
    }

    private void CheckForMessages()
    {
        if (!showingMessage && messagesQueue.Count != 0)
        {
            String message = messagesQueue.Dequeue();
            StartCoroutine(ShowMessage(message));
        }
    }


    private IEnumerator ShowMessage(string message)
    {
        showingMessage = true;
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
        showingMessage = false;
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
