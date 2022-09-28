using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera firstPersonCamera;
    [SerializeField] private CinemachineVirtualCamera thirdPersonCamera;

    public void DisplayInFirstPerson()
    {
        this.firstPersonCamera.Priority = 1;
        this.thirdPersonCamera.Priority = 0;
    }

    public void DisplayInThirdPerson()
    {
        this.thirdPersonCamera.Priority = 1;
        this.firstPersonCamera.Priority = 0;
    }
}
