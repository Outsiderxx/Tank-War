using UnityEngine;

public class BillBoard : MonoBehaviour
{
    void Update()
    {
        this.transform.LookAt(this.transform.position + Camera.main.transform.forward);
    }
}
