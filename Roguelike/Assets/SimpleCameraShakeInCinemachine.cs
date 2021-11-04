using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class SimpleCameraShakeInCinemachine : MonoBehaviour {

    private static float ShakeDuration = 0.3f;          // Time the Camera Shake effect will last
    private static float ShakeAmplitude = 1.2f;         // Cinemachine Noise Profile Parameter
    private static float ShakeFrequency = 2.0f;         // Cinemachine Noise Profile Parameter

    private static float ShakeElapsedTime = 0f;

    private static float FreezeDuration = 0f;

    // local variable for keeping track of personal initiation of freeze-frame
    private static bool frozen = false;

    // Cinemachine Shake
    public CinemachineVirtualCamera VirtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    // Use this for initialization
    void Start()
    {
        VirtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        // Get Virtual Camera Noise Profile
        if (VirtualCamera != null)
            virtualCameraNoise = VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the Cinemachine componet is not set, avoid update
        if (VirtualCamera != null && virtualCameraNoise != null)
        {
            // If Camera Shake effect is still playing
            if (ShakeElapsedTime > 0)
            {
                // Set Cinemachine Camera Noise parameters
                virtualCameraNoise.m_AmplitudeGain = ShakeAmplitude;
                virtualCameraNoise.m_FrequencyGain = ShakeFrequency;

                // Update Shake Timer
                ShakeElapsedTime -= Time.deltaTime;
            }
            else
            {
                // If Camera Shake effect is over, reset variables
                virtualCameraNoise.m_AmplitudeGain = 0f;
                ShakeElapsedTime = 0f;
            }
        }

        if(FreezeDuration <= 0 && frozen) {
            Time.timeScale = 1f;
            frozen = false;
        }
        FreezeDuration -= Time.fixedDeltaTime;
    }

    public static void Shake() {
        ShakeAmplitude = 0.7f;
        ShakeFrequency = 1.0f;

        ShakeElapsedTime = 0.1f;
    }

    public static void Shake(float duration, float amplitude, float frequency) {
        ShakeAmplitude = amplitude;
        ShakeFrequency = frequency;

        ShakeElapsedTime = duration;
    }

    public static void Freeze(float duration) {
        FreezeDuration = duration;
        frozen = true;
    }
}
