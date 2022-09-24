using UnityEngine;
using System;

public class Bomb : MonoBehaviour
{
    public Rigidbody Rigidbody { get; private set; }
    public TrailRenderer Trail { get; private set; }
    public GameObject Body { get; private set; }
    public ParticleSystem Explosion { get; private set; }
    public Action OnExplodeEndAction;

    private void Awake()
    {
        this.Rigidbody = this.GetComponent<Rigidbody>();
        this.Trail = this.GetComponent<TrailRenderer>();
        this.Body = this.transform.Find("Body").gameObject;
        this.Explosion = this.transform.Find("Explosion").GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (this.transform.position.y < -10)
        {
            this.OnExplodeEndAction.Invoke();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        this.Explosion.Play();
        this.Rigidbody.detectCollisions = false;
        this.Rigidbody.useGravity = false;
        this.Rigidbody.velocity = Vector3.zero;
        this.Body.SetActive(false);
    }

    public void OnExplodeEnd()
    {
        this.OnExplodeEndAction.Invoke();
    }
}
