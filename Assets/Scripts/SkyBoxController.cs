using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxController : MonoBehaviour
{
    public int timeInterval;
    public float rotationSpeed, timeChangeSpeed, nightValue, dayValue;
    public bool isNight;

    private Material skyBox;
    private float currentValue;
    private int jobID;

    private void Awake()
    {
        this.skyBox = RenderSettings.skybox;
        this.currentValue = this.skyBox.GetColor("_Tint").r;
        this.jobID = Scheduler.Schedule(() =>
        {
            this.isNight = !this.isNight;
        }, this.timeInterval);
    }

    private void OnDestroy()
    {
        Scheduler.Unschedule(this.jobID);
    }

    // Update is called once per frame
    void Update()
    {
        // rotate
        this.skyBox.SetFloat("_Rotation", this.skyBox.GetFloat("_Rotation") + (Time.deltaTime * this.rotationSpeed));

        // time pass
        if (this.isNight && this.currentValue > this.nightValue)
        {
            this.currentValue = Mathf.Clamp(this.currentValue - Time.deltaTime * this.timeChangeSpeed, this.nightValue, this.dayValue);
            RenderSettings.ambientIntensity = (this.currentValue - this.nightValue) / (this.dayValue - this.nightValue);
            this.skyBox.SetColor("_Tint", new Color(this.currentValue, this.currentValue, this.currentValue, 1));

        }
        else if (!this.isNight && this.currentValue < this.dayValue)
        {
            this.currentValue = Mathf.Clamp(this.currentValue + Time.deltaTime * this.timeChangeSpeed, this.nightValue, this.dayValue);
            RenderSettings.ambientIntensity = (this.currentValue - this.nightValue) / (this.dayValue - this.nightValue);
            this.skyBox.SetColor("_Tint", new Color(this.currentValue, this.currentValue, this.currentValue, 1));
        }

        print(RenderSettings.ambientIntensity);
    }
}
