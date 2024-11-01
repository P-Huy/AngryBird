using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Baddie : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 3f;
    [SerializeField] private float _damageThreshold = 0.2f;
    [SerializeField] private GameObject _baddieDeathParticle;
    [SerializeField] private AudioClip _deathClip;
    [SerializeField] private int pointValue = 100; // Điểm số của mỗi Baddie khi bị tiêu diệt
    [SerializeField] private GameObject scorePopupPrefab; // Prefab hiển thị điểm


    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void DamageBaddie(float damageAmount)
    {
        _currentHealth -= damageAmount;

        if (_currentHealth <= 0f)
        {
            Die();
        }
    }


    private void Die()
    {
        Debug.Log("Baddie chết - Điểm đang được hiển thị.");
        GameManager.instance.RemoveBaddie(this);
        Instantiate(_baddieDeathParticle, transform.position, Quaternion.identity);

        GameManager.instance.AddScore(pointValue);

        ShowScorePopup();
        Destroy(gameObject);
    }

    private void ShowScorePopup()
    {
        if (scorePopupPrefab != null)
        {
            Vector3 offset = new Vector3(0, 1.5f, 0);
            GameObject popup = Instantiate(scorePopupPrefab, transform.position + offset, Quaternion.identity);
            popup.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); // Điều chỉnh scale để phù hợp

            Debug.Log("ScorePopup instantiated at position: " + popup.transform.position + " with scale: " + popup.transform.localScale);

            ScorePopup scorePopup = popup.GetComponent<ScorePopup>();
            if (scorePopup != null)
            {
                scorePopup.Setup(pointValue);
            }
            else
            {
                Debug.LogWarning("ScorePopup script not found on the instantiated prefab.");
            }
        }
        else
        {
            Debug.LogWarning("ScorePopupPrefab chưa được gán trong Inspector.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float impactVelocity = collision.relativeVelocity.magnitude;

        if (impactVelocity > _damageThreshold)
        {
            DamageBaddie(impactVelocity);
        }
    }


}
