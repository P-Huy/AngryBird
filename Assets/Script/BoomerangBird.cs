using System.Collections;
using UnityEngine;
using DG.Tweening;

public class BoomerangBird : BirdBase
{
    [SerializeField] private float rotationSpeed = 720f;      // Tốc độ xoay của chim
    [SerializeField] private float boomerangHeight = 2f;      // Độ cao khi chim bay cong
    [SerializeField] private float boomerangDistance = 5f;    // Khoảng cách mà chim sẽ di chuyển ngược lại
    [SerializeField] private float boomerangDuration = 1.5f;  // Thời gian quay lại theo quỹ đạo cong
    [SerializeField] private float increasedDamage = 200f;    // Sát thương tăng khi bấm Space
    [SerializeField] private float returnForceMultiplier = 3f; // Hệ số tăng lực khi quay lại

    private bool isReturning = false;  // Cờ kiểm tra khi chim đang quay lại
    private bool isBoosting = false;   // Cờ kiểm tra khi chim đang tăng tốc và sát thương

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        // Kiểm tra nếu phím Space được nhấn để kích hoạt boomerang
        if (_hasBeenLaunched && !isReturning && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Phím Space được nhấn, chim sẽ quay ngược lại với lực mạnh hơn!");
            ActivateSpecialAbility();  // Kích hoạt khả năng boomerang
        }
    }

    // Kích hoạt khả năng đặc biệt cho quỹ đạo boomerang quay ngược lại với lực mạnh hơn
    public override void ActivateSpecialAbility()
    {
        if (isReturning)
        {
            return;  // Không thực hiện lại nếu chim đã đang quay lại
        }

        isReturning = true;
        isBoosting = true;

        // 1. Xoay chim quanh trục Z trong khi bay
        transform.DORotate(new Vector3(0, 0, -360), 1f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);  // Xoay liên tục

        // 2. Tạo quỹ đạo boomerang quay ngược lại (theo đường cong)
        Vector3 startPosition = transform.position;  // Vị trí hiện tại của chim
        Vector3 apexPosition = startPosition + new Vector3(-boomerangDistance / 2, boomerangHeight, 0);  // Điểm cao nhất của quỹ đạo
        Vector3 returnPosition = startPosition + new Vector3(-boomerangDistance, 0, 0);  // Vị trí chim sẽ quay ngược lại

        // Sử dụng DOTween để di chuyển quay ngược với quỹ đạo cong
        Sequence boomerangSequence = DOTween.Sequence();
        boomerangSequence.Append(transform.DOMove(apexPosition, boomerangDuration / 2).SetEase(Ease.OutQuad));  // Quỹ đạo cong lên cao
        boomerangSequence.Append(transform.DOMove(returnPosition, boomerangDuration / 2).SetEase(Ease.InQuad));  // Quay lại vị trí

        // 3. Sau khi bắt đầu quay lại, áp dụng thêm lực để di chuyển nhanh hơn
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 returnDirection = (returnPosition - transform.position).normalized;  // Hướng quay lại
        rb.AddForce(returnDirection * returnForceMultiplier, ForceMode2D.Impulse);   // Áp dụng lực mạnh hơn

        boomerangSequence.OnComplete(() =>
        {
            isReturning = false;
            Debug.Log("BoomerangBird đã quay ngược lại và hoàn thành quỹ đạo!");
        });
    }

    // Xử lý va chạm khi chim đang bay theo quỹ đạo boomerang quay ngược lại
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBoosting)
        {
            Debug.Log("BoomerangBird đã va chạm khi đang quay ngược lại!");

            // Tìm kẻ thù và gây sát thương lớn hơn
            Baddie baddie = collision.gameObject.GetComponent<Baddie>();
            if (baddie != null)
            {
                // Gây sát thương mạnh hơn cho kẻ thù
                baddie.DamageBaddie(increasedDamage);  // Gây sát thương lớn khi bấm Space
                Debug.Log("Gây sát thương lớn: " + increasedDamage);
            }

            // Sau va chạm, dừng xoay và hủy chim
            isBoosting = false;
            transform.DOKill();  // Dừng mọi hiệu ứng xoay
            Destroy(gameObject, 4f);  // Phá hủy chim sau khi va chạm
        }

        // Gọi logic va chạm cơ bản từ BirdBase
        base.OnCollisionEnter2D(collision);
    }
}
