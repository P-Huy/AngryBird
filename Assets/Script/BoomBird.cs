using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomBird : BirdBase
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionRadius = 5f; // Bán kính gây sát thương cho các baddies xung quanh
    [SerializeField] private float explosionDamage = 1f; // Lượng sát thương áp dụng cho baddies trong phạm vi

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        ActivateSpecialAbility(); // Kích hoạt nổ khi va chạm
    }

    public override void ActivateSpecialAbility()
    {
        // Tạo hiệu ứng nổ
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D obj in hitObjects)
        {
            Baddie baddie = obj.GetComponent<Baddie>();
            if (baddie != null)
            {
                baddie.DamageBaddie(explosionDamage); // Áp dụng sát thương cho Baddie
            }
        }

        Destroy(gameObject); // Xóa BoomBird sau khi nổ
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
