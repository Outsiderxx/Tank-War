using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bind3DTarget : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void LateUpdate()
    {
        this.transform.position = Camera.main.WorldToScreenPoint(this.target.position);
    }
}
