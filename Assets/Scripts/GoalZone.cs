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
        if (triggered) return;

        // Prefer a component check so you don't depend on tags
        var rb = other.attachedRigidbody;
        if (rb && rb.GetComponent<SlimeController>())
        {
            triggered = true;
            onGoalReached?.Invoke();
        }
    }
}
