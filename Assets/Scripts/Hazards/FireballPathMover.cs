using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class FireballPathMover : MonoBehaviour
{
    public enum LoopMode { Loop, PingPong }

    [Header("Path")]
    public List<Transform> points = new List<Transform>(); // set in Inspector
    public LoopMode loopMode = LoopMode.PingPong;
    public float speed = 2f;                 // world units per second
    public float waitAtPoint = 0f;           // seconds to pause at waypoints

    [Header("Gizmos")]
    public Color pathColor = new Color(1f, 0.6f, 0f, 0.8f);

    Rigidbody2D rb;
    int targetIndex = 0;
    int dir = 1; // +1 forward, -1 backward for PingPong
    float waitTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Start()
    {
        // Place fireball at the first point if available
        if (points.Count > 0 && points[0])
        {
            rb.position = points[0].position;
            targetIndex = (points.Count > 1) ? 1 : 0;
        }
    }

    void FixedUpdate()
    {
        if (points.Count < 2) return;
        if (!points[targetIndex]) return;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector2 current = rb.position;
        Vector2 target = points[targetIndex].position;
        Vector2 toTarget = target - current;
        float dist = toTarget.magnitude;

        float step = speed * Time.fixedDeltaTime;

        if (dist <= step)
        {
            // Snap to waypoint
            rb.MovePosition(target);

            if (waitAtPoint > 0f)
                waitTimer = waitAtPoint;

            // Advance target index
            if (loopMode == LoopMode.Loop)
            {
                targetIndex = (targetIndex + 1) % points.Count;
            }
            else // PingPong
            {
                if (targetIndex == points.Count - 1) dir = -1;
                else if (targetIndex == 0) dir = +1;

                targetIndex += dir;
            }
        }
        else
        {
            Vector2 next = current + toTarget.normalized * step;
            rb.MovePosition(next);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (points == null || points.Count == 0) return;
        Gizmos.color = pathColor;
        for (int i = 0; i < points.Count; i++)
        {
            var p = points[i];
            if (!p) continue;
            Gizmos.DrawSphere(p.position, 0.07f);
            if (i + 1 < points.Count && points[i + 1])
                Gizmos.DrawLine(p.position, points[i + 1].position);
        }
        // Loop preview
        if (loopMode == LoopMode.Loop && points.Count > 1 && points[0] && points[^1])
            Gizmos.DrawLine(points[^1].position, points[0].position);
    }
}