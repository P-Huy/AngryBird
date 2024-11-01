using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlingShotHandler : MonoBehaviour
{
    [Header("Line Renderers")]
    [SerializeField] private LineRenderer _leftLineRenderer;
    [SerializeField] private LineRenderer _rightLineRenderer;

    [Header("Transform References")]
    [SerializeField] private Transform _leftStartPosition;
    [SerializeField] private Transform _rightStartPosition;
    [SerializeField] private Transform _centerPosition;
    [SerializeField] private Transform _idlePosition;
    [SerializeField] private Transform _elasticTransform;

    [Header("Slingshot Stats")]
    [SerializeField] private float _maxDistance = 3.5f;
    [SerializeField] private float _shotForce = 5f;
    [SerializeField] private float _timeBetweenBirdRespawns = 2f;
    [SerializeField] private float _elasticDivider = 1.2f;
    [SerializeField] private AnimationCurve _elasticCurve;

    [Header("Scripts")]
    [SerializeField] private SlingShotArea _slingShotArea;
    [SerializeField] private CameraManager _cameraManager;

    [Header("Bird Types")]
    [SerializeField] private BirdBase[] birdPrefabs; // Danh sách các loại chim
    [SerializeField] private float _birdPositionOffset = 2f;

    [Header("Sounds")]
    [SerializeField] private AudioClip _elasticPulledClip;
    [SerializeField] private AudioClip[] _elasticReleasedClips;

    private Vector2 _slingShotLinesPosition;
    private Vector2 _direction;
    private Vector2 _directionNormalized;

    private bool _clickedWithinArea;
    private bool _birdOnSlingshot;

    private BirdBase _spawnedBird;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _leftLineRenderer.enabled = false;
        _rightLineRenderer.enabled = false;

        SpawnBird();
    }

    private void Update()
    {
        if (InputManager.WasLeftMouseButtonPressed && _slingShotArea.IsWithinSlingshotArea())
        {
            _clickedWithinArea = true;
            if (_birdOnSlingshot)
            {
                _audioSource.PlayOneShot(_elasticPulledClip);
                _cameraManager.SwitchToFollowCam(_spawnedBird.transform);
            }
        }

        if (InputManager.IsLeftMousePressed && _clickedWithinArea && _birdOnSlingshot)
        {
            DrawSlingShot();
            PositionAndRotateBird();
        }

        if (InputManager.WasLeftMouseButtonReleased && _clickedWithinArea && _birdOnSlingshot)
        {
            _clickedWithinArea = false;
            _birdOnSlingshot = false;

            float pullDistance = Vector2.Distance(_slingShotLinesPosition, _centerPosition.position);
            float finalShotForce = Mathf.Clamp(pullDistance * _shotForce, 0f, _shotForce * _maxDistance);

            _audioSource.PlayOneShot(_elasticReleasedClips[1]);

            // Bắn chim (có thể là CloneBird, BoomerangBird, hoặc loại khác)
            _spawnedBird.LaunchBird(_direction, finalShotForce);

            GameManager.instance.UseShot();

            AnimateSlingShot();

            if (GameManager.instance.HasEnoughShots())
            {
                StartCoroutine(SpawnBirdAfterTime());
            }
        }
    }

    #region SlingShot Methods
    private void DrawSlingShot()
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);
        _slingShotLinesPosition = _centerPosition.position + Vector3.ClampMagnitude(touchPosition - _centerPosition.position, _maxDistance);
        SetLines(_slingShotLinesPosition);
        _direction = (Vector2)_centerPosition.position - _slingShotLinesPosition;
        _directionNormalized = _direction.normalized;
    }

    private void SetLines(Vector2 position)
    {
        if (!_leftLineRenderer.enabled && !_rightLineRenderer.enabled)
        {
            _leftLineRenderer.enabled = true;
            _rightLineRenderer.enabled = true;
        }

        _leftLineRenderer.SetPosition(0, position);
        _leftLineRenderer.SetPosition(1, _leftStartPosition.position);
        _rightLineRenderer.SetPosition(0, position);
        _rightLineRenderer.SetPosition(1, _rightStartPosition.position);
    }
    #endregion

    #region Bird Methods

    private void SpawnBird()
    {
        SetLines(_idlePosition.position);

        Vector2 dir = (_centerPosition.position - _idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)_idlePosition.position + dir * _birdPositionOffset;

        // Lựa chọn ngẫu nhiên một loại chim từ danh sách các loại chim
        int birdIndex = Random.Range(0, birdPrefabs.Length);
        _spawnedBird = Instantiate(birdPrefabs[birdIndex], spawnPosition, Quaternion.identity);
        _spawnedBird.transform.right = dir;

        _birdOnSlingshot = true;
    }

    private void PositionAndRotateBird()
    {
        _spawnedBird.transform.position = _slingShotLinesPosition + _directionNormalized * _birdPositionOffset;
        _spawnedBird.transform.right = _directionNormalized;
    }

    private IEnumerator SpawnBirdAfterTime()
    {
        yield return new WaitForSeconds(_timeBetweenBirdRespawns);
        SpawnBird();
        _cameraManager.SwitchToIdleCam();
    }
    #endregion

    #region Animate SlingShot
    private void AnimateSlingShot()
    {
        _elasticTransform.position = _leftLineRenderer.GetPosition(0);

        float dist = Vector2.Distance(_elasticTransform.position, _centerPosition.position);
        float time = dist / _elasticDivider;

        _elasticTransform.DOMove(_centerPosition.position, time).SetEase(_elasticCurve);
        StartCoroutine(AnimateSlingShotLines(_elasticTransform, time));
    }

    private IEnumerator AnimateSlingShotLines(Transform trans, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            SetLines(trans.position);
            yield return null;
        }
    }
    #endregion
}
