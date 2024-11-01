using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BirdBase : MonoBehaviour
{
    protected Rigidbody2D _rb;
    protected Collider2D _circleCollider;
    protected bool _hasBeenLaunched;
    private bool _isClone = false; // Thêm biến cờ để xác định chim clone

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<Collider2D>();

        if (!_isClone)
        {
            InitializeBird();
        }
    }

    public void SetIsClone(bool value)
    {
        _isClone = value;
    }

    protected virtual void InitializeBird()
    {
        _rb.isKinematic = true;
        _circleCollider.enabled = false;
    }

    public virtual void LaunchBird(Vector2 direction, float force)
    {
        _rb.isKinematic = false;  // Không để Kinematic
        _circleCollider.enabled = true;  // Bật collider để có thể tương tác
        _rb.AddForce(direction * force, ForceMode2D.Impulse);
        _hasBeenLaunched = true;

        Debug.Log("LaunchBird() được gọi, _hasBeenLaunched: " + _hasBeenLaunched);
    }


    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // Có thể triển khai logic va chạm trong các lớp con nếu cần
    }

    public abstract void ActivateSpecialAbility();
}
