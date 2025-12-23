using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class BlockSpawner : MonoBehaviour
{
    [Header("Block Prefabs")]
    public GameObject redPrefab;
    public GameObject bluePrefab; // ※今回はPrefabを使わずAddComponentしていますが枠は残しておきます
    public GameObject greenPrefab;

    [Header("Spawn Settings")]
    public float moveSpeed = 5f;
    public float minX = -3.6f;
    public float maxX = 3.6f;
    public float spawnY = 4.5f;

    [Header("Blue Block Limit")]
    public int maxBlueBlocks = 10; // 最大使用回数
    private int currentBlueCount = 0; // 使用した青ブロックの数
    public TextMeshProUGUI blueCountText; // UIで残り回数を表示

    private GameObject currentBlock;
    private bool isWaitingNext = false;

    void Update()
    {
        if (isWaitingNext) return;

        // 現在操作中のブロックがある場合
        if (currentBlock != null)
        {
            Rigidbody2D body = currentBlock.GetComponent<Rigidbody2D>();
            
            // 操作中（Kinematic）の場合のみ動かせる
            if (body != null && body.bodyType == RigidbodyType2D.Kinematic)
            {
                Vector3 pos = currentBlock.transform.position;

                // 左右移動 (A/Dキー)
                if (Keyboard.current.aKey.isPressed) pos.x -= moveSpeed * Time.deltaTime;
                if (Keyboard.current.dKey.isPressed) pos.x += moveSpeed * Time.deltaTime;
                pos.x = Mathf.Clamp(pos.x, minX, maxX);
                currentBlock.transform.position = pos;

                // Fで落下
                if (Keyboard.current.fKey.wasPressedThisFrame)
                {
                    body.bodyType = RigidbodyType2D.Dynamic;
                    currentBlock = null;
                    StartCoroutine(WaitAndSpawnNext());
                }

                // Qで赤↔青変換
                if (Keyboard.current.qKey.wasPressedThisFrame)
                {
                    ToggleRedBlue();
                }
            }
        }
        else
        {
            SpawnNewBlock();
        }

        // UI更新
        if (blueCountText != null)
            blueCountText.text = $"Blue Left: {maxBlueBlocks - currentBlueCount}";
    }

    void SpawnNewBlock()
    {
        Vector3 spawnPos = new Vector3(0f, spawnY, 0f);
        // 最初は必ず赤ブロックとして生成
        currentBlock = Instantiate(redPrefab, spawnPos, Quaternion.identity);

        Rigidbody2D rb = currentBlock.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; // 操作中は物理演算をオフ

        RedBlock block = currentBlock.GetComponent<RedBlock>();
        if (block != null)
        {
            // 確率でサイズを決定
            float r = Random.value;
            int randomShape;
            if (r < 0.8f) randomShape = 0;      // 80% Small
            else if (r < 0.95f) randomShape = 2; // 15% Large
            else randomShape = 1;               // 5% Medium

            block.shapeIndex = randomShape;
            switch (randomShape)
            {
                case 0: block.value = Random.Range(1, 5); break;
                case 1: block.value = Random.Range(5, 10); break;
                case 2: block.value = Random.Range(10, 20); break;
            }
            block.ApplyAllUpdates();
        }
    }

    IEnumerator WaitAndSpawnNext()
    {
        isWaitingNext = true;
        yield return new WaitForSeconds(1f);
        isWaitingNext = false;
    }

    void ToggleRedBlue()
    {
        if (currentBlock == null) return;

        SpriteRenderer sr = currentBlock.GetComponent<SpriteRenderer>();
        RedBlock red = currentBlock.GetComponent<RedBlock>();
        BlueBlock blue = currentBlock.GetComponent<BlueBlock>();

        if (red != null && blue == null)
        {
            // ■ 赤 → 青 に変換
            if (currentBlueCount >= maxBlueBlocks)
            {
                Debug.Log("青ブロックはもう使えません！");
                return;
            }

            if (sr != null) sr.color = Color.blue;

            // BlueBlockを追加し、値を引き継ぐ
            blue = currentBlock.AddComponent<BlueBlock>();
            blue.value = red.value;
            
            // 【重要】青ブロックの物理設定（重さなど）を適用
            blue.UpdatePhysics();

            Destroy(red); // 赤コンポーネントを削除
            currentBlueCount++;
        }
        else if (blue != null)
        {
            // ■ 青 → 赤 に戻す
            if (sr != null) sr.color = Color.white; // 赤ブロックは元のスプライトの色を使うので白に戻す

            red = currentBlock.AddComponent<RedBlock>();
            red.value = blue.value;
            red.ApplyAllUpdates();

            Destroy(blue); // 青コンポーネントを削除
            // 青ブロックの使用回数は戻さない（使ったら消費）
        }
    }
}