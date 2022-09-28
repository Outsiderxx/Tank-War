using UnityEngine;

/// <summary>
/// 音量單位轉換器，線性單位: 0~1，對數單位: dB
/// </summary>
public static class VolumeConverter
{
    /// <summary>
    /// 將音量轉換為對數(dB)單位
    /// </summary>
    /// <param name="linearVolume">以線性(0~1)單位表示的音量</param>
    /// <returns>以對數(dB)單位表示的音量</returns>
    public static float ConvertToLogarithmicVolume(float linearVolume)
    {
        // 無法從0直接轉換為dB值，因為log10(0)==-Infinity
        if (linearVolume <= 0)
        {
            return Mathf.Log10(0.0001f) * 20;
        }
        return Mathf.Log10(linearVolume) * 20;
    }

    /// <summary>
    /// 將音量轉換為線性(0~1)單位
    /// </summary>
    /// <param name="logarithmicVolume">以對數(dB)單位表示的音量</param>
    /// <returns>以線性(0~1)單位表示的音量</returns>
    public static float ConvertToLinearVolume(float logarithmicVolume)
    {
        return logarithmicVolume <= -80 ? 0 : Mathf.Pow(10, logarithmicVolume / 20);
    }
}
