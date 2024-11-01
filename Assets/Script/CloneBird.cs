using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneBird : BirdBase
{
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneForce = 5f;  // Lực đẩy cho chim clone
    [SerializeField] private float spreadAngle = 30f;  // Góc lệch khi phân thân

    private bool _canClone = true;

protected override void Awake()
{
    SetIsClone(false); // Đảm bảo chim gốc không phải là clone
    base.Awake();

    // Setup physics properties for the clone in Awake()
    _rb = GetComponent<Rigidbody2D>();
    _circleCollider = GetComponent<Collider2D>();

    // Ensure that clone is ready to interact with physics
    _rb.isKinematic = false;  // Đặt thành false để vật lý hoạt động
    _circleCollider.enabled = true;  // Bật collider

    Debug.Log("CloneBird Awake: Rigidbody2D isKinematic: " + _rb.isKinematic + ", Collider2D enabled: " + _circleCollider.enabled);
}

    private void Update()
    {
        if (_hasBeenLaunched)
        {
            Debug.Log("Chim đã được phóng.");
        }
        else
        {
            Debug.Log("Chim chưa được phóng.");
        }

        if (_canClone)
        {
            Debug.Log("Có thể phân thân.");
        }
        else
        {
            Debug.Log("Không thể phân thân.");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Phím Space được nhấn.");
        }

        if (_hasBeenLaunched && _canClone && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Điều kiện thỏa mãn, kích hoạt phân thân.");
            ActivateSpecialAbility();
            _canClone = false;
            _rb.isKinematic = false;
            _circleCollider.enabled = true;
        }
    }

    public override void ActivateSpecialAbility()
    {
        Debug.Log("Kích hoạt năng lực đặc biệt: Phân thân.");

        // Tạo 3 bản clone với góc lệch
        CreateClone(spreadAngle);
        CreateClone(0);
        CreateClone(-spreadAngle);

        // Hủy đối tượng gốc sau khi clone
        Destroy(gameObject);
    }

    private void CreateClone(float angleOffset)
    {
        Debug.Log("Tạo chim clone với góc lệch: " + angleOffset);

        // Instantiate the clone at the current bird's position
        GameObject clone = Instantiate(clonePrefab, transform.position, Quaternion.identity);

        if (clone == null)
        {
            Debug.LogError("Không thể tạo chim clone!");
            return;
        }

        // Retrieve the CloneBird script from the clone
        CloneBird cloneBird = clone.GetComponent<CloneBird>();

        // Make sure this clone is marked as a clone before Awake() runs
        cloneBird.SetIsClone(true);

        // Copy layer and tag from the original bird
        clone.layer = gameObject.layer;
        clone.tag = gameObject.tag;

        // Calculate the current velocity of the original bird
        Vector2 currentVelocity = _rb.velocity.normalized;
        if (currentVelocity == Vector2.zero)
        {
            currentVelocity = Vector2.right; // Default direction if there's no velocity
        }

        // Apply the angle offset to the launch direction
        Vector2 direction = Quaternion.Euler(0, 0, angleOffset) * currentVelocity;
        Debug.Log("Phóng chim clone theo hướng: " + direction);

        // Multiply the force by 2 to launch the clone with double the force of the current bird
        float launchForce = _rb.velocity.magnitude * 2.0f; // Dựa trên vận tốc hiện tại và nhân lên 2 lần

        // Launch the clone bird using the calculated direction and the new force
        cloneBird.LaunchBird(direction, launchForce);

        // Coroutine to enable physics for clone after it's instantiated
        StartCoroutine(EnablePhysicsForClone(clone.GetComponent<Rigidbody2D>(), clone.GetComponent<Collider2D>()));
    }


    private IEnumerator EnablePhysicsForClone(Rigidbody2D cloneRb, Collider2D cloneCollider)
    {
        // Wait for one frame to ensure the clone is correctly instantiated before setting physics properties
        yield return new WaitForEndOfFrame();

        // Force clone to not be kinematic and ensure collider is enabled
        cloneRb.isKinematic = false;  // Đảm bảo clone bị ảnh hưởng bởi vật lý (không ở trạng thái Kinematic)
        if (cloneCollider != null)
        {
            cloneCollider.enabled = true;  // Đảm bảo collider được kích hoạt để có thể tương tác
        }

        // Debugging information to confirm changes
        Debug.Log("Physics enabled for clone: Rigidbody2D isKinematic: " + cloneRb.isKinematic + ", Collider2D enabled: " + cloneCollider.enabled);
    }



    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        // Xử lý va chạm
        Baddie baddie = collision.gameObject.GetComponent<Baddie>();
        if (baddie != null)
        {
            baddie.DamageBaddie(10);
        }

        Destroy(gameObject, 5f);
    }
}
