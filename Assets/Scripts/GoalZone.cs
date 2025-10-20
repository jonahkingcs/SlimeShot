using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class GoalZone : MonoBehaviour
{
    [Tooltip("Checks for SlimeController on the entering Rigidbody2D.")]
    public UnityEvent onGoalReached;

    bool triggered;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true; // ensure trigger
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var rb = other.attachedRigidbody;
        var slime = rb ? rb.GetComponent<SlimeController>() : null;
        if (!slime) return;

        // stop the HUD timer if present
        var timer = FindObjectOfType<TimerUI>(true);
        if (timer) timer.StopTimer();

        onGoalReached?.Invoke(); // shows Level Complete UI
    }
}
