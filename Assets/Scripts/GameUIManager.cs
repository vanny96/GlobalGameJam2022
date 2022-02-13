using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class GameUIManager : MonoBehaviour
{
    [SerializeField] private GameSceneManager gameSceneManager;
    [SerializeField] private TextMeshProUGUI treasureText;
    [SerializeField] private TreasureHolder playerTreasure= null;

    [SerializeField] private Volume postprocessingVolume;
    [SerializeField] private float finalVignetteIntensity;
    [SerializeField] private float vignetteSpeed;
    private IEnumerator runningVignetteCoroutine;

    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float messageTextSpeed;
    [SerializeField] private float messageTextDuration;
    [SerializeField] private float messageTextPosition;
    private float messageTextStartPosition;

    private Queue<string> messagesQueue = new Queue<string>();
    private bool showingMessage = false;

    [SerializeField] private GameObject errorPopupPanel;

    void Start()
    {
        messageTextStartPosition = messageText.transform.position.y;
    }

    void Update()
    {
        UpdateTreasure();
        CheckForMessages();
    }

    public void HideErrorMessages()
    {
        errorPopupPanel.SetActive(false);
    }

    // Treasure
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

    // Vignette
    public void ShowVignette()
    {
        HandleVignette(true);
    }

    public void HideVignette()
    {
        HandleVignette(false);
    }

    private void HandleVignette(bool show)
    {
        if (runningVignetteCoroutine != null)
            StopCoroutine(runningVignetteCoroutine);

        runningVignetteCoroutine = show ? ShowVignetteCoroutine() : HideVignetteCoroutine();
        StartCoroutine(runningVignetteCoroutine);
    }

    private IEnumerator ShowVignetteCoroutine()
    {
        postprocessingVolume.gameObject.SetActive(true);
        Vignette vignetteEffect = (Vignette) postprocessingVolume.profile.components[0];
        
        while(vignetteEffect.intensity.value < finalVignetteIntensity)
        {
            vignetteEffect.intensity.value += Math.Min(
                vignetteSpeed * Time.deltaTime,
                finalVignetteIntensity - vignetteEffect.intensity.value
                );

            yield return null;
        }

        runningVignetteCoroutine = null;
    }

    private IEnumerator HideVignetteCoroutine()
    {
        Vignette vignetteEffect = (Vignette)postprocessingVolume.profile.components[0];

        while (vignetteEffect.intensity.value > 0)
        {
            vignetteEffect.intensity.value -= Math.Min(
                vignetteSpeed * Time.deltaTime,
                vignetteEffect.intensity.value
                );

            yield return null;
        }

        postprocessingVolume.gameObject.SetActive(false);
        runningVignetteCoroutine = null;
    }

    // Messages
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
