using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the ball
        if (other.CompareTag("Ball"))
        {
            // Notify GoalManager about the goal
            GoalManager.Instance.CheckGoal(gameObject);
        }
    }
}
