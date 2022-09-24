using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TankController : MonoBehaviour
{
    public GameObjectPool bombPool;
    private Transform turrent;
    private Transform gun;
    private Transform bombSpawnAt;
    private List<WheelCollider> wheels = new List<WheelCollider>();
    private List<Transform> wheelMeshes = new List<Transform>();

    [SerializeField] private float wheelForce = 10000, rotateSensitivity = 0.5f, bombForce = 100;

    private void Awake()
    {
        this.turrent = this.transform.Find("Turret");
        this.gun = this.turrent.Find("Gun");
        this.bombSpawnAt = this.gun.Find("Barrel").Find("BombSpawnAt");
        foreach (Transform wheel in this.transform.Find("WheelColliders"))
        {
            this.wheels.Add(wheel.GetComponent<WheelCollider>());
        }
        foreach (Transform wheel in this.transform.Find("WheelMeshes"))
        {
            this.wheelMeshes.Add(wheel);
        }
    }

    private void Update()
    {
        // rotate tank orientation
        Vector3 newEulerAngles = Vector3.zero;
        newEulerAngles.y = (this.transform.eulerAngles.y + Input.GetAxis("Horizontal") * this.rotateSensitivity) % 360;
        this.transform.eulerAngles = newEulerAngles;

        // fire bomb
        if (Input.GetMouseButtonUp(0))
        {
            this.FireBomb();
        }
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
            wheel.motorTorque = Input.GetAxis("Vertical") * this.wheelForce * Time.fixedDeltaTime;
        });
    }

    private void FireBomb()
    {
        GameObject bomb = this.bombPool.Get();
        bomb.transform.position = this.bombSpawnAt.position;
        bomb.GetComponent<Rigidbody>().AddForce(this.bombSpawnAt.forward * this.bombForce);
    }
}
