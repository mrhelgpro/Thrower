using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    
    [SerializeField] private CinemachineBrain mainCameraBrain;
    [SerializeField] private CinemachineVirtualCamera menuCamera;
    [SerializeField] private CinemachineVirtualCamera startCamera;
    [SerializeField] private CinemachineVirtualCamera throwCamera;
    [SerializeField] private CinemachineVirtualCamera finalCamera;

    private void Awake()
    {
        Instance = GetComponent<CameraManager>();
    }

    public void SetMenuCamera(float fade = 1.0f) => SetCamera(menuCamera, fade);
    public void SetStartCamera(float fade = 1.0f) => SetCamera(startCamera, fade);
    public void SetThrowCamera(float fade = 1.0f) => SetCamera(throwCamera, fade);
    public void SetFinalCamera(float fade = 1.0f) => SetCamera(finalCamera, fade);

    private void SetCamera(CinemachineVirtualCamera currentCamera, float fade = 1.0f)
    {
        startCamera.Priority = 0;
        throwCamera.Priority = 0;

        currentCamera.Priority = 1;
        mainCameraBrain.m_DefaultBlend.m_Time = fade;
    }
}
