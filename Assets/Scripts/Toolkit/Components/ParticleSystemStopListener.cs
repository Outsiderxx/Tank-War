using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParticleSystemStopListener : MonoBehaviour
{
    [SerializeField] UnityEvent OnStop;

    private void Awake()
    {
        var main = this.GetComponent<ParticleSystem>().main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    private void OnParticleSystemStopped()
    {
        this.OnStop.Invoke();
    }
}
