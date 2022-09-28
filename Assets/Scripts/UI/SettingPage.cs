using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Page))]
public class SettingPage : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectSlider;

    private Page page;

    private void Awake()
    {
        page = this.GetComponent<Page>();
        page.OnOpen += () =>
        {
            this.musicSlider.value = AudioManager.Instance.MusicVolume;
            this.effectSlider.value = AudioManager.Instance.EffectVolume;
        };
        musicSlider.onValueChanged.AddListener((float value) =>
        {
            AudioManager.Instance.MusicVolume = value;
        });
        effectSlider.onValueChanged.AddListener((float value) =>
        {
            AudioManager.Instance.EffectVolume = value;
        });
    }
}