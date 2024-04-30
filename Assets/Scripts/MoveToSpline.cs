using UnityEngine;
using UnityEngine.Splines;

public class MoveToSpline : MonoBehaviour
{
    private const float Threshold = 0.1f;
    
    [SerializeField, Range (0, 20)] private float moveSpeed = 5;

    private bool _isMovement;
    private Transform _thisTransform;
    private Transform _parentTransform;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Spline _spline;
    private int _currentPointNumber;
    private Vector3 _moveDirection;

    private void Awake()
    {
        _thisTransform = transform;
        _parentTransform = _thisTransform.parent;
        _startPosition = _thisTransform.localPosition;
        _startRotation = _thisTransform.localRotation;
    }

    private void FixedUpdate()
    {
        if (_isMovement)
        {
            MoveTo();
        }
    }

    public void ResetPosition()
    {
        _isMovement = false;
        _thisTransform.parent = _parentTransform;
        _thisTransform.localPosition = _startPosition;
        _thisTransform.localRotation = _startRotation;
        _spline = null;
    }

    public void StartMovement(Spline spline)
    {
        if (spline.Count > 1)
        {
            _isMovement = true;
            _thisTransform.parent = null;
            _spline = spline;
            _currentPointNumber = 0;
        }
    }

    public void StopMovement()
    {
        _isMovement = false;
    }
    
    private Vector3 GetTargetPosition => _spline[_currentPointNumber].Position;
    private Vector3 GetTargetDirection => (GetTargetPosition - _thisTransform.position).normalized;
    
    private void MoveTo()
    {
        _thisTransform.position += _moveDirection * (moveSpeed * Time.fixedDeltaTime);

        if (_currentPointNumber == _spline.Count - 1)
        {
            return;
        }

        float distance = Vector3.Distance(_thisTransform.position, GetTargetPosition);

        if (distance < Threshold)
        {
            if (_currentPointNumber < _spline.Count - 1)
            {
                _currentPointNumber++;

                if (_currentPointNumber == _spline.Count - 1)
                {
                    _moveDirection = GetTargetDirection;

                    return;
                }
            }
        }

        _moveDirection = GetTargetDirection;
    }
}