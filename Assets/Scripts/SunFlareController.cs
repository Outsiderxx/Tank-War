using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunFlareController : MonoBehaviour
{
    [SerializeField] private Flare sunFlare;

    private Light sun;

    private void Awake()
    {
        this.sun = this.GetComponent<Light>();
        SkyBoxController.instance.onTimeStateChanged += this.OnTimeChanged;
    }

    void Start()
    {
        this.OnTimeChanged();
    }

    private void OnTimeChanged()
    {
        this.sun.flare = SkyBoxController.instance.isNight ? null : this.sunFlare;
    }
}
