using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource soundFXObject;

    public static SoundFXManager soundFXManager { get; private set; }

    private void Awake()
    {
        if (soundFXManager != null && soundFXManager != this)
            Destroy(this);
        else
            soundFXManager = this;
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(
            soundFXObject,
            spawnTransform.position,
            Quaternion.identity
        );

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }
}
