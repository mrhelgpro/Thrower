using System;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[Serializable]
public struct SplineLineRendererSettings
{
    public float Width;
    public Material Material;
    [Range(16, 512)]
    public int Subdivisions;
    public Color StartColor;
    public Color EndColor;
}

[RequireComponent(typeof(SplineContainer))]
public class SplineRenderer : MonoBehaviour
{
    public SplineLineRendererSettings RendererSettings;
    
    private SplineContainer _splineContainer;
    private Spline _spline;
    private LineRenderer _line;
    private bool _dirty;
    private Vector3[] _points;

    private void Awake()
    {
        _splineContainer = GetComponent<SplineContainer>();
        _spline = _splineContainer.Spline;
    }

    private void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
    }

    private void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
    }

    private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
    {
        if (_splineContainer.Spline.Count > 1)
        {
            _dirty = true;
            SplineUpdate();

            return;
        }

        if (_line != null)
        {
            _line.enabled = false;
        } 
    }

    private void SplineUpdate()
    {
        if (_line == null)
        {
            _line = new LineRenderer();

            _line = new GameObject().AddComponent<LineRenderer>();
            _line.gameObject.name = "SplineRenderer";
            _line.transform.SetParent(transform, true);

            _dirty = true;
        }

        _line.enabled = true;

        // It's nice to be able to see resolution changes at runtime
        if (_points?.Length != RendererSettings.Subdivisions)
        {
            _dirty = true;
            _points = new Vector3[RendererSettings.Subdivisions];
            _line.positionCount = RendererSettings.Subdivisions;
        }

        if (_dirty == false)
        {
            return;
        }

        _dirty = false;
        var trs = _splineContainer.transform.localToWorldMatrix;

        for (int i = 0; i < RendererSettings.Subdivisions; i++)
        {
            _points[i] = math.transform(trs, _spline.EvaluatePosition(i / (RendererSettings.Subdivisions - 1f)));
        }

        _line.widthCurve = new AnimationCurve(new Keyframe(0f, RendererSettings.Width));
        _line.startColor = RendererSettings.StartColor;
        _line.endColor = RendererSettings.EndColor;
        _line.material = RendererSettings.Material;
        _line.useWorldSpace = true;
        _line.SetPositions(_points);
    }
}