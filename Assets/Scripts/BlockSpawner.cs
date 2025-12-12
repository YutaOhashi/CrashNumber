using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class BlockSpawner : MonoBehaviour
{
    [Header("Block Prefabs")]
    public GameObject redPrefab;
    public GameObject bluePrefab;
    public GameObject greenPrefab;

    [Header("Spawn Settings")]
    public float moveSpeed = 5f;
    public float minX = -3.6f;
    public float maxX = 3.6f;
    public float spawnY = 4.5f;

    [Header("Blue Block Limit")]
    public int maxBlueBlocks = 10; // 最大使用回数
    private int currentBlueCount = 0; // 使用した青ブロックの数
    public TextMeshProUGUI blueCountText; // UIで残り回数を表示したい場合

    private GameObject currentBlock;
    private bool isWaitingNext = false;

    void Update()
    {
        if (isWaitingNext) return;

        // 現在操作中のブロックがある場合
        if (currentBlock != null)
        {
            Rigidbody2D body = currentBlock.GetComponent<Rigidbody2D>();
            if (body.bodyType == RigidbodyType2D.Kinematic)
            {
                Vector3 pos = currentBlock.transform.position;

                // 左右移動
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
            blueCountText.text = $"青ブロック残り: {maxBlueBlocks - currentBlueCount}";
    }

    void SpawnNewBlock()
    {
        Vector3 spawnPos = new Vector3(0f, spawnY, 0f);
        currentBlock = Instantiate(redPrefab, spawnPos, Quaternion.identity);

        Rigidbody2D rb = currentBlock.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        RedBlock block = currentBlock.GetComponent<RedBlock>();
        if (block != null)
        {
            float r = Random.value;
            int randomShape;
            if (r < 0.8f) randomShape = 0;
            else if (r < 0.95f) randomShape = 2;
            else randomShape = 1;

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
            // 赤→青に変換
            if (currentBlueCount >= maxBlueBlocks)
            {
                Debug.Log("青ブロックはもう使えません！");
                return;
            }

            if (sr != null) sr.color = Color.blue;

            blue = currentBlock.AddComponent<BlueBlock>();
            blue.value = red.value;

            Destroy(red); // 赤コンポーネントは削除
            currentBlueCount++;
        }
        else if (blue != null)
        {
            // 青→赤に戻す
            if (sr != null) sr.color = Color.red;

            red = currentBlock.AddComponent<RedBlock>();
            red.value = blue.value;
            red.ApplyAllUpdates();

            Destroy(blue); // 青コンポーネントは削除
        }
    }
}
