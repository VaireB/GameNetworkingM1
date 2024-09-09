using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections;

public class MatchmakingManager : MonoBehaviourPunCallbacks
{
    public TMP_Text statusText;  // Reference to the status text UI element
    public float countdownTime = 5f;  // Time for countdown before the game starts
    public Button cancelButton;  // Reference to the cancel button UI element

    private float elapsedTime = 0f;  // Time elapsed while searching for a match
    private Coroutine matchmakingCoroutine;  // Coroutine reference for updating the status text
    private Coroutine countdownCoroutine;  // Coroutine reference for the countdown

    private void Start()
    {
        // Initial status message
        statusText.text = "Waiting for matchmaking...";

        // Disable the cancel button initially
        cancelButton.gameObject.SetActive(false);

        // Add a listener to the cancel button
        cancelButton.onClick.AddListener(CancelMatchmaking);
    }

    public void StartMatchmaking()
    {
        // Update status
        statusText.text = "Connecting to Photon...";

        // Enable the cancel button
        cancelButton.gameObject.SetActive(true);

        // Start the matchmaking process and the timer coroutine
        PhotonNetwork.ConnectUsingSettings();
        matchmakingCoroutine = StartCoroutine(UpdateStatusText());
    }

    private IEnumerator UpdateStatusText()
    {
        elapsedTime = 0f;

        while (true)
        {
            // Calculate minutes and seconds
            int minutes = Mathf.FloorToInt(elapsedTime / 60F);
            int seconds = Mathf.FloorToInt(elapsedTime % 60F);

            // Format the time as mm:ss
            string timeFormatted = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Update the status text with the formatted time
            statusText.text = $"Finding match... {timeFormatted} elapsed";

            elapsedTime += Time.deltaTime;

            yield return null;  // Wait for the next frame
        }
    }

    public override void OnConnectedToMaster()
    {
        statusText.text = "Connected. Joining Random Match...";

        // Attempt to join a random room
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        statusText.text = "No match found. Creating a new match...";

        // If no room is available, create a new one
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnJoinedRoom()
    {
        // Check the number of players in the room
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount < 2)
        {
            statusText.text = "Waiting for another player to join...";
        }
        else
        {
            // Update status to show match found and start the countdown
            statusText.text = "Match found! Starting countdown...";
            if (countdownCoroutine == null)
            {
                countdownCoroutine = StartCoroutine(StartCountdown());
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Check again when a new player joins
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount == 2)
        {
            // Update status to show match found and start the countdown
            statusText.text = "Match found! Starting countdown...";
            if (countdownCoroutine == null)
            {
                countdownCoroutine = StartCoroutine(StartCountdown());
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Update status if a player leaves
        statusText.text = "Player left. Waiting for another player...";

        // Stop the countdown if a player leaves
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }
    }

    private IEnumerator StartCountdown()
    {
        float timeLeft = countdownTime;

        while (timeLeft > 0)
        {
            // Update the status text with the countdown
            statusText.text = $"Starting in {timeLeft:F0} seconds...";
            timeLeft -= Time.deltaTime;

            yield return null;  // Wait for the next frame
        }

        // Start the game after the countdown finishes
        StartGame();
    }

    private void StartGame()
    {
        // Stop the matchmaking coroutine if it's still running
        if (matchmakingCoroutine != null)
        {
            StopCoroutine(matchmakingCoroutine);
        }

        // Load the game scene
        PhotonNetwork.LoadLevel("GameScene");
    }

    public void CancelMatchmaking()
    {
        // Stop all matchmaking-related coroutines
        if (matchmakingCoroutine != null)
        {
            StopCoroutine(matchmakingCoroutine);
            matchmakingCoroutine = null;
        }

        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        // Disconnect from Photon
        PhotonNetwork.Disconnect();

        // Update the status text
        statusText.text = "Matchmaking canceled";

        // Disable the cancel button
        cancelButton.gameObject.SetActive(false);
    }
}
