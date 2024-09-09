using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerCustomizationManager : MonoBehaviour
{
    // Avatar selection related
    public GameObject[] avatarOptions;            // Array of avatar buttons (with images)
    public TextMeshProUGUI selectedAvatarText;    // UI text to display the selected avatar
    private int selectedAvatarIndex = 0;          // Currently selected avatar index

    // Player name related
    public TMP_InputField playerNameInput;        // Input field for the player's name
    public Button saveNameButton;                 // Button to save the player's name
    private string playerName = "";               // Store the player's name

    void Start()
    {
        // Setup avatar selection listeners for each button
        for (int i = 0; i < avatarOptions.Length; i++)
        {
            int index = i; // Local variable to capture button index
            avatarOptions[i].GetComponent<Button>().onClick.AddListener(() => UpdateSelectedAvatar(index));
        }

        // Initialize avatar selection (defaults to first avatar)
        UpdateSelectedAvatar(selectedAvatarIndex);

        // Setup save name button listener
        saveNameButton.onClick.AddListener(SavePlayerName);

        // Load previously saved player name if it exists
        LoadPlayerName();
    }

    // Update the UI and save the selected avatar
    private void UpdateSelectedAvatar(int index)
    {
        selectedAvatarIndex = index;
        selectedAvatarText.text = $"Selected Avatar: {index + 1}"; // Update UI

        // Save the selected avatar index to PlayerPrefs
        PlayerPrefs.SetInt("SelectedAvatar", selectedAvatarIndex);
        Debug.Log($"Avatar Selected: {selectedAvatarIndex}");
    }

    // Save the player's name to PlayerPrefs and Photon Nickname
    public void SavePlayerName()
    {
        playerName = playerNameInput.text;

        // Ensure the player name is not empty
        if (!string.IsNullOrEmpty(playerName))
        {
            // Save player name to PlayerPrefs
            PlayerPrefs.SetString("PlayerName", playerName);

            // Set Photon player nickname for multiplayer games
            PhotonNetwork.NickName = playerName;

            Debug.Log($"Player name saved: {playerName}");
        }
        else
        {
            Debug.LogWarning("Player name cannot be empty.");
        }
    }

    // Load the saved player name and set it in the input field
    private void LoadPlayerName()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            playerName = PlayerPrefs.GetString("PlayerName");
            playerNameInput.text = playerName;  // Pre-fill the input field
        }
    }
}
