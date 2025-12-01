using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BlockSpawner : MonoBehaviour
{
    [Header("Block Settings")]
    public GameObject blockPrefab;
    public float moveSpeed = 5f;
    public float minX = -3.6f;
    public float maxX = 3.6f;
    public float spawnY = 4.5f;

    private GameObject currentBlock;
    private bool isWaitingNext = false;   // ★次の生成を待っているか

    void Update()
    {
        // ★待機中は操作も生成もできない
        if (isWaitingNext)
            return;

        // ブロックがなければ生成
        if (currentBlock == null)
        {
            SpawnNewBlock();
            return;
        }

        Rigidbody2D body = currentBlock.GetComponent<Rigidbody2D>();

        // 落下中のブロックは操作不可
        if (body.bodyType == RigidbodyType2D.Dynamic)
        {
            currentBlock = null;
            return;
        }

        Vector3 pos = currentBlock.transform.position;

        // ← A
        if (Keyboard.current.aKey.isPressed)
            pos.x -= moveSpeed * Time.deltaTime;

        // → D
        if (Keyboard.current.dKey.isPressed)
            pos.x += moveSpeed * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        currentBlock.transform.position = pos;

        // F で落下
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            body.bodyType = RigidbodyType2D.Dynamic;
            currentBlock = null;

            // ★1秒待ってから次のブロック生成
            StartCoroutine(WaitAndSpawnNext());
        }
    }

    // ----------------------------------------
    // ブロック生成（重み付きランダム形状・ランダム値付き）
    // ----------------------------------------
    void SpawnNewBlock()
    {
        Vector3 spawnPos = new Vector3(0f, spawnY, 0f);

        // ブロック生成
        currentBlock = Instantiate(blockPrefab, spawnPos, Quaternion.identity);

        // Rigidbody を設定
        Rigidbody2D rb = currentBlock.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        // RedBlock コンポーネント取得
        RedBlock block = currentBlock.GetComponent<RedBlock>();
        if (block != null)
        {
            // ----------------------------------------
            // 重み付きランダムで形状を決定
            // 確率例: 〇=50%, □=35%, △=15%
            // 0:〇, 1:△, 2:□
            // ----------------------------------------
            float r = Random.value; // 0～1 の乱数
            int randomShape;

            if (r < 0.8f) randomShape = 0; // 〇
            else if (r < 0.95f) randomShape = 2; // □
            else randomShape = 1; // △

            block.shapeIndex = randomShape;

            // 形状に応じて value をランダム設定
            switch (randomShape)
            {
                case 0: // 〇
                    block.value = Random.Range(1, 5);    // 1~4
                    break;
                case 1: // △
                    block.value = Random.Range(5, 10);   // 5~9
                    break;
                case 2: // □
                    block.value = Random.Range(10, 20);  // 10~19
                    break;
            }

            // すべての更新処理を呼ぶ
            block.ApplyAllUpdates();
        }
    }

    // ----------------------------------------
    // ★ 1秒待ってから次のブロックを生成
    // ----------------------------------------
    IEnumerator WaitAndSpawnNext()
    {
        isWaitingNext = true;

        yield return new WaitForSeconds(1f);   // ← ここで1秒待つ

        isWaitingNext = false;
    }
}