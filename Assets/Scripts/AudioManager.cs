using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 音量管理員，處理裝置音量並與game client同步
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer = null;

    public float MusicVolume
    {
        get
        {
            return audioMixer.GetMusicVolume();
        }
        set
        {
            audioMixer.SetMusicVolume(value);
        }
    }

    public float EffectVolume
    {
        get
        {
            return audioMixer.GetEffectVolume();
        }
        set
        {
            audioMixer.SetEffectVolume(value);
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
