using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public float rotateSensitivity = 0.5f;
    private Transform tank;
    private Transform gun;
    private float horizontalAngle = 0;
    private float verticalAngle = 0;
    private float lastEulerAngleY = 0;

    private void Awake()
    {
        this.tank = this.transform.parent;
        this.gun = this.transform.Find("Gun");
    }

    private void Start()
    {
        this.lastEulerAngleY = this.tank.eulerAngles.y;
    }

    private void Update()
    {
        // direction of turret do not affect by the direction of tank
        this.horizontalAngle = (this.horizontalAngle + Input.GetAxis("Mouse X") * this.rotateSensitivity - (this.tank.eulerAngles.y - this.lastEulerAngleY)) % 360;
        this.lastEulerAngleY = this.tank.eulerAngles.y;

        Vector3 localEulerAngles = this.transform.localEulerAngles;
        localEulerAngles.y = this.horizontalAngle;
        this.transform.localEulerAngles = localEulerAngles;

        this.verticalAngle = Mathf.Clamp((this.verticalAngle - Input.GetAxis("Mouse Y")), -30, 0);
        this.gun.localRotation = Quaternion.AngleAxis(this.verticalAngle, Vector3.right);
    }
}
