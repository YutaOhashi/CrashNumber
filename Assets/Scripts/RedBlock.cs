// ----------------------------
// RedBlock.cs
// ----------------------------
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
public class RedBlock : MonoBehaviour
{
    public int value = 1;
    public Sprite[] shapes;    // 0:〇, 1:△, 2:□, 3:☆, 4:♤, 5:♡
    public TextMeshPro text;

    private SpriteRenderer sr;
    public Rigidbody2D rb;
    private PolygonCollider2D poly;
    private bool isMerging = false;

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

    // ----------------------------------------
    public void ApplyAllUpdates()
    {
        UpdateShape();
        RefreshCollider();
        UpdatePhysics();
        UpdateVisualScale();
        UpdateText();
    }

    IEnumerator StabilizeAfterMerge()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        yield return new WaitForSeconds(0.05f);
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    void FixOverlap()
    {
        int wallLayer = LayerMask.GetMask("Wall");
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.1f, wallLayer);

        if (hit)
        {
            Vector2 dir = (Vector2)(transform.position - hit.transform.position);
            if (dir == Vector2.zero) dir = Vector2.right;

            transform.position += (Vector3)dir.normalized * 0.1f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isMerging) return;

        RedBlock other = collision.gameObject.GetComponent<RedBlock>();
        if (other == null || other.isMerging) return;

        if (this.shapeIndex == 5 && other.shapeIndex == 5)
        {
            this.isMerging = true;
            other.isMerging = true;
            Destroy(this.gameObject);
            Destroy(other.gameObject);
            return;
        }

        if (this.shapeIndex == other.shapeIndex)
            MergeWith(other);
    }

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
        bigger.FixOverlap();
        bigger.StartCoroutine(bigger.StabilizeAfterMerge());

        Destroy(smaller.gameObject);
    }

    public void UpdateShape()
    {
        if (value <= 4) shapeIndex = 0;       // 〇
        else if (value <= 9) shapeIndex = 1;  // □
        else if (value <= 19) shapeIndex = 2; // △
        else if (value <= 29) shapeIndex = 3; // ☆
        else if (value <= 49) shapeIndex = 4; // ♤
        else shapeIndex = 5;                  // ♡

        // Nullチェックで安全に
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
}
