using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    private Transform tank;
    private float horizontalAngle = 0;
    private float verticalAngle = 0;
    private float lastEulerAngleY = 0;

    private void Awake()
    {
        this.tank = this.transform.parent;
    }

    private void Start()
    {
        this.lastEulerAngleY = this.tank.eulerAngles.y;
    }

    private void Update()
    {
        this.horizontalAngle = (horizontalAngle + Input.GetAxis("Mouse X") - (this.tank.eulerAngles.y - this.lastEulerAngleY)) % 360;
        this.lastEulerAngleY = this.tank.eulerAngles.y;
        this.verticalAngle = Mathf.Clamp((this.verticalAngle - Input.GetAxis("Mouse Y")), -45, 0);

        Vector3 localEulerAngles = this.transform.localEulerAngles;
        localEulerAngles.y = this.horizontalAngle;
        this.transform.localEulerAngles = localEulerAngles;
    }
}
