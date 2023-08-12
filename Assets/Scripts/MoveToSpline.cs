using UnityEngine;
using UnityEngine.Splines;

public class MoveToSpline : MonoBehaviour
{
    public enum ActionType { StartPoint, MoveToSpline, DestinationPoint }

    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField, Range (0, 20)] private float _moveSpeed = 5;

    // Buffer
    private Transform _thisTransform;
    private Transform _parentTransform;
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    // Movement Data
    private ActionType _actionType = ActionType.StartPoint;
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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ResetPosition();
        }

        if (Input.GetMouseButtonUp(0))
        {
            StartMovement(_splineContainer.Spline);
        }
    }

    private void FixedUpdate()
    {
        if (_actionType == ActionType.MoveToSpline)
        {
            moveToSpline();
        }
    }

    public void ResetPosition()
    {
        _actionType = ActionType.StartPoint;
        _thisTransform.parent = _parentTransform;
        _thisTransform.localPosition = _startPosition;
        _thisTransform.localRotation = _startRotation;
        _spline = null;
    }

    public void StartMovement(Spline spline)
    {
        if (spline.Count > 1)
        {
            _actionType = ActionType.MoveToSpline;
            _thisTransform.parent = null;
            _spline = spline;
            _currentPointNumber = 0;
        }
    }

    // Get Values
    private Vector3 getTargetPosition => _spline[_currentPointNumber].Position;
    private Vector3 getTargetDirection => (getTargetPosition - _thisTransform.position).normalized;

    // Private Methods
    private void moveToSpline()
    {
        _thisTransform.position += _moveDirection * _moveSpeed * Time.fixedDeltaTime;

        if (_currentPointNumber == _spline.Count - 1)
        {
            return;
        }

        float distance = Vector3.Distance(_thisTransform.position, getTargetPosition);

        if (distance < 0.1f)
        {
            if (_currentPointNumber < _spline.Count - 1)
            {
                _currentPointNumber++;

                if (_currentPointNumber == _spline.Count - 1)
                {
                    _moveDirection = getTargetDirection;

                    return;
                }
            }
        }

        _moveDirection = getTargetDirection;
    }

    private void destination()
    {
        _actionType = ActionType.DestinationPoint;
    }
}
