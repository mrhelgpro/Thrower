using UnityEngine.Splines;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private enum ActionType { Menu, Idle, Prepare, Throw, Observation, Result }
    private ActionType _actionType = ActionType.Menu;

    [SerializeField] private Animator animator;
    [SerializeField] private SplineRenderer splineRendererDisplay;
    [SerializeField] private SplineDrawer splineDrawerDisplay;
    [SerializeField] private SplineDrawer splineDrawerThrowing;
    [SerializeField] private SplineContainer splineContainerThrowing;
    [SerializeField] private Card card;
    [SerializeField] private GameObject warpEffect;

    private void Update()
    {
        if (_actionType == ActionType.Idle)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PrepareThrowing();
            }
        }
        else if (_actionType == ActionType.Prepare)
        {
            HoldDrawing();

            if (Input.GetMouseButtonUp(0))
            {
                EndDrawing();

                if (splineContainerThrowing.Spline.Count > 1)
                {
                    StartThrowing();
                }
                else
                {
                    IdleThrowing();
                }
            }
        }
        else if (_actionType == ActionType.Observation)
        {
            if (card.IsFinal)
            {
                ResultThrowing();
            }
        }
    }

    // Gameplay
    public void MenuGame()
    {
        CameraManager.Instance.SetMenuCamera(0.0f);
        _actionType = ActionType.Menu;
        
        animator.Play("Menu");
    }

    public void StartGame()
    {
        Invoke(nameof(IdleThrowing), 0.25f);
        animator.Play("Turn");
    }
    
    public void ResultGame()
    {
        CameraManager.Instance.SetMenuCamera();
        _actionType = ActionType.Menu;
        GameManager.Instance.ResultGame();
    }
    
    // Throwing
    private bool IsCorrectLine => splineContainerThrowing.Spline.Count < 4;

    private void IdleThrowing()
    {
        CameraManager.Instance.SetStartCamera();

        warpEffect.SetActive(false);
        card.ResetPosition();
        animator.CrossFade("Idle", 0.25f);
        _actionType = ActionType.Idle;
    }
    
    private void PrepareThrowing()
    {
        StartDrawing();
        
        animator.CrossFade("Prepare", 0.25f);
        _actionType = ActionType.Prepare;
    }

    private void StartThrowing()
    {
        Invoke(nameof(ObservationThrowing), 0.25f);
        
        animator.CrossFade("Throw", 0.025f);
        _actionType = ActionType.Throw;
    }

    private void ObservationThrowing()
    {
        CameraManager.Instance.SetThrowCamera();
        
        warpEffect.SetActive(true);
        card.StartMovement(splineContainerThrowing.Spline);
        _actionType = ActionType.Observation;
    }
    
    private void ResultThrowing()
    {
        CameraManager.Instance.SetFinalCamera(1.5f);
        Invoke(nameof(ResultGame), 0.25f);
        
        _actionType = ActionType.Result;
        animator.CrossFade("Idle", 0.5f);
        warpEffect.SetActive(false);
    }
    
    // Drawing
    private void StartDrawing()
    {
        card.ResetPosition();
        
        splineRendererDisplay.RendererSettings.StartColor = new Color32(90, 160, 240,0);
        splineRendererDisplay.RendererSettings.EndColor = new Color32(90, 160, 240,255);
        splineDrawerDisplay.StartDrawing();
        splineDrawerThrowing.StartDrawing();
    }
    private void HoldDrawing()
    {
        if (IsCorrectLine)
        {
            splineDrawerDisplay.HoldDrawing();
            splineDrawerThrowing.HoldDrawing();
            
            return;
        }

        splineRendererDisplay.RendererSettings.StartColor = new Color32(225, 90, 75,0);
        splineRendererDisplay.RendererSettings.EndColor = new Color32(225, 90, 75,255);
        splineDrawerThrowing.Clear();
    }

    private void EndDrawing()
    {
        splineDrawerDisplay.Clear();
        splineDrawerThrowing.EndDrawing();
    }
}