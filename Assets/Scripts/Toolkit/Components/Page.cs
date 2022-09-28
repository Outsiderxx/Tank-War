using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 基礎頁面
/// </summary>
public class Page : MonoBehaviour
{
    [SerializeField] private Button blockInputEvent;
    [SerializeField] protected RectTransform content;
    /// <summary>
    /// 當頁面失去focus的時候是否關閉頁面
    /// </summary>
    [SerializeField] private bool closeOnBlur;

    public System.Action OnOpen;
    public System.Action OnClose;

    /// <summary>
    /// 當前頁面是否處於開啟狀態
    /// </summary>
    public bool IsOpen
    {
        get
        {
            return content.gameObject.activeInHierarchy;
        }
    }

    protected virtual void Awake()
    {
        blockInputEvent?.onClick.AddListener(() =>
        {
            if (closeOnBlur)
            {
                Close();
            }
        });
    }

    /// <summary>
    /// 開啟頁面
    /// </summary>
    public virtual void Open()
    {
        if (IsOpen)
        {
            return;
        }
        blockInputEvent?.gameObject.SetActive(true);
        content.gameObject.SetActive(true);
        this.OnOpen?.Invoke();
    }

    /// <summary>
    /// 關閉頁面
    /// </summary>
    public virtual void Close()
    {
        if (!IsOpen)
        {
            return;
        }
        blockInputEvent?.gameObject.SetActive(false);
        content.gameObject.SetActive(false);
        this.OnClose?.Invoke();
    }
}
