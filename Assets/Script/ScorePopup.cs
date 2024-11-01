using System.Collections;
using TMPro; // Sử dụng TextMeshPro
using UnityEngine;

public class ScorePopup : MonoBehaviour
{
    [SerializeField] private float moveUpSpeed = 1f;
    [SerializeField] private float fadeOutDuration = 1f;
    private TextMeshProUGUI scoreText;

    private void Awake()
    {
        scoreText = GetComponentInChildren<TextMeshProUGUI>();
        if (scoreText == null)
        {
            Debug.LogError("TextMeshProUGUI component not found in ScorePopup prefab. Make sure there is a Text (TMP) component in the prefab.");
        }
    }

    // Thiết lập điểm hiển thị
    public void Setup(int score)
    {
        scoreText.text = "+" + score;
        StartCoroutine(FadeOutAndMoveUp());
    }

    private IEnumerator FadeOutAndMoveUp()
    {
        float elapsedTime = 0f;
        Color originalColor = scoreText.color;

        while (elapsedTime < fadeOutDuration)
        {
            // Di chuyển popup lên trên
            transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;

            // Dần dần làm mờ text
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeOutDuration);
            scoreText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject); // Xóa popup sau khi hiệu ứng kết thúc
    }
}
