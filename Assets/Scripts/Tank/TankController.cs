using UnityEngine;

public class TankController : MonoBehaviour
{
    protected Tank tank { get; private set; }

    protected virtual void Awake()
    {
        this.tank = this.GetComponent<Tank>();
    }
}
