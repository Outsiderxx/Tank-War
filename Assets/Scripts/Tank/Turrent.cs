using UnityEngine;

public class Turrent : MonoBehaviour
{
    public float rotateSensitivity = 0.5f;
    private Transform tank;
    private Transform gun;

    private void Awake()
    {
        this.tank = this.transform.parent;
        this.gun = this.transform.Find("Gun");
    }

    private void Update()
    {



    }
}
