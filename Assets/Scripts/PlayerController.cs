using UnityEngine.Splines;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum ActionType { Menu, Idle, Prepare, Throw, Observation }
    private ActionType _actionType = ActionType.Idle;

    [SerializeField] private SplineDrawer _splineDrawerDisplay;
    [SerializeField] private SplineDrawer _splineDrawerThrowing;
    [SerializeField] private SplineContainer _splineContainerThrowing;
    [SerializeField] private MoveToSpline _moveToSpline;

    private void Update()
    {
        if (_actionType == ActionType.Idle)
        {
            if (Input.GetMouseButtonDown(0))
            {
                prepareThrowing();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            endDrawing();

            if (_actionType == ActionType.Prepare)
            {
                if (_splineContainerThrowing.Spline.Count > 1)
                {
                    startThrowing();
                }
                else
                {
                    idleThrowing();
                }
            }
        }

        holdDrawing();
    }

    // Drawing
    private void startDrawing()
    {
        _moveToSpline.ResetPosition();
        _splineDrawerDisplay.StartDrawing();
        _splineDrawerThrowing.StartDrawing();
    }
    private void holdDrawing()
    {
        _splineDrawerDisplay.HoldDrawing();
        _splineDrawerThrowing.HoldDrawing();
    }

    private void endDrawing()
    {
        _splineDrawerDisplay.Clear();
        _splineDrawerThrowing.EndDrawing();
    }

    // Throwing
    private void idleThrowing()
    {
        _moveToSpline.ResetPosition();

        _actionType = ActionType.Idle;
    }

    private void prepareThrowing()
    {
        startDrawing();

        _actionType = ActionType.Prepare;
    }

    private void startThrowing()
    {
        _moveToSpline.StartMovement(_splineContainerThrowing.Spline);

        _actionType = ActionType.Throw;

        Invoke(nameof(observationThrowing), 0.5f);
    }

    private void observationThrowing()
    {
        _actionType = ActionType.Observation;

        Invoke(nameof(idleThrowing), 2.0f);
    }
}
