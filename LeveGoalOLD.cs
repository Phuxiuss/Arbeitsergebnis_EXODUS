using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelGoal : MonoBehaviour
{
    
    [SerializeField] private int desiredNPCAmount;
    [SerializeField] public static Action levelFinished;
    [SerializeField] private Animation remainingAnimation;
    [SerializeField] private Animation thanksForPlayingAnimation;
    [SerializeField] private TextMeshProUGUI levelCompleted;
    [SerializeField] private TextMeshProUGUI remaining;
    [SerializeField] private UnityEvent endFightOver;

    [SerializeField] private float fadeToMainMenuTime;
    private int NPCsEntered;
    private bool playerWon = false;
    private int currentNpcAlive;
    private int currentSceneIndex;

    private int[] playableLevels = new int[] {3, 4, 5};
    
    private int currentGroupNumber;
    private int remainingGroups;
    public void Initialize(int desiredNPCAmount)
    {
        this.desiredNPCAmount = desiredNPCAmount;
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        currentGroupNumber = Array.IndexOf(playableLevels, currentSceneIndex) + 1;

        if (currentGroupNumber <= 0)
        {
            Debug.LogWarning($"Scene {currentSceneIndex} is not listed as a playable level!");
            return;
        }
        
        remainingGroups = playableLevels.Length - currentGroupNumber;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<ConvoyNPC>(out ConvoyNPC convoyNPC))
        {
            currentNpcAlive = ConvoyAndEnemyNotifier.instance.GetListLength();
            NPCsEntered++;

            bool isLastPlayableLevel = (currentGroupNumber == playableLevels.Length);
            
            if(NPCsEntered >= desiredNPCAmount && NPCsEntered >= currentNpcAlive && !playerWon && !isLastPlayableLevel)
            { 
                SetupText();
                remainingAnimation.Play("FadeInLevelGoal");
                SoundManager.PlaySoundWithDelay(SoundType.UI_PLAY, 1f, 1.8f);
                playerWon = true;
                StartCoroutine(SwitchSceneLevel());
            }
            else if (NPCsEntered >= desiredNPCAmount && NPCsEntered >= currentNpcAlive && !playerWon && isLastPlayableLevel)
            {
                //thanksForPlayingAnimation.Play("ThanksForPlaying");
                StartCoroutine(SwitchToMainMenu());
                playerWon = true;
            }
        }
    }

    public void PlayThanksForPlaying()
    {
      //  thanksForPlayingAnimation.Play("ThanksForPlaying");
        StartCoroutine(SwitchToMainMenu());
        endFightOver?.Invoke();
        playerWon = true;
    }

    private void SetupText()
    {
        levelCompleted.text = $"Group {currentGroupNumber} saved!";
        if (remainingGroups > 1)
        {
            remaining.text = $"{remainingGroups} remaining groups to save.";
        }
        else if  (remainingGroups == 1)
        {
            remaining.text = $"{remainingGroups} remaining group to save.";
        }
        else
        {
            remaining.text = $"All groups saved!";  
        }
    }

    private IEnumerator SwitchSceneLevel()
    {
        yield return new WaitForSeconds(4f);
       // Cursor.lockState = CursorLockMode.None;
       // Cursor.visible = true;
        levelFinished?.Invoke();
    }

    private IEnumerator SwitchToMainMenu()
    {
        yield return new WaitForSeconds(fadeToMainMenuTime);
      //  Cursor.lockState = CursorLockMode.None;
       // Cursor.visible = true;
        SceneController.sceneController.SwitchToMainMenuInstantly();
    }
}
