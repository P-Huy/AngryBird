using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggBird : BirdBase
{
    [SerializeField] private GameObject eggPrefab;

    public override void ActivateSpecialAbility()
    {
        // Đẻ trứng khi kích hoạt năng lực
        Instantiate(eggPrefab, transform.position, Quaternion.identity);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        // Kích hoạt năng lực đẻ trứng khi va chạm
        ActivateSpecialAbility();
    }
}
