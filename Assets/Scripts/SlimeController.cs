using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class SlimeController : MonoBehaviour
{
    [Header("Launch")]
    public float maxPullDistance = 3f;
    public float launchPower = 8f;
    public LineRenderer aimLine;
    public LayerMask groundMask;

    [Header("State (debug)")]
    public bool isStuck = true;

    [Header("Physics")]
    public float flightGravityScale = 2f;
    public CollisionDetectionMode2D flightCollisionDetection = CollisionDetectionMode2D.Continuous; // helps fast shots

    [Header("Aim Arrow")]
    public Transform aimArrowRoot;
    public SpriteRenderer arrowBody;
    public SpriteRenderer arrowHead;
    public float arrowMaxLength = 2.5f;       // world units cap
    public float arrowThickness = 2f / 16f;   // 2 px at PPU=16
    public float arrowHeadWidth = 8f / 16f;
    public bool pixelSnapArrow = true;
    public float headOverlapPixels = 1f;

    [Header("Arrow Origin")]
    public float arrowStartMarginPixels = 2f;   // how far outside the slime border
    public bool pixelSnapArrowOrigin = true;    // snap origin to pixel grid too

    [Header("Pixels")]
    public float pixelsPerUnit = 16f;

    [Header("Input")]
    public float pullDeadZone = 0.05f;

    Rigidbody2D rb;
    Collider2D col;
    Camera cam;
    Vector2 pullStart;
    float defaultGravity;

    float WorldPixel => 1f / pixelsPerUnit;
    float PPU => pixelsPerUnit;

    float BodyWorldWidth =>
        (arrowBody && arrowBody.sprite) ? arrowBody.sprite.rect.width / PPU : 0f;

    float HeadWorldWidth =>
        (arrowHead && arrowHead.sprite) ? arrowHead.sprite.rect.width / PPU : 0f;

    float BodyPivotX =>
        (arrowBody && arrowBody.sprite) ? arrowBody.sprite.pivot.x / PPU : 0f;

    float HeadPivotX =>
        (arrowHead && arrowHead.sprite) ? arrowHead.sprite.pivot.x / PPU : 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        cam = Camera.main;                    // cache
        defaultGravity = rb.gravityScale;
        if (aimArrowRoot) aimArrowRoot.gameObject.SetActive(false);
        Stick();                               // start stuck so you can sling immediately
    }

    void Update()
    {
        if (!isStuck) return;

        if (Input.GetMouseButtonDown(0))
        {
            pullStart = GetMouseWorld();
            ShowLine(false);
            if (aimArrowRoot) aimArrowRoot.gameObject.SetActive(true);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 current = GetMouseWorld();
            Vector2 pull = Vector2.ClampMagnitude(pullStart - current, maxPullDistance);

            if (pull.magnitude < pullDeadZone) // small dead-zone to stop jitter
            {
                if (aimArrowRoot) aimArrowRoot.gameObject.SetActive(false);
                return;
            }

            // --- Arrow UI while dragging ---
            // (REPLACE your existing arrow code in the dragging branch with this)
            if (aimArrowRoot)
            {
                aimArrowRoot.gameObject.SetActive(true);

                Vector2 dir = pull.normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                // Start the arrow at the slime’s edge, a few pixels outside
                aimArrowRoot.position = GetArrowOrigin(dir);
                aimArrowRoot.rotation = Quaternion.Euler(0, 0, angle);

                // === Visual length should match pull strength up to maxPullDistance ===
                float pullLen = pull.magnitude;
                float totalLen = Mathf.Min(pullLen, maxPullDistance);

                float wp = WorldPixel;                 // 1 / PPU
                if (pixelSnapArrow) totalLen = Mathf.Round(totalLen / wp) * wp;

                float headW = HeadWorldWidth;
                float overlap = headOverlapPixels * wp;
                float bodyLen = Mathf.Max(0f, totalLen - headW);
                if (pixelSnapArrow) bodyLen = Mathf.Round(bodyLen / wp) * wp;

                if (arrowBody)
                {
                    arrowBody.drawMode = SpriteDrawMode.Tiled; // important
                    arrowBody.size = new Vector2(bodyLen, arrowThickness);
                    arrowBody.transform.localPosition = new Vector3(BodyPivotX, 0f, 0f);
                }

                if (arrowHead)
                {
                    float headX = Mathf.Max(0f, bodyLen - overlap) + HeadPivotX;
                    if (pixelSnapArrow) headX = Mathf.Round(headX / wp) * wp;
                    arrowHead.transform.localPosition = new Vector3(headX, 0f, 0f);
                }
            }

        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector2 current = GetMouseWorld();
            Vector2 pull = Vector2.ClampMagnitude(pullStart - current, maxPullDistance);

            if (pull.magnitude >= pullDeadZone)
            {
                Vector2 launchVel = pull * launchPower;
                Unstick();
                rb.linearVelocity = launchVel;                         // correct property

                if (aimArrowRoot) aimArrowRoot.gameObject.SetActive(false);
                ShowLine(false);
            }
            else
            {
                if (aimArrowRoot) aimArrowRoot.gameObject.SetActive(false);
            }
        }
        else
        {
            if (aimArrowRoot) aimArrowRoot.gameObject.SetActive(false);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        int otherLayer = col.collider.gameObject.layer;
        if ((groundMask.value & (1 << otherLayer)) != 0)
        {
            if (col.contactCount > 0)
            {
                var c = col.GetContact(0);

                Stick();

                // Push OUT of the surface by the penetration amount (+ a tiny skin)
                // c.separation is negative when overlapped.
                float skin = 0.01f; // ~ 1/100 world unit
                float pushOut = Mathf.Max(0f, -c.separation) + skin;

                rb.position = rb.position + c.normal * pushOut;
            }
            else
            {
                Stick();
            }
        }
    }

    void Stick()
    {
        isStuck = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;                                  // truly stuck
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Unstick()
    {
        isStuck = false;
        rb.bodyType = RigidbodyType2D.Dynamic;                               // enable physics for flight
        rb.gravityScale = flightGravityScale;
        rb.collisionDetectionMode = flightCollisionDetection;    // better for fast shots
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    Vector2 GetMouseWorld()
    {
        var m = cam ? cam.ScreenToWorldPoint(Input.mousePosition) : Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(m.x, m.y);
    }

    void ShowLine(bool on)
    {
        if (aimLine) aimLine.enabled = on;
    }

    Vector3 GetArrowOrigin(Vector2 dir)
    {
        // Fallback if no collider
        if (!col) return transform.position;

        Vector2 center = rb.position;
        // Far point along aim direction so ClosestPoint returns the facing surface point
        Vector2 farPoint = center + dir * 100f;
        Vector2 surfacePoint = col.ClosestPoint(farPoint);

        float wp = 1f / pixelsPerUnit;
        float margin = arrowStartMarginPixels * wp;

        Vector2 origin = surfacePoint + dir * margin;

        if (pixelSnapArrowOrigin)
        {
            origin.x = Mathf.Round(origin.x / wp) * wp;
            origin.y = Mathf.Round(origin.y / wp) * wp;
        }

        return origin;
    }
}
