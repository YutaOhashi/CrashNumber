using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GreenBlock : MonoBehaviour
{
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
    }

    // 合体・減少など一切無視
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 何も起こさない（ただの障害物）
    }
}
