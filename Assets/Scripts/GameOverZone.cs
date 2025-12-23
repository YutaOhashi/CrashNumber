using UnityEngine;

public class GameOverZone : MonoBehaviour
{
    public GameObject gameOverText; // 表示する文字
    public float timeLimit = 2.0f;  // 我慢する時間（秒）

    private float timer = 0f;
    private BoxCollider2D myCollider; // 自分のセンサー範囲

    void Start()
    {
        // 自分の当たり判定を取得
        myCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        // レーダー探索開始
        // 1. 自分の場所とサイズを計算（少しだけ小さくして誤作動を防ぐ）
        Vector2 point = (Vector2)transform.position + (myCollider.offset * transform.localScale);
        Vector2 size = myCollider.size * transform.localScale * 0.9f; 
        float angle = transform.eulerAngles.z;

        // 2. その範囲にいる全員を見つけ出す（OverlapBoxAll）
        Collider2D[] hits = Physics2D.OverlapBoxAll(point, size, angle);

        bool isBlockInZone = false;

        // 3. 見つけたものがブロックか確認
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue; // 自分は無視

            // 親オブジェクトも含めて「ブロック」か調べる
            if (IsBlock(hit))
            {
                isBlockInZone = true;
                break; // ひとつでもあればOK
            }
        }

        // 4. タイマー処理
        if (isBlockInZone)
        {
            timer += Time.deltaTime; // 時間を進める
        }
        else
        {
            timer = 0f; // ブロックがいなくなったらリセット
        }

        // 5. 制限時間を超えたらゲームオーバー
        if (timer > timeLimit)
        {
            GameOver();
        }
    }

    // これは「それがブロックかどうか」を見分ける便利な関数
    bool IsBlock(Collider2D col)
    {
        // 親をたどってRigidbodyを探す
        Rigidbody2D rb = col.GetComponentInParent<Rigidbody2D>();
        
        // プレイヤーが操作中（Kinematic）なら無視する
        if (rb != null && rb.bodyType == RigidbodyType2D.Kinematic) return false;

        // 赤・緑・青ブロックのスクリプトがついているか（親も含めて）確認
        if (col.GetComponentInParent<RedBlock>() != null || 
            col.GetComponentInParent<GreenBlock>() != null ||
            col.GetComponentInParent<BlueBlock>() != null)
        {
            return true;
        }
        return false;
    }

    void GameOver()
    {
        Debug.Log("Game Over!");
        
        // 文字を表示する
        if (gameOverText != null)
        {
            gameOverText.SetActive(true);
        }

        // 時間を止める
        Time.timeScale = 0f;
    }

    // デバッグ用：Unityの編集画面で赤い四角を表示する機能
    void OnDrawGizmos()
    {
        if (myCollider == null) myCollider = GetComponent<BoxCollider2D>();
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Vector3 size = myCollider.size;
        size.x *= transform.localScale.x * 0.9f;
        size.y *= transform.localScale.y * 0.9f;
        Gizmos.DrawCube(transform.position + (Vector3)myCollider.offset, size);
    }
}