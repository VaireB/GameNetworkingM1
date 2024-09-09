using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class GoalManager : MonoBehaviour
{
    public static GoalManager Instance;

    public int player1Score = 0;
    public int player2Score = 0;
    public int maxScore = 15;
    public BallController ballController; // Ensure this is assigned in the inspector

    public GameObject gameOverPanel; // Reference to the game over panel
    public TMPro.TextMeshProUGUI messageText; // Reference to the message text component on the panel
    public UnityEngine.UI.Button returnToLobbyButton; // Reference to the button to return to the lobby

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

    private void Start()
    {
        // Add a listener to the return to lobby button
        if (returnToLobbyButton != null)
        {
            returnToLobbyButton.onClick.AddListener(OnReturnToLobbyClicked);
        }
        else
        {
            Debug.LogError("Return to Lobby Button is not assigned.");
        }
    }

    public void CheckGoal(GameObject goal)
    {
        if (goal.CompareTag("Player1Goal"))
        {
            PlayerScored("Player2");
        }
        else if (goal.CompareTag("Player2Goal"))
        {
            PlayerScored("Player1");
        }
    }

    public void PlayerScored(string playerTag)
    {
        if (playerTag == "Player1")
        {
            player1Score++;
        }
        else if (playerTag == "Player2")
        {
            player2Score++;
        }

        Debug.Log($"{playerTag} scored! Player1: {player1Score}, Player2: {player2Score}");

        // Check if a player has reached the max score
        if (player1Score >= maxScore || player2Score >= maxScore)
        {
            EndGame(playerTag);
        }
        else
        {
            // Reset the ball after a goal
            if (ballController != null)
            {
                ballController.ResetBall();
            }
            else
            {
                Debug.LogError("BallController instance is not assigned.");
            }
        }
    }

    private void EndGame(string winningPlayer)
    {
        Debug.Log($"{winningPlayer} wins the game!");
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); // Show the game over panel
            messageText.text = $"{winningPlayer} wins!";
        }
        else
        {
            Debug.LogError("Game Over Panel is not assigned.");
        }

        // Optionally, you can add game-over logic here
        DisablePlayerControls();
        StopGame();
    }

    private void DisablePlayerControls()
    {
        // Implement logic to disable player controls here
        Debug.Log("Player controls have been disabled.");
    }

    private void StopGame()
    {
        // Implement logic to stop the game here
        Time.timeScale = 0f; // Freeze the game time
        Debug.Log("Game has been stopped.");
    }

    private void OnReturnToLobbyClicked()
    {
        // Load the lobby scene
        SceneManager.LoadScene("Lobby"); // Ensure "LobbyScene" is the name of your lobby scene
    }
}
