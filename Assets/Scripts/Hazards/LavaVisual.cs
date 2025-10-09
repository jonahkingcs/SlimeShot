using UnityEngine;

[ExecuteAlways]
public class LavaVisual : MonoBehaviour
{
    [Header("Assign renderers")]
    public SpriteRenderer surface;   // LavaSurface
    public SpriteRenderer body;      // LavaBody

    [Header("Tiling (world units)")]
    [Tooltip("48px @ 16 PPU = 3 world units")]
    public float tileSize = 3f;          // one tile = 3 world units
    [Tooltip("Approx world width to cover; will snap to tile width")]
    public float levelWidthWorld = 200f; // adjust to your level
    [Tooltip("How many tile rows of body to render below the surface")]
    public int bodyRows = 40;            // 40 * 3 = 120 units tall

    BoxCollider2D col;

    void OnEnable()
    {
        col = GetComponentInParent<BoxCollider2D>();
        Apply();
    }

    void LateUpdate() => Apply();

    void Apply()
    {
        if (!surface || !body || col == null) return;

        // Snap width to whole tiles
        int tilesWide = Mathf.Max(1, Mathf.CeilToInt(levelWidthWorld / tileSize));
        float width = tilesWide * tileSize;

        // Heights
        float surfaceH = tileSize;            // exactly one 48px row
        float bodyH = Mathf.Max(1, bodyRows) * tileSize;

        // Set sizes
        surface.drawMode = SpriteDrawMode.Tiled;
        body.drawMode = SpriteDrawMode.Tiled;
        surface.size = new Vector2(width, surfaceH);
        body.size = new Vector2(width, bodyH);

        // Align to collider top
        float topY = col.bounds.max.y;
        float centerX = col.bounds.center.x;

        // Surface centered on its strip at the top
        surface.transform.position = new Vector3(centerX, topY - surfaceH * 0.5f, surface.transform.position.z);

        // Body sits immediately below the surface
        float bodyTopY = topY - surfaceH;
        body.transform.position = new Vector3(centerX, bodyTopY - bodyH * 0.5f, body.transform.position.z);
    }
}
