using UnityEngine.Splines;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MoveToSpline))]
public class Card : MonoBehaviour
{
    private bool _isFinal = false;
    private bool _isMovement = false;

    // Buffer
    private Transform _thisTransform;
    private MoveToSpline _moveToSpline;

    private void Start()
    {
        _thisTransform = transform;
        _moveToSpline = GetComponent<MoveToSpline>();
    }

    private void Update()
    {
        if (_isMovement == true)
        {
            _thisTransform.Rotate(new Vector3(0, -500, 0) * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            Collectable collectable = other.GetComponent<Collectable>();

            if (collectable != null)
            {
                Debug.Log("IS COLLECTABLE - " + other.gameObject.name);
                Destroy(other.gameObject);

                return;
            }

            Debug.Log("IS FINAL - " + other.gameObject.name);
            finalPosition();
        }
    }

    public bool IsFinal => _isFinal;

    public void ResetPosition()
    {
        _moveToSpline.ResetPosition();
        _isMovement = false;
        _isFinal = false;
    }

    public void StartMovement(Spline spline)
    {
        _moveToSpline.StartMovement(spline);
        _thisTransform.rotation = Quaternion.identity;
        _isMovement = true;
    }

    private void finalPosition()
    {
        _isFinal = true;
        _isMovement = false;
        _moveToSpline.StopMovement();
    }
}
