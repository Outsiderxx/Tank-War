using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DIsplayPositionOnMiniMap : MonoBehaviour
{
    [SerializeField] private Transform icon;
    [SerializeField] private Camera miniMapCamera;

    private Tank tank;
    private Image iconImage;

    private void Awake()
    {
        this.tank = this.GetComponent<Tank>();
        this.iconImage = this.icon.GetComponent<Image>();
        this.tank.onStateChanged += () =>
        {
            Color newColor = new Color(this.iconImage.color.r, this.iconImage.color.g, this.iconImage.color.b);
            newColor.a = this.tank.isAlive ? 1 : (this.tank.state == TankState.Dead ? 0 : 0.5f);
            this.iconImage.color = newColor;
        };
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 positionInViewport = this.miniMapCamera.WorldToViewportPoint(this.transform.position);
        this.icon.GetComponent<RectTransform>().anchoredPosition = new Vector3(-100 + positionInViewport.x * 200, -100 + positionInViewport.y * 200, 0);
    }
}
