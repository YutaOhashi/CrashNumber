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
        if (otherBlock != null && parentBlock != null)
        {
            // 同じ形状なら合体
            if (parentBlock.shapeIndex == otherBlock.shapeIndex)
                parentBlock.MergeWith(otherBlock);
        }

        // 青・緑ブロックには無効
    }
}
