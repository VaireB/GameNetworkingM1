using UnityEngine;
using Photon.Pun;
using System.Collections;

public class BallController : MonoBehaviourPun, IPunObservable
{
    public static BallController Instance;

    public float initialSpeed = 10f; // Initial speed of the ball
    public float speedIncreaseFactor = 1.1f; // Factor by which the speed increases on each bounce
    public float maxSpeed = 20f; // Maximum speed for the ball
    public float resetTime = 10f; // Time after which the ball will be reset if not hit
    public Transform spawnPosition; // Transform for the spawn position

    private Rigidbody rb;
    private bool isBallInPlay = false;
    private float timeSinceLastHit = 0f; // Timer to track time since last hit

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
        rb = GetComponent<Rigidbody>();
        if (PhotonNetwork.IsMasterClient)
        {
            // Start the ball after a delay
            StartCoroutine(LaunchBall());
        }
    }

    IEnumerator LaunchBall()
    {
        yield return new WaitForSeconds(2f); // Delay before launching the ball

        // Launch the ball in a random direction
        Vector3 randomDirection = GetRandomDirection();
        rb.velocity = randomDirection * initialSpeed;
        isBallInPlay = true;

        // Sync the ball's initial launch with other clients
        photonView.RPC("SyncBallMovement", RpcTarget.Others, rb.position, rb.velocity);
    }

    void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Update the timer
        if (isBallInPlay)
        {
            timeSinceLastHit += Time.deltaTime;

            // Check if the ball needs to be reset
            if (timeSinceLastHit >= resetTime)
            {
                ResetBall();
            }
        }

        // Ensure the ball keeps moving in the correct direction
        if (isBallInPlay && rb.velocity.magnitude < initialSpeed)
        {
            rb.velocity = rb.velocity.normalized * initialSpeed;
        }

        // Cap the ball speed to avoid it going too fast
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        // Sync the ball's position and velocity with other clients
        photonView.RPC("SyncBallMovement", RpcTarget.Others, transform.position, rb.velocity);
    }

    private Vector3 GetRandomDirection()
    {
        // Generate a random direction for the ball launch
        float x = Random.Range(-3f, 3f);
        float z = Random.Range(-3f, 3f);

        Vector3 direction = new Vector3(x, 0f, z).normalized;
        return direction;
    }

    public void ResetBall()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        rb.velocity = Vector3.zero;
        transform.position = spawnPosition ? spawnPosition.position : Vector3.zero; // Use spawnPosition if assigned
        isBallInPlay = false;
        timeSinceLastHit = 0f; // Reset the timer

        // Relaunch the ball after a short delay
        StartCoroutine(LaunchBall());
    }

    private void OnCollisionEnter(Collision collision)
    {
        // When the ball collides with paddles, increase the speed and reset the timer
        if (collision.gameObject.CompareTag("Paddle"))
        {
            rb.velocity *= speedIncreaseFactor;
            rb.velocity = rb.velocity.normalized * Mathf.Min(rb.velocity.magnitude, maxSpeed);
            timeSinceLastHit = 0f; // Reset the timer on a hit

            // Sync the updated velocity with other clients
            photonView.RPC("SyncBallMovement", RpcTarget.Others, transform.position, rb.velocity);
        }
    }

    [PunRPC]
    private void SyncBallMovement(Vector3 position, Vector3 velocity)
    {
        if (PhotonNetwork.IsMasterClient) return;

        // Update the position and velocity of the ball on non-master clients
        rb.position = position;
        rb.velocity = velocity;
    }

    // Sync the ball's position and velocity with other clients via Photon
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send ball's position and velocity to other clients
            stream.SendNext(transform.position);
            stream.SendNext(rb.velocity);
        }
        else
        {
            // Receive ball's position and velocity from the master client
            transform.position = (Vector3)stream.ReceiveNext();
            rb.velocity = (Vector3)stream.ReceiveNext();
        }
    }
}
