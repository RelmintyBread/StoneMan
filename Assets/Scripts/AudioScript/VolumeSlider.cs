using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public enum VolumeTarget
    {
        BGM,
        SFX
    }

    [SerializeField] private VolumeTarget target = VolumeTarget.BGM;
    [SerializeField] private Slider slider;

    void Awake()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
    }

    void Start()
    {
        if (slider == null) return;
        if (AudioManager.Instance == null) return;

        switch (target)
        {
            case VolumeTarget.BGM:
                slider.value = AudioManager.Instance.GetBGMVolume();
                break;
            case VolumeTarget.SFX:
                slider.value = AudioManager.Instance.GetSFXVolume();
                break;
        }

        slider.onValueChanged.AddListener(HandleValueChanged);
    }

    void OnDestroy()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(HandleValueChanged);
        }
    }

    private void HandleValueChanged(float value)
    {
        if (AudioManager.Instance == null) return;

        switch (target)
        {
            case VolumeTarget.BGM:
                AudioManager.Instance.SetBGMVolume(value);
                break;
            case VolumeTarget.SFX:
                AudioManager.Instance.SetSFXVolume(value);
                break;
        }
    }

}
