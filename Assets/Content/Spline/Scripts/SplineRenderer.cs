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
    private SplineContainer _splineContainer;
    private Spline[] _splines;
    private bool _dirty;
    private Vector3[] _points;

    [SerializeField]
    private SplineLineRendererSettings _lineRendererSettings = new SplineLineRendererSettings()
    {
        Width = 0.5f,
        Subdivisions = 64
    };

    private LineRenderer[] _lines;

    private void Awake()
    {
        _splineContainer = GetComponent<SplineContainer>();
        _splines = _splineContainer.Splines.ToArray();
    }

    private void OnEnable()
    {
        //_splineContainer.Spline.changed += OnSplineChanged;

        Spline.Changed += OnSplineChanged;
    }

    private void OnDisable()
    {
        //_splineContainer.Spline.changed -= OnSplineChanged;

        Spline.Changed -= OnSplineChanged;
    }

    private void OnSplineChanged()
    {
        if (_splineContainer.Spline.Count > 1)
        {
            for (int i = 0, c = _splines.Length; !_dirty && i < c; ++i)
                if (_splines[i] == _splineContainer.Spline)
                    _dirty = true;

            SplineUpdate();
        }
    }

    private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
    {
        if (_splineContainer.Spline.Count > 1)
        {
            for (int i = 0, c = _splines.Length; !_dirty && i < c; ++i)
                if (_splines[i] == _splineContainer.Spline)
                    _dirty = true;

            SplineUpdate();
        }
    }

    private void SplineUpdate()
    {
        if (_lines?.Length != _splines.Length)
        {
            if (_lines != null)
                foreach (var line in _lines) DestroyImmediate(line.gameObject);

            _lines = new LineRenderer[_splines.Length];

            for (int i = 0, c = _splines.Length; i < c; ++i)
            {
                _lines[i] = new GameObject().AddComponent<LineRenderer>();
                _lines[i].gameObject.name = $"SplineRenderer {i}";
                _lines[i].transform.SetParent(transform, true);
            }

            _dirty = true;
        }

        // It's nice to be able to see resolution changes at runtime
        if (_points?.Length != _lineRendererSettings.Subdivisions)
        {
            _dirty = true;
            _points = new Vector3[_lineRendererSettings.Subdivisions];
            foreach (var line in _lines)
                line.positionCount = _lineRendererSettings.Subdivisions;
        }

        if (!_dirty)
            return;

        _dirty = false;
        var trs = _splineContainer.transform.localToWorldMatrix;

        for (int s = 0, c = _splines.Length; s < c; ++s)
        {
            if (_splines[s].Count < 1)
                continue;

            for (int i = 0; i < _lineRendererSettings.Subdivisions; i++)
                _points[i] = math.transform(trs, _splines[s].EvaluatePosition(i / (_lineRendererSettings.Subdivisions - 1f)));

            _lines[s].widthCurve = new AnimationCurve(new Keyframe(0f, _lineRendererSettings.Width));
            _lines[s].startColor = _lineRendererSettings.StartColor;
            _lines[s].endColor = _lineRendererSettings.EndColor;
            _lines[s].material = _lineRendererSettings.Material;
            _lines[s].useWorldSpace = true;
            _lines[s].SetPositions(_points);
        }
    }
}