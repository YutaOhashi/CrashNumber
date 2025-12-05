using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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

    private GameObject currentBlock;
    private bool isWaitingNext = false;

    public enum BlockType { Red, Blue, Green }
    public BlockType nextBlockType = BlockType.Red; // デフォルト赤

    void Update()
    {
        // ★ Qキーで次のブロックを青に
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            nextBlockType = BlockType.Blue;
            Debug.Log("次のブロックは青になります！");
        }

        // ★ Gキーで次のブロックを緑に
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            nextBlockType = BlockType.Green;
            Debug.Log("次のブロックは緑になります！");
        }

        if (isWaitingNext) return;

        // ブロックがなければ生成
        if (currentBlock == null)
        {
            SpawnNewBlock();
            return;
        }

        Rigidbody2D body = currentBlock.GetComponent<Rigidbody2D>();

        // 落下中は操作不可
        if (body.bodyType == RigidbodyType2D.Dynamic)
        {
            currentBlock = null;
            return;
        }

        // 左右移動
        Vector3 pos = currentBlock.transform.position;
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
    }

    void SpawnNewBlock()
    {
        Vector3 spawnPos = new Vector3(0f, spawnY, 0f);
        GameObject prefabToSpawn = redPrefab; // デフォルト赤

        // nextBlockType に応じてPrefabを変更
        switch (nextBlockType)
        {
            case BlockType.Red: prefabToSpawn = redPrefab; break;
            case BlockType.Blue: prefabToSpawn = bluePrefab; break;
            case BlockType.Green: prefabToSpawn = greenPrefab; break;
        }

        currentBlock = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        // 生成後はデフォルト赤に戻す
        nextBlockType = BlockType.Red;

        Rigidbody2D rb = currentBlock.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        // 赤ブロックなら形状・値設定
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

        // 青・緑ブロックなら Rigidbody 等の設定だけでOK
        Rigidbody2D rbOther = currentBlock.GetComponent<Rigidbody2D>();
        if (rbOther != null)
        {
            rbOther.gravityScale = 1f;
            rbOther.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    IEnumerator WaitAndSpawnNext()
    {
        isWaitingNext = true;
        yield return new WaitForSeconds(1f);
        isWaitingNext = false;
    }
}
