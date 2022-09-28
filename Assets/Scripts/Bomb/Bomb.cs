using UnityEngine;
using System;
using System.Linq;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float damageRadius;
    private bool isCollided = false;
    private Tank belongTo;

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
        if (this.isCollided)
        {
            return;
        }
        this.isCollided = true;
        this.Explosion.Play();
        this.Rigidbody.detectCollisions = false;
        this.Rigidbody.useGravity = false;
        this.Rigidbody.velocity = Vector3.zero;
        this.Body.SetActive(false);
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, this.damageRadius);
        colliders.ToList().ForEach(collider =>
        {
            Tank tank = collider.GetComponentInParent<Tank>();
            if (tank && this.belongTo.IsEnemy(tank))
            {
                float distance = Vector3.Distance(this.transform.position, tank.transform.position);
                tank.Hurt((int)(Mathf.Max(1, this.damageRadius - distance) / this.damageRadius * 10));
            }
        });
    }

    public void Fire(Tank belongTo, Vector3 position, Vector3 force)
    {
        this.isCollided = false;
        this.belongTo = belongTo;
        this.transform.position = position;
        this.GetComponent<Rigidbody>().AddForce(force);
    }

    public void OnExplodeEnd()
    {
        this.OnExplodeEndAction.Invoke();
    }
}
