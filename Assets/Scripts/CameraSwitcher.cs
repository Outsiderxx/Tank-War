using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera firstPersonCamera;

    // Update is called once per frame
    void Update()
    {
        this.firstPersonCamera.Priority = Input.GetMouseButton(1) ? 11 : 9;
    }
}
