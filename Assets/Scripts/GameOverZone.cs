using UnityEngine;

public class GameOverZone : MonoBehaviour
{
    public GameObject gameOverText; // �Q�[���I�[�o�[�̕���
    public float timeLimit = 2.0f;  // ���b�͂ݏo������A�E�g��

    private float timer = 0f;       // ���Ԍv���p

    // �G��Ă���Ԃ����ƌĂ΂��
    void OnTriggerStay2D(Collider2D collision)
    {
        // �Ԃ����Ă���̂��u�u���b�N�v���ǂ����m�F
        if (collision.GetComponent<RedBlock>() != null || 
            collision.GetComponent<GreenBlock>() != null)
        {
            // �����ƐG��Ă�����^�C�}�[��i�߂�
            timer += Time.deltaTime;

            // �������Ԃ𒴂�����Q�[���I�[�o�[�I
            if (timer > timeLimit)
            {
                GameOver();
            }
        }
    }

    // ���ꂽ�烊�Z�b�g
    void OnTriggerExit2D(Collider2D collision)
    {
        timer = 0f;
    }

    void GameOver()
    {
        Debug.Log("Game Over!");
        
        // ������\������
        if (gameOverText != null)
        {
            gameOverText.SetActive(true);
        }

        // �Q�[���̎��Ԃ��~�߂�
        Time.timeScale = 0f;
    }
}