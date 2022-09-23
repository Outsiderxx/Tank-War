using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TankController : MonoBehaviour
{
    Transform gun;
    Transform turrent;
    List<WheelCollider> wheels = new List<WheelCollider>();
    List<Transform> wheelMeshes = new List<Transform>();

    public float force = 10000, rotateSensitivity = 0.5f;
    public float moveSpeed;

    private void Awake()
    {
        this.turrent = this.transform.Find("Turret");
        this.gun = this.turrent.Find("Gun");
        foreach (Transform wheel in this.transform.Find("WheelColliders"))
        {
            this.wheels.Add(wheel.GetComponent<WheelCollider>());
        }
        foreach (Transform wheel in this.transform.Find("WheelMeshes"))
        {
            this.wheelMeshes.Add(wheel);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // direction of turret do not affect by the direction of tank
        float oldAngle = this.transform.eulerAngles.y;
        this.transform.eulerAngles = new Vector3(0, (this.transform.eulerAngles.y + Input.GetAxis("Horizontal") * this.rotateSensitivity) % 360, 0);
        // this.turrent.eulerAngles = new Vector3(this.turrent.transform.eulerAngles.x, oldAngle - this.transform.eulerAngles.y, this.turrent.transform.eulerAngles.z);
    }

    private void FixedUpdate()
    {
        // simulate wheel rotating
        for (int i = 0; i < this.wheelMeshes.Count; i++)
        {
            wheels[i].GetWorldPose(out Vector3 pos, out Quaternion quat);
            this.wheelMeshes[i].rotation = quat;
        }

        this.wheels.ForEach((wheel) =>
        {
            wheel.motorTorque = Input.GetAxis("Vertical") * this.force * Time.fixedDeltaTime;
        });
    }
}
