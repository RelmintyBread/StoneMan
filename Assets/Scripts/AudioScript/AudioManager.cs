using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance { get; private set; }
    [Header("Debug")]
    [SerializeField] private bool logAudioEvents = true;

    void Awake()
    {
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }

    Instance = this;
    DontDestroyOnLoad(gameObject);

    EnsureSources();
    ValidateAudioSetup();
    }

    void EnsureSources()
{
    if (bgmSource == null) bgmSource = gameObject.AddComponent<AudioSource>();
    if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
    if (stonemanSource == null) stonemanSource = gameObject.AddComponent<AudioSource>();
    if (playerSource == null) playerSource = gameObject.AddComponent<AudioSource>();
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
    public AudioClip sfxHoldLoop;
    public AudioClip sfxHoldComplete;
    public AudioClip sfxFlashlightClick;

    [Header("Settings")]
    [SerializeField] private float bgmFadeDuration = 1f;
    [SerializeField] private AudioSource holdLoopSource;

    // ─────────────────────────────────────────────
    //  BGM
    // ─────────────────────────────────────────────
    public void PlayBGM(AudioClip clip)
    {
        if (logAudioEvents) Debug.Log($"[AudioManager] PlayBGM: {ClipName(clip)}");
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
        if (logAudioEvents) Debug.Log($"[AudioManager] PlaySFX: {ClipName(clip)}");
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
    public void PlayButtonClick() => PlaySFX(buttonClick);
    public void PlayHoldLoop()
    {
        if (logAudioEvents) Debug.Log($"[AudioManager] PlayHoldLoop: {ClipName(sfxHoldLoop)}");
        if (sfxHoldLoop == null || holdLoopSource == null) return;
        if (holdLoopSource.isPlaying) return;
        holdLoopSource.clip = sfxHoldLoop;
        holdLoopSource.loop = true;
        holdLoopSource.Play();
    }

    public void StopHoldLoop()
    {
        if (holdLoopSource == null) return;
        holdLoopSource.loop = false;
        holdLoopSource.Stop();
    }

    public void PlayHoldComplete() => PlaySFX(sfxHoldComplete);
    public void PlayFlashlightClick() => PlaySFX(sfxFlashlightClick);

    // ─────────────────────────────────────────────
    //  SUARA STONEMAN
    // ─────────────────────────────────────────────
    public void PlayStonemanStep()
    {
        if (logAudioEvents) Debug.Log($"[AudioManager] PlayStonemanStep: {ClipName(sfxStonemanStep)}");
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
        if (logAudioEvents) Debug.Log($"[AudioManager] PlayFootstep: {ClipName(clip)}");
        if (playerSource.isPlaying) return;
        playerSource.clip = clip;
        playerSource.loop = true;
        playerSource.Play();
    }
    public void StopFootstep()
    {
    if (playerSource != null && playerSource.isPlaying)
        {
        playerSource.loop = false;
        playerSource.Stop();
        }
    }
    public void PlayExhausted() => playerSource.PlayOneShot(sfxExhausted);


    // ─────────────────────────────────────────────
    //  VOLUME CONTROL (untuk settings UI nanti)
    // ─────────────────────────────────────────────

    public void SetBGMVolume(float volume) => bgmSource.volume = volume;
    public void SetSFXVolume(float volume) => sfxSource.volume = volume;
    public float GetBGMVolume() => bgmSource != null ? bgmSource.volume : 1f;
    public float GetSFXVolume() => sfxSource != null ? sfxSource.volume : 1f;

    private static string ClipName(AudioClip clip)
    {
        return clip == null ? "null" : clip.name;
    }

    private void ValidateAudioSetup()
    {
        if (bgmSource == null) Debug.LogWarning("[AudioManager] bgmSource belum di-assign.");
        if (sfxSource == null) Debug.LogWarning("[AudioManager] sfxSource belum di-assign.");
        if (stonemanSource == null) Debug.LogWarning("[AudioManager] stonemanSource belum di-assign.");
        if (playerSource == null) Debug.LogWarning("[AudioManager] playerSource belum di-assign.");

        if (bgmMainMenu == null) Debug.LogWarning("[AudioManager] bgmMainMenu belum di-assign.");
        if (bgmGameplay == null) Debug.LogWarning("[AudioManager] bgmGameplay belum di-assign.");
        if (bgmChase == null) Debug.LogWarning("[AudioManager] bgmChase belum di-assign.");

        if (sfxFootStep == null) Debug.LogWarning("[AudioManager] sfxFootStep belum di-assign.");
        if (sfxExhausted == null) Debug.LogWarning("[AudioManager] sfxExhausted belum di-assign.");
        if (sfxStonemanStep == null) Debug.LogWarning("[AudioManager] sfxStonemanStep belum di-assign.");
        if (sfxStun == null) Debug.LogWarning("[AudioManager] sfxStun belum di-assign.");

        if (sfxDoorOpen == null) Debug.LogWarning("[AudioManager] sfxDoorOpen belum di-assign.");
        if (sfxBarrel == null) Debug.LogWarning("[AudioManager] sfxBarrel belum di-assign.");
        if (sfxLemari == null) Debug.LogWarning("[AudioManager] sfxLemari belum di-assign.");
        if (sfxSave == null) Debug.LogWarning("[AudioManager] sfxSave belum di-assign.");
        if (buttonClick == null) Debug.LogWarning("[AudioManager] buttonClick belum di-assign.");
        if (sfxHoldLoop == null) Debug.LogWarning("[AudioManager] sfxHoldLoop belum di-assign.");
        if (sfxHoldComplete == null) Debug.LogWarning("[AudioManager] sfxHoldComplete belum di-assign.");
        if (sfxFlashlightClick == null) Debug.LogWarning("[AudioManager] sfxFlashlightClick belum di-assign.");

        if (holdLoopSource == null) Debug.LogWarning("[AudioManager] holdLoopSource belum di-assign.");

        if (AudioListener.pause)
        {
            Debug.LogWarning("[AudioManager] AudioListener sedang pause.");
        }
    }
}
