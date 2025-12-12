using UnityEngine;

/// <summary>
/// 青ブロック
/// - 赤ブロックにのみ衝突処理を行う
/// - 緑や青ブロックには衝突しても無効
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
public class BlueBlock : MonoBehaviour
{
    public int value = 1;
    public bool hasCollided = false;

    private Rigidbody2D rb;
    private PolygonCollider2D poly;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        poly = GetComponent<PolygonCollider2D>();

        rb.freezeRotation = false;
        rb.gravityScale = 1f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (poly.sharedMaterial == null)
            poly.sharedMaterial = new PhysicsMaterial2D("BlockMaterial");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 赤ブロックにのみ反応
        RedBlock red = collision.gameObject.GetComponent<RedBlock>();
        if (red != null)
        {
            red.HandleBlueCollision(this);
        }
        // 緑や青ブロックには無効
    }

    public void UpdatePhysics()
    {
        var mat = poly.sharedMaterial;
        if (value <= 4) { rb.mass = 0.3f; mat.bounciness = 0.6f; mat.friction = 0.1f; }
        else if (value <= 9) { rb.mass = 0.6f; mat.bounciness = 0.4f; mat.friction = 0.2f; }
        else if (value <= 19) { rb.mass = 0.9f; mat.bounciness = 0.3f; mat.friction = 0.3f; }
        else if (value <= 29) { rb.mass = 1.5f; mat.bounciness = 0.2f; mat.friction = 0.4f; }
        else if (value <= 49) { rb.mass = 2.5f; mat.bounciness = 0.1f; mat.friction = 0.5f; }
        else { rb.mass = 4f; mat.bounciness = 0.05f; mat.friction = 0.6f; }
    }
}
