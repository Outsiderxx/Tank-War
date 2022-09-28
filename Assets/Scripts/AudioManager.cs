using UnityEngine;
using UnityEngine.Audio;
using System.Linq;

public static class AudioClipName
{
    public static readonly string BUTTON_PRESS_01 = "";
    public static readonly string BUTTON_PRESS_02 = "";
}

/// <summary>
/// 音量管理員，處理裝置音量並與game client同步
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer = null;
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private AudioMixerGroup effectGroup;

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

    private AudioSource[] audioSources
    {
        get
        {
            return this.GetComponentsInChildren<AudioSource>();
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

    public void PlayEffect(string clipName)
    {
        AudioSource audioSource = this.audioSources.FirstOrDefault(audioSource => !audioSource.isPlaying) ?? this.SpawnAudioSource();
        AudioClip clip = this.clips.FirstOrDefault((clip) => clip.name == clipName);
        if (clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            throw new System.Exception($"Can't find clip {clipName}");
        }

    }

    public void Play3DEffect(string clipName, Vector3 position, float volume = 1)
    {
        AudioClip clip = this.clips.FirstOrDefault((clip) => clip.name == clipName);
        if (clip)
        {
            AudioSource audioSource = this.SpawnAudioSource();
            audioSource.transform.parent = null;
            audioSource.transform.position = position;
            audioSource.spatialBlend = 0.95f;
            audioSource.outputAudioMixerGroup = this.effectGroup;
            audioSource.volume = volume;
            audioSource.clip = clip;
            audioSource.Play();
            Scheduler.ScheduleOnce(() => Destroy(audioSource.gameObject), clip.length);
        }
        else
        {
            throw new System.Exception($"Can't find clip {clipName}");
        }
    }

    private AudioSource SpawnAudioSource()
    {
        GameObject child = new GameObject("AudioSource");
        AudioSource audioSource = child.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.outputAudioMixerGroup = this.effectGroup;
        child.transform.parent = this.transform;
        return audioSource;
    }
}
