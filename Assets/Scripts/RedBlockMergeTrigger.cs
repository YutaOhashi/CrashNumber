using UnityEngine;

/// <summary>
/// 赤ブロック同士の合体を検知するトリガー
/// 緑・青ブロックには無効
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class RedBlockMergeTrigger : MonoBehaviour
{
    private RedBlock parentBlock;
    private CircleCollider2D triggerCol;

    void Start()
    {
        parentBlock = GetComponentInParent<RedBlock>();

        // トリガー追加
        triggerCol = GetComponent<CircleCollider2D>();
        if (triggerCol == null)
            triggerCol = gameObject.AddComponent<CircleCollider2D>();

        triggerCol.isTrigger = true;
        triggerCol.radius = 0.5f;
        triggerCol.offset = Vector2.zero;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        RedBlock otherBlock = other.GetComponentInParent<RedBlock>();
        // 1. 相手そのもの、または親に RedBlock がついているか探す
        if (otherBlock == null) otherBlock = other.GetComponentInParent<RedBlock>();

        // 2. 相手が赤ブロックではない（緑や青）なら、ここで完全に無視する
        if (otherBlock == null) return;

        // 3. 相手が赤ブロックだった場合のみ、形をチェックして合体
        if (parentBlock != null && parentBlock.shapeIndex == otherBlock.shapeIndex)
        {
            parentBlock.MergeWith(otherBlock);
        }
    }
}
