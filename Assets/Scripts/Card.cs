using UnityEngine.Splines;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MoveToSpline))]
public class Card : MonoBehaviour
{
    [SerializeField] private AudioSource flySound;
    [SerializeField] private AudioSource hitSound;

    private bool _isMovement;
    
    private Transform _thisTransform;
    private TrailRenderer _trailRenderer;
    private MoveToSpline _moveToSpline;

    private void Start()
    {
        _thisTransform = transform;
        _trailRenderer = GetComponent<TrailRenderer>();
        _moveToSpline = GetComponent<MoveToSpline>();
        
        _trailRenderer.enabled = false;
    }

    private void Update()
    {
        if (_isMovement)
        {
            _thisTransform.Rotate(new Vector3(0, -500, 0) * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Default")) return;
        
        Collectable collectable = other.GetComponent<Collectable>();

        if (collectable != null)
        {
            collectable.Interaction();
            return;
        }
            
        FinalPosition();
    }

    public bool IsFinal { get; private set; }

    public void ResetPosition()
    {
        _moveToSpline.ResetPosition();
        _isMovement = false;
        IsFinal = false;
        _trailRenderer.enabled = false;
    }

    public void StartMovement(Spline spline)
    {
        _moveToSpline.StartMovement(spline);
        _thisTransform.rotation = Quaternion.identity;
        _isMovement = true;
        _trailRenderer.enabled = true;
        flySound.Play();
    }

    private void FinalPosition()
    {
        IsFinal = true;
        _isMovement = false;
        _moveToSpline.StopMovement();
        _trailRenderer.enabled = false;
        
        flySound.Stop();
        hitSound.Play();
    }
}