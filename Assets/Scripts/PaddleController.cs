using UnityEngine;
using Photon.Pun;

public class PaddleController : MonoBehaviourPun
{
    public float speed = 10f; // Speed of the paddle
    public float boundaryX = 8.5f; // Horizontal boundary

    private Vector3 paddlePosition;

    void Update()
    {
        // Check if this is the local player's paddle
        if (photonView.IsMine)
        {
            // Get player input for horizontal movement (left/right)
            float horizontalInput = Input.GetAxis("Horizontal");

            // Calculate new paddle position
            paddlePosition = transform.position;
            paddlePosition.x += horizontalInput * speed * Time.deltaTime;

            // Restrict paddle within screen boundaries
            paddlePosition.x = Mathf.Clamp(paddlePosition.x, -boundaryX, boundaryX);

            // Apply new position to the paddle
            transform.position = paddlePosition;
        }
    }
}
