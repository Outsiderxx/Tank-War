using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局排程器
/// </summary>
public class Scheduler : MonoBehaviour
{
    private static readonly List<Coroutine> jobList = new List<Coroutine>();
    private static Scheduler instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 設定一個每隔固定時間執行的工作
    /// </summary>
    /// <param name="callback">工作</param>
    /// <param name="interval">工作間隔(in seconds)</param>
    /// <param name="times">執行次數(-1代表不限次數)</param>
    /// <returns>工作ID</returns>
    public static int Schedule(Action callback, float interval, int times = -1)
    {
        if (times <= 0 && times != -1)
        {
            Debug.LogWarning("Invalid schedule times");
            return -1;
        }
        int jobIndex = jobList.Count;
        IEnumerator job = instance.RunTask(callback, interval, times);
        jobList.Add(instance.StartCoroutine(job));
        return jobIndex;
    }

    /// <summary>
    /// 取消設定一個每隔固定時間執行的工作
    /// </summary>
    /// <param name="jobIndex">工作ID</param>
    public static void Unschedule(int jobIndex)
    {
        if (jobIndex < 0 || jobIndex >= jobList.Count)
        {
            Debug.LogWarning("Invalid job index");
            return;
        }
        instance.StopCoroutine(jobList[jobIndex]);
    }

    /// <summary>
    /// 設定一個指定時間過後執行的工作
    /// </summary>
    /// <param name="callback">工作</param>
    /// <param name="interval">指定時間</param>
    /// <returns>工作ID</returns>
    public static int ScheduleOnce(Action callback, float interval)
    {
        int jobIndex = jobList.Count;
        IEnumerator job = instance.RunTask(callback, interval, 1);
        jobList.Add(instance.StartCoroutine(job));
        return jobIndex;
    }

    private IEnumerator RunTask(Action callback, float interval, int times)
    {
        while (times == -1 || times > 0)
        {
            yield return new WaitForSeconds(interval);
            callback();
            if (times > 0)
            {
                times--;
            }
        }
    }
}
