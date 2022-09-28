using System;
using UnityEngine.Audio;

public static class AudioMixerExtensionMethod
{
    /// <summary>
    /// 取得音樂音量
    /// </summary>
    /// <param name="audioMixer">This</param>
    public static float GetMusicVolume(this AudioMixer audioMixer)
    {
        if (!audioMixer.GetFloat("MusicVolume", out float volumeInDB))
        {
            throw new Exception("AudioMixer parameter MusicVolume isn't exposed");
        }
        return VolumeConverter.ConvertToLinearVolume(volumeInDB);
    }

    /// <summary>
    /// 設置音樂音量
    /// </summary>
    /// <param name="audioMixer">This</param>
    /// <param name="volume">音量</param>
    public static void SetMusicVolume(this AudioMixer audioMixer, float volume)
    {
        float volumeIndB = VolumeConverter.ConvertToLogarithmicVolume(volume);
        if (!audioMixer.SetFloat("MusicVolume", volumeIndB))
        {
            throw new Exception("AudioMixer parameter MusicVolume isn't exposed");
        }
    }

    /// <summary>
    /// 取得音效音量
    /// </summary>
    /// <param name="audioMixer">This</param>
    public static float GetEffectVolume(this AudioMixer audioMixer)
    {
        if (!audioMixer.GetFloat("EffectVolume", out float volumeInDB))
        {
            throw new Exception("AudioMixer parameter EffectVolume isn't exposed");
        }
        return VolumeConverter.ConvertToLinearVolume(volumeInDB);
    }

    /// <summary>
    /// 設置音效音量
    /// </summary>
    /// <param name="audioMixer">This</param>
    /// <param name="volume">音量</param>
    public static void SetEffectVolume(this AudioMixer audioMixer, float volume)
    {
        float volumeIndB = VolumeConverter.ConvertToLogarithmicVolume(volume);
        if (!audioMixer.SetFloat("EffectVolume", volumeIndB))
        {
            throw new Exception("AudioMixer parameter EffectVolume isn't exposed");
        }
    }

}
