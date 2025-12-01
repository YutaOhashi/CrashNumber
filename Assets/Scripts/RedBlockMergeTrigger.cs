using UnityEngine;

public class RedBlockMergeTrigger : MonoBehaviour
{
    private RedBlock parentBlock;
    private CircleCollider2D triggerCol;

    void Start()
    {
        parentBlock = GetComponentInParent<RedBlock>();

        // Trigger Collider を取得／追加
        triggerCol = GetComponent<CircleCollider2D>();
        if (triggerCol == null) triggerCol = gameObject.AddComponent<CircleCollider2D>();

        triggerCol.isTrigger = true;
        triggerCol.radius = 0.5f;  // 固定サイズ
        triggerCol.offset = Vector2.zero;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        RedBlock otherBlock = other.GetComponentInParent<RedBlock>();
        if (otherBlock != null && parentBlock != null)
        {
            if (parentBlock.shapeIndex == otherBlock.shapeIndex)
                parentBlock.MergeWith(otherBlock);
        }
    }
}