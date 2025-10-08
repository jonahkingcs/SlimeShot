using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class LavaRiser : MonoBehaviour
{
    [Tooltip("World units per second the lava rises.")]
    public float riseSpeed = 0.5f;

    [Tooltip("Optional delay before lava starts rising.")]
    public float startDelay = 0f;

    [Tooltip("Optional extra units/second^2 to ramp difficulty.")]
    public float acceleration = 0f;

    Rigidbody2D _rb;
    float _timer;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        // Ensure we’re a kinematic trigger
        _rb.bodyType = RigidbodyType2D.Kinematic;
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void FixedUpdate()
    {
        if (_timer < startDelay)
        {
            _timer += Time.fixedDeltaTime;
            return;
        }

        // accelerate if set
        if (acceleration != 0f)
            riseSpeed += acceleration * Time.fixedDeltaTime;

        // Move kinematic body via MovePosition for correct trigger updates
        Vector2 next = _rb.position + Vector2.up * (riseSpeed * Time.fixedDeltaTime);
        _rb.MovePosition(next);
    }
}