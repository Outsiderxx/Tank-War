using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bind3DTarget : MonoBehaviour
{
    [SerializeField] private Transform target;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Camera.main.WorldToScreenPoint(this.target.position);
    }
}
