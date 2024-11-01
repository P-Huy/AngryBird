using System.Collections;
using UnityEngine;

public class ScorePopup3D : MonoBehaviour
{
    public float moveUpSpeed = 1f;
    public float fadeOutDuration = 1f;
    private TextMesh textMesh;

    private void Awake()
    {
        // Tìm kiếm TextMesh trong đối tượng con nếu không tìm thấy trên đối tượng chính
        textMesh = GetComponentInChildren<TextMesh>();
        if (textMesh == null)
        {
            Debug.LogError("TextMesh component not found on ScorePopup3D prefab or its children.");
        }
    }


    public void Setup(int score)
    {
        textMesh.text = "+" + score;
        StartCoroutine(FadeOutAndMoveUp());
    }

    private IEnumerator FadeOutAndMoveUp()
    {
        float elapsedTime = 0f;
        Color originalColor = textMesh.color;

        while (elapsedTime < fadeOutDuration)
        {
            // Di chuyển popup lên trên
            transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;

            // Dần dần làm mờ text
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeOutDuration);
            textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject); // Xóa popup sau khi hiệu ứng kết thúc
    }
}
