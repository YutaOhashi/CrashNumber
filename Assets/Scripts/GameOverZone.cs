using UnityEngine;

public class GameOverZone : MonoBehaviour
{
    public GameObject gameOverText; // ゲームオーバーの文字
    public float timeLimit = 2.0f;  // 何秒はみ出したらアウトか

    private float timer = 0f;       // 時間計測用

    // 触れている間ずっと呼ばれる
    void OnTriggerStay2D(Collider2D collision)
    {
        // ぶつかっているのが「ブロック」かどうか確認
        if (collision.GetComponent<RedBlock>() != null || 
            collision.GetComponent<GreenBlock>() != null ||
            collision.GetComponent<BlueBlock>() != null)
        {
            // ずっと触れていたらタイマーを進める
            timer += Time.deltaTime;

            // 制限時間を超えたらゲームオーバー！
            if (timer > timeLimit)
            {
                GameOver();
            }
        }
    }

    // 離れたらリセット
    void OnTriggerExit2D(Collider2D collision)
    {
        timer = 0f;
    }

    void GameOver()
    {
        Debug.Log("Game Over!");
        
        // 文字を表示する
        if (gameOverText != null)
        {
            gameOverText.SetActive(true);
        }

        // ゲームの時間を止める
        Time.timeScale = 0f;
    }
}