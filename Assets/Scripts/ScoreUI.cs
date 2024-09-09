using TMPro;
using UnityEngine;
using System.Collections;

public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance; // Singleton instance

    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    public TextMeshProUGUI messageText; // Text to display the score or win message

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initial update to display starting scores
        UpdateScoreUI();
    }

    void Update()
    {
        // Continuously update score texts based on the GoalManager
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (GoalManager.Instance != null)
        {
            player1ScoreText.text = "Player 1: " + GoalManager.Instance.player1Score;
            player2ScoreText.text = "Player 2: " + GoalManager.Instance.player2Score;
        }
        else
        {
            Debug.LogError("GoalManager.Instance is null.");
        }
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;

        // Optionally, clear the message after a few seconds using a coroutine
        StartCoroutine(ClearMessageAfterDelay(3f)); // Clear after 3 seconds
    }

    private IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.text = ""; // Clear the message
    }
}
