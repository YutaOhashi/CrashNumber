using UnityEngine;
using TMPro; // TextMeshProを使うために必要

public class ScoreManager : MonoBehaviour
{
    // どこからでも ScoreManager.Instance でアクセスできるようにする（シングルトン）
    public static ScoreManager Instance { get; private set; }

    [Header("UI Reference")]
    public TextMeshProUGUI scoreText; // 画面表示用のテキスト

    private int currentScore = 0;

    void Awake()
    {
        // 自分自身をInstanceに登録
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreText();
    }

    // スコアを加算する関数
    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreText();
    }

    // テキスト表示を更新
    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString();
        }
    }
}