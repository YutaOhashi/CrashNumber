using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 緑ブロック
/// - どのブロックとも合体しない
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
public class GreenBlock : MonoBehaviour
{
    public int value = 1;
    public Sprite[] shapes;
    public TextMeshPro text;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private PolygonCollider2D poly;
    private int shapeIndex = 0;

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

    void Start() => ApplyAllUpdates();

    public void Initialize(int newValue)
    {
        value = Mathf.Abs(newValue);
        ApplyAllUpdates();
    }

    public void ApplyAllUpdates()
    {
        UpdateShape();
        RefreshCollider();
        UpdatePhysics();
        UpdateVisualScale();
        UpdateText();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 緑ブロックはどのブロックとも合体しない
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
        if (text != null) text.text = value.ToString();
    }
    #endregion
}
