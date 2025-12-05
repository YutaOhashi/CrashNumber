using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BlueBlock : MonoBehaviour
{
    public int decreaseAmount = 3;  // 赤ブロックを何減少させるか
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        RedBlock red = collision.gameObject.GetComponent<RedBlock>();
        if (red == null) return;

        // ▼ 数値減少
        red.value -= decreaseAmount;
        if (red.value < 1) red.value = 1;

        // ▼ 更新
        red.ApplyAllUpdates();

        // ▼ もし 1 を下回った場合 → 緑ブロックに変化
        if (red.value <= 1)
        {
            ConvertToGreen(red);
        }

        // 青ブロック自身は役目を終えたら消す
        Destroy(gameObject);
    }

    void ConvertToGreen(RedBlock red)
    {
        // GreenBlock プレハブに置き換える
        GameObject greenPrefab = Resources.Load<GameObject>("GreenBlock");
        if (greenPrefab != null)
        {
            Instantiate(greenPrefab, red.transform.position, Quaternion.identity);
            Destroy(red.gameObject);
        }
    }
}
