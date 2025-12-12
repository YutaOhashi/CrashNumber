using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 赤ブロックの挙動
/// - 赤ブロック同士は同じ形状なら合体
/// - 青ブロックと衝突すると値を計算
/// - 緑ブロックには何もしない
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
public class RedBlock : MonoBehaviour
{
    public int value = 1;                       // 赤ブロックの値
    public Sprite[] shapes;                     // 形状スプライト
    public TextMeshPro text;                    // 値表示

    [Header("Prefab References")]
    public GameObject greenPrefab;              // 緑ブロックに変換するPrefab

    private SpriteRenderer sr;
    public Rigidbody2D rb;
    private PolygonCollider2D poly;
    private bool isMerging = false;             // 合体中フラグ
    [HideInInspector] public int shapeIndex = 0;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        poly = GetComponent<PolygonCollider2D>();

        rb.freezeRotation = false;
        rb.gravityScale = 1f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (poly.sharedMaterial == null)
            poly.sharedMaterial = new PhysicsMaterial2D("BlockMaterial");
    }

    void Start()
    {
        ApplyAllUpdates();
    }

    /// <summary>
    /// 形状・物理・テキストをまとめて更新
    /// </summary>
    public void ApplyAllUpdates()
    {
        UpdateShape();
        RefreshCollider();
        UpdatePhysics();
        UpdateVisualScale();
        UpdateText();
    }

    /// <summary>
    /// 赤ブロック同士の合体
    /// </summary>
    public void MergeWith(RedBlock other)
    {
        if (isMerging || other.isMerging) return;

        RedBlock smaller, bigger;
        if (this.value < other.value) { smaller = this; bigger = other; }
        else if (this.value > other.value) { smaller = other; bigger = this; }
        else { smaller = (this.GetInstanceID() < other.GetInstanceID()) ? this : other; bigger = (smaller == this) ? other : this; }

        smaller.isMerging = true;
        bigger.value += smaller.value;
        bigger.ApplyAllUpdates();
        bigger.StartCoroutine(bigger.StabilizeAfterMerge());

        Destroy(smaller.gameObject);
    }

    IEnumerator StabilizeAfterMerge()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        yield return new WaitForSeconds(0.05f);
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    /// <summary>
    /// 青ブロックと衝突した場合の処理
    /// </summary>
    public void HandleBlueCollision(BlueBlock blue)
    {
        if (blue == null || blue.hasCollided) return;

        blue.hasCollided = true;

        int newValue = value - blue.value;
        Destroy(blue.gameObject);

        if (newValue < 0)
        {
            // 赤より青が大きい場合は緑ブロックに変換
            ConvertToGreen(Mathf.Abs(newValue));
        }
        else if (newValue == 0)
        {
            // 値が0なら消滅
            Destroy(gameObject);
        }
        else
        {
            value = newValue;
            ApplyAllUpdates();
        }
    }

    void ConvertToGreen(int greenValue)
    {
        if (greenPrefab != null)
        {
            GameObject green = Instantiate(greenPrefab, transform.position, Quaternion.identity);
            GreenBlock greenBlock = green.GetComponent<GreenBlock>();
            if (greenBlock != null)
            {
                greenBlock.Initialize(greenValue);
            }
        }
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isMerging) return;

        // 赤ブロック同士の合体
        RedBlock otherRed = collision.gameObject.GetComponent<RedBlock>();
        if (otherRed != null && shapeIndex == otherRed.shapeIndex)
        {
            MergeWith(otherRed);
        }

        // 青ブロックとの衝突
        BlueBlock blue = collision.gameObject.GetComponent<BlueBlock>();
        if (blue != null)
        {
            HandleBlueCollision(blue);
        }

        // 緑ブロックには何もしない
    }

    #region 更新処理
    public void UpdateShape()
    {
        if (value <= 4) shapeIndex = 0;
        else if (value <= 9) shapeIndex = 1;
        else if (value <= 19) shapeIndex = 2;
        else if (value <= 29) shapeIndex = 3;
        else if (value <= 49) shapeIndex = 4;
        else shapeIndex = 5;

        if (sr != null && shapes != null && shapes.Length > shapeIndex)
            sr.sprite = shapes[shapeIndex];
    }

    public void RefreshCollider()
    {
        if (poly == null || sr == null || sr.sprite == null) return;

        poly.pathCount = sr.sprite.GetPhysicsShapeCount();
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < poly.pathCount; i++)
        {
            points.Clear();
            sr.sprite.GetPhysicsShape(i, points);
            poly.SetPath(i, points.ToArray());
        }
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

    public void UpdateVisualScale()
    {
        float scale = 1f;
        switch (shapeIndex)
        {
            case 0: scale = 0.2f; break;
            case 1: scale = 0.4f; break;
            case 2: scale = 0.6f; break;
            case 3: scale = 0.8f; break;
            case 4: scale = 0.5f; break;
            case 5: scale = 1.2f; break;
        }
        transform.localScale = Vector3.one * scale;
    }

    public void UpdateText()
    {
        if (text != null)
            text.text = value.ToString();
    }
    #endregion
}
