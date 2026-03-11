using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Tetap hidup antar scene
    }

    // ─────────────────────────────────────────────
    //  INSPECTOR FIELDS
    // ─────────────────────────────────────────────

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource stonemanSource; // Khusus suara Stoneman agar bisa dipause
    [SerializeField] private AudioSource playerSource; // Khusus suara player agar bisa dipause

    [Header("BGM Clips")]
    public AudioClip bgmMainMenu;
    public AudioClip bgmGameplay;
    public AudioClip bgmChase;

    [Header("Player Clips")]
    public AudioClip sfxFootStep;
    public AudioClip sfxExhausted;

    [Header("Enemy Clips")]
    public AudioClip sfxStonemanStep;
    public AudioClip sfxStun;

    [Header("Interact Clips")]
    public AudioClip sfxDoorOpen;
    public AudioClip sfxBarrel;
    public AudioClip sfxLemari;
    public AudioClip sfxSave;

    [Header("UI Clips")]
    public AudioClip buttonClick;

    [Header("Settings")]
    [SerializeField] private float bgmFadeDuration = 1f;

    // ─────────────────────────────────────────────
    //  BGM
    // ─────────────────────────────────────────────
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        StopAllCoroutines();
        StartCoroutine(FadeBGM(clip));
    }

    public void StopBGM() => StartCoroutine(FadeOut(bgmSource, bgmFadeDuration));

    IEnumerator FadeBGM(AudioClip newClip)
    {
        // Fade out BGM lama
        if (bgmSource.isPlaying)
            yield return StartCoroutine(FadeOut(bgmSource, bgmFadeDuration));

        // Ganti clip dan fade in
        bgmSource.clip = newClip;
        bgmSource.loop = true;
        bgmSource.Play();
        yield return StartCoroutine(FadeIn(bgmSource, bgmFadeDuration));
    }

    IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume; // Reset volume untuk pemakaian berikutnya
    }

    IEnumerator FadeIn(AudioSource source, float duration)
    {
        float targetVolume = source.volume;
        source.volume = 0f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, targetVolume, timer / duration);
            yield return null;
        }
    }

    // ─────────────────────────────────────────────
    // Play BGM
    // ─────────────────────────────────────────────
    public void PlayMainMenuBGM() => PlayBGM(bgmMainMenu);
    public void PlayGameplayBGM() => PlayBGM(bgmGameplay);
    public void PlayChaseBGM() => PlayBGM(bgmChase);

    // ─────────────────────────────────────────────
    //  SFX
    // ─────────────────────────────────────────────

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    // ─────────────────────────────────────────────
    // INTERACT & UI SFX
    // ─────────────────────────────────────────────
    public void PlayDoorOpen() => PlaySFX(sfxDoorOpen);
    public void PlayBarrel() => PlaySFX(sfxBarrel);
    public void PlayLemari() => PlaySFX(sfxLemari);
    public void PlaySave() => PlaySFX(sfxSave);

    // ─────────────────────────────────────────────
    //  SUARA STONEMAN
    // ─────────────────────────────────────────────
    public void PlayStonemanStep()
    {
        if (stonemanSource.isPlaying) return;
        stonemanSource.clip = sfxStonemanStep;
        stonemanSource.loop = true;
        stonemanSource.Play();
    }
    public void StopStoneman() => stonemanSource.Stop();
    public void PlayStun() => stonemanSource.PlayOneShot(sfxStun);

    // ─────────────────────────────────────────────
    //  PLAYER SFX
    // ─────────────────────────────────────────────
    public void PlayFootstep(AudioClip clip)
    {
        if (playerSource.isPlaying) return;
        playerSource.clip = clip;
        playerSource.loop = true;
        playerSource.Play();
    }
    public void StopFootstep()
    {
        playerSource.loop = false;
        playerSource.Stop();
    }
    public void PlayExhausted() => playerSource.PlayOneShot(sfxExhausted);


    // ─────────────────────────────────────────────
    //  VOLUME CONTROL (untuk settings UI nanti)
    // ─────────────────────────────────────────────

    public void SetBGMVolume(float volume) => bgmSource.volume = volume;
    public void SetSFXVolume(float volume) => sfxSource.volume = volume;
}