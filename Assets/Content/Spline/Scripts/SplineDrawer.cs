using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineDrawer : MonoBehaviour
{
    public enum PositionType { Display, Surface }
    
    [SerializeField]
    private PositionType _positionType = PositionType.Display;

    // The minimum amount of cursor movement to be considered a new sample.
    const float StrokeDeltaThreshold = 0.1f;

    // Point reduction epsilon determines how aggressive the point reduction algorithm is when removing redundant points. 
    [SerializeField, Range(0f, 1f)]
    private float _pointReductionEpsilon = 0.15f;

    // Tension affects how "curvy" splines are at knots. 0 is a sharp corner, 1 is maximum curvitude.
    [SerializeField, Range(0f, 1f)]
    private float _splineTension = 0.25f;

    [SerializeField]
    private LayerMask _layerMask;
    [SerializeField]
    private bool _testDrawing = false;

    private Camera _camera;
    private List<float3> _stroke = new List<float3>(1024);
    private List<float3> _reduced = new List<float3>(512);
    private bool _painting;
    private Vector3 _lastMousePosition;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void RebuildSpline()
    {
        // Before setting spline knots, reduce the number of sample points.
        SplineUtility.ReducePoints(_stroke, _reduced, _pointReductionEpsilon);

        Spline spline = GetComponent<SplineContainer>().Spline;

        // Assign the reduced sample positions to the Spline knots collection. Here we are constructing new
        // BezierKnots from a single position, disregarding tangent and rotation. The tangent and rotation will be
        // calculated automatically in the next step wherein the tangent mode is set to "Auto Smooth."
        spline.Knots = _reduced.Select(x => new BezierKnot(x));

        var all = new SplineRange(0, spline.Count);

        // Sets the tangent mode for all knots in the spline to "Auto Smooth."
        spline.SetTangentMode(all, TangentMode.AutoSmooth);

        // Sets the tension parameter for all knots. Note that the "Tension" parameter is only applicable to
        // "Auto Smooth" mode knots.
        spline.SetAutoSmoothTension(all, _splineTension);
    }

    private void AddPoint()
    {
        Vector2 inputMousePosition = Input.mousePosition;
        _lastMousePosition = inputMousePosition;

        if (_positionType == PositionType.Display)
        {
            Vector3 worldPoint = _lastMousePosition;
            worldPoint.z = 1.0f;
            _stroke.Add(_camera.ScreenToWorldPoint(worldPoint));
        }
        else
        {
            RaycastHit hit;

            if (Physics.Raycast(_camera.ScreenPointToRay(inputMousePosition), out hit, Mathf.Infinity, _layerMask))
            {
                _stroke.Add(hit.point);
            }
        }

        RebuildSpline();
    }

    private void Update()
    {
        if (_testDrawing == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartDrawing();
            }

            HoldDrawing();

            if (Input.GetMouseButtonUp(0))
            {
                EndDrawing();
            }
        }
    }

    public void StartDrawing()
    {
        _painting = true;
        _stroke.Clear();
        AddPoint();
    }

    public void HoldDrawing()
    {
        if (_painting && Vector2.Distance(Input.mousePosition, _lastMousePosition) > StrokeDeltaThreshold)
        {
            AddPoint();
        }
    }

    public void EndDrawing()
    {
        _painting = false;
    }
}