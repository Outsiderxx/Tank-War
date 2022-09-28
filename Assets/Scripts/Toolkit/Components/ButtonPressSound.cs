using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPressSound : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool playOnMouseUp = true;
    public bool playOnMouseDown = false;
    public string clipName;



    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.playOnMouseDown)
        {
            AudioManager.Instance.PlayEffect(this.clipName);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (this.playOnMouseUp)
        {
            AudioManager.Instance.PlayEffect(this.clipName);
        }
    }
}
