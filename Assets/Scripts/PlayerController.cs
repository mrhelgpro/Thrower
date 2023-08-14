using UnityEngine.Splines;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum ActionType { Menu, Idle, Prepare, Throw, Observation, Result }
    private ActionType _actionType = ActionType.Idle;

    [SerializeField] private Animator _animator;
    [SerializeField] private SplineDrawer _splineDrawerDisplay;
    [SerializeField] private SplineDrawer _splineDrawerThrowing;
    [SerializeField] private SplineContainer _splineContainerThrowing;
    [SerializeField] private Card _card;

    private void Update()
    {
        if (_actionType == ActionType.Idle)
        {
            if (Input.GetMouseButtonDown(0))
            {
                prepareThrowing();
            }
        }
        else if (_actionType == ActionType.Prepare)
        {
            holdDrawing();

            if (Input.GetMouseButtonUp(0))
            {
                endDrawing();

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
        else if (_actionType == ActionType.Observation)
        {
            if (_card.IsFinal == true)
            {
                resultThrowing();
            }
        }
    }

    // Drawing
    private void startDrawing()
    {
        _card.ResetPosition();
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
        _card.ResetPosition();

        _actionType = ActionType.Idle;
    }

    private void prepareThrowing()
    {
        startDrawing();

        _actionType = ActionType.Prepare;
    }

    private void startThrowing()
    {
        _card.StartMovement(_splineContainerThrowing.Spline);

        _actionType = ActionType.Throw;

        Invoke(nameof(observationThrowing), 0.5f);
    }

    private void observationThrowing()
    {
        _actionType = ActionType.Observation;
    }

    private void resultThrowing()
    {
        _actionType = ActionType.Result;

        Invoke(nameof(idleThrowing), 2.0f);
    }
}
