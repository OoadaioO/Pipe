using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [Header("music settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;

    [Header("SFX Settings")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip gameOverClip;


    public void FadeIn() {
        StopAllCoroutines();
        StartCoroutine(FadeInCoroutine());
    }

    public void FadeOut() {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeInCoroutine() {
        float time = 0f;
        float startVolume = audioSource.volume;
        audioSource.volume = 0f;
        audioSource.Play();
        while (time < fadeInDuration) {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 1f, time / fadeInDuration);
            yield return null;
        }
        audioSource.volume = 1f;
    }

    private IEnumerator FadeOutCoroutine() {
        float time = 0f;
        float startVolume = audioSource.volume;
        while (time < fadeOutDuration) {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeOutDuration);
            yield return null;
        }
        audioSource.Stop();
    }


    public void PlayGameOverOneShot(){
        sfxSource.PlayOneShot(gameOverClip);
    }
}
