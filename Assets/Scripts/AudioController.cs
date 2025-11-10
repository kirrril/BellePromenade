using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    [SerializeField]
    private AudioMixer mixer;

    public bool isAccelerating;
    public static bool musicIsOn;
    private float accelValue = 0f;

    void Update()
    {
        float target = isAccelerating ? 1f : 0f;

        accelValue = Mathf.MoveTowards(accelValue, target, Time.deltaTime * 2f);

        mixer.SetFloat("motorPitch", Mathf.Lerp(1f, 2f, accelValue));
        mixer.SetFloat("motorVolume", Mathf.Lerp(0f, 2f, accelValue));
        mixer.SetFloat("motorDistortion", Mathf.Lerp(0.5f, 0.9f, accelValue));

        float value = musicIsOn ? 0f : value = -80f;
        mixer.SetFloat("musicVolume", value);
    }
}
