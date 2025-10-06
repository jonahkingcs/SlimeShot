using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class SlimeController : MonoBehaviour
{
    [Header("Launch")]
    public float maxPullDistance = 3f;  // how far you can pull back
    public float launchPower = 8f;     // velocity multiplier
    public LineRenderer aimLine;        // show an aiming line
    public LayerMask groundMask;        // assign "Ground" layer in Inspector

    [Header("State (debug)")]
    public bool isStuck = true;

    [Header("Physics")]
    public float flightGravityScale = 2f;

    Rigidbody2D rb;
    Vector2 pullStart;
    float defaultGravity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
        Stick();    // start stuck so you can sling immediately
    }

    void Update()
    {
        if (!isStuck) return;

        if (Input.GetMouseButtonDown(0))
        {
            pullStart = GetMouseWorld();
            ShowLine(true);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 current = GetMouseWorld();
            Vector2 pull = Vector2.ClampMagnitude(pullStart - current, maxPullDistance);

            if (aimLine)
            {
                aimLine.positionCount = 2;
                aimLine.SetPosition(0, transform.position);
                aimLine.SetPosition(1, (Vector2)transform.position + pull);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector2 current = GetMouseWorld();
            Vector2 pull = Vector2.ClampMagnitude(pullStart - current, maxPullDistance);

            Vector2 launchVel = pull * launchPower; // opposite of pull direction
            Unstick();
            rb.linearVelocity = launchVel;
            ShowLine(false);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Stick to anything on groundMask
        if (((1 << col.collider.gameObject.layer) & groundMask) != 0)
        {
            Stick();

            // Nudge slightly into the surface to feel glued (tiny value)
            rb.position = rb.position + col.contacts[0].normal * -0.02f;
        }
    }

    void Stick()
    {
        isStuck = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0f;   // no sliding while stuck
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Unstick()
    {
        isStuck = false;
        rb.gravityScale = flightGravityScale;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    Vector2 GetMouseWorld()
    {
        var m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(m.x, m.y);
    }

    void ShowLine(bool on)
    {
        if (aimLine) aimLine.enabled = on;
    }
}
