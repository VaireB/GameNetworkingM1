using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Tooltip("The prefab to use for representing the player")]
    [SerializeField]
    private GameObject[] playerPrefabs;  // Array of player prefabs

    [Tooltip("The spawn points for players")]
    [SerializeField]
    private Transform[] spawnPoints;     // Array of spawn points

    private void Start()
    {
        // Check if we are connected to Photon and in a room
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            // Try to instantiate the player if it's not already done
            if (PhotonNetwork.LocalPlayer.TagObject == null)
            {
                // Instantiate player
                InstantiatePlayer();
            }
        }
    }

    public override void OnJoinedRoom()
    {
        // Called when the local player joins a room
        if (PhotonNetwork.LocalPlayer.TagObject == null)
        {
            InstantiatePlayer();
        }
    }

    private void InstantiatePlayer()
    {
        // Get the local player's actor number
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        // Get the prefab and spawn point based on the actor number
        string prefabName = GetPrefabNameForActorNumber(actorNumber);
        Transform spawnPoint = GetSpawnPointForActorNumber(actorNumber);

        if (string.IsNullOrEmpty(prefabName) || spawnPoint == null)
        {
            Debug.LogError("Prefab name or spawn point is not assigned.");
            return;
        }

        // Instantiate the player prefab using PhotonNetwork.Instantiate
        GameObject playerObject = PhotonNetwork.Instantiate(prefabName, spawnPoint.position, spawnPoint.rotation, 0);

        if (playerObject == null)
        {
            Debug.LogError("Failed to instantiate player prefab.");
            return;
        }

        // Store the player object in the TagObject to avoid re-instantiating
        PhotonNetwork.LocalPlayer.TagObject = playerObject;

        Debug.Log($"Player prefab instantiated: {playerObject.name} at {spawnPoint.position}");
    }

    private string GetPrefabNameForActorNumber(int actorNumber)
    {
        // Map actor number to prefab name
        if (actorNumber == 1)
            return "Player1";
        else if (actorNumber == 2)
            return "Player2";
        else
            return null;
    }

    private Transform GetSpawnPointForActorNumber(int actorNumber)
    {
        // Map actor number to spawn points
        if (actorNumber == 1 && spawnPoints.Length > 0)
            return spawnPoints[0];
        else if (actorNumber == 2 && spawnPoints.Length > 1)
            return spawnPoints[1];
        else
            return null;
    }
}
