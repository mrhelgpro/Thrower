using UnityEngine.Splines;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    enum ActionType { Menu, Idle, Prepare, Throw, Observation, Result }
    private ActionType _actionType = ActionType.Idle;

    [SerializeField] private Animator _animator;
    [SerializeField] private CinemachineBrain _mainCameraBrain;
    [SerializeField] private CinemachineVirtualCamera _startCamera;
    [SerializeField] private CinemachineVirtualCamera _throwCamera;
    [SerializeField] private CinemachineVirtualCamera _finalCamera;
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
        setCamera(_startCamera, 0.0f);

        _card.ResetPosition();
        _animator.CrossFade("Idle", 0.25f);

        _actionType = ActionType.Idle;
    }

    private void prepareThrowing()
    {
        startDrawing();
        _animator.CrossFade("Prepare", 0.25f);

        _actionType = ActionType.Prepare;
    }

    private void startThrowing()
    {
        _animator.CrossFade("Throw", 0.025f);

        _actionType = ActionType.Throw;

        Invoke(nameof(observationThrowing), 0.25f);
    }

    private void observationThrowing()
    {
        setCamera(_throwCamera, 1.0f);
        _card.StartMovement(_splineContainerThrowing.Spline);
        _actionType = ActionType.Observation;
    }

    private void resultThrowing()
    {
        _actionType = ActionType.Result;
        _animator.CrossFade("Idle", 0.5f);
        setCamera(_finalCamera, 1.5f);
        
        Invoke(nameof(idleThrowing), 2.0f);
    }

    private void setCamera(CinemachineVirtualCamera currentCamera, float fade = 1.0f)
    {
        _startCamera.Priority = 0;
        _throwCamera.Priority = 0;

        currentCamera.Priority = 1;
        _mainCameraBrain.m_DefaultBlend.m_Time = fade;
    }
}