using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineBasicMultiChannelPerlin _perlin;

    private void Start()
    {
        _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        _perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void DoCameraShake(float strength)
    {
        _perlin.m_AmplitudeGain = strength;
        _perlin.m_FrequencyGain = 1f;
    }

    public void StopCameraShake()
    {
        _perlin.m_AmplitudeGain = 0f;
        _perlin.m_FrequencyGain = 0f;
    }
}
