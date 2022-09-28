using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TankState
{
    Alive,
    Broken,
    Dead,
}

public class Tank : MonoBehaviour
{
    // object
    [SerializeField] private GameObjectPool bombPool;
    [SerializeField] private Slider healthPointBar;
    [SerializeField] private Light[] headLights;
    [SerializeField] private ParticleSystem[] dusts;
    public Transform turret { get; private set; }
    public Transform gun { get; private set; }
    private Transform bombSpawnAt;
    private Rigidbody body;
    private List<WheelCollider> wheels = new List<WheelCollider>();
    private List<Transform> wheelMeshes = new List<Transform>();
    private AudioSource firingSound;
    private AudioSource moveSound;

    // setting
    public float reloadDuration = 3;
    [SerializeField] private float maxCooldown = 0.3f;
    [SerializeField] private int maxBombCount = 10;
    [SerializeField] private int maxHealthPoint = 100;
    [SerializeField] private float bombForce = 100;
    [SerializeField] private float wheelForcePerSecond = 10000;
    [SerializeField] private float rotateAnglePerSecond;
    [SerializeField] private float turretRotateAnglePerSecond;
    [SerializeField] private float gunRaiseAnglePerSecond;

    // current state
    private float moveSpeedRatio = 0; // -1 ~ 1
    private float rotateAngleRatio = 0; // -1 ~ 1
    private float turretRotateAngleRatio = 0; // -1 ~ 1
    private float gunRaiseAngleRatio = 0; // -1 ~ 1
    private float currentGunRaiseAngle = 0;
    private float currentCooldown = 0;
    private int _currentBombCount = 10;
    private int _currentHealthPoint = 100;
    private bool hasBrokenBefore = false;

    public TankState state { get; private set; } = TankState.Alive;
    public float reloadLeftTime { get; private set; }
    public int currentBombCount
    {
        get
        {
            return this._currentBombCount;
        }
        private set
        {
            this._currentBombCount = value;
            if (this._currentBombCount == 0)
            {
                this.reloadLeftTime = this.reloadDuration;
            }
            this.onBombCountChanged?.Invoke();
        }
    }
    private int currentHealthPoint
    {
        get
        {
            return this._currentHealthPoint;
        }
        set
        {
            this._currentHealthPoint = Mathf.Max(0, value);
            this.DisplayHealthPoints();
            if (this._currentHealthPoint == 0)
            {
                this.Break();
            }
            this.onHealthPointChanged?.Invoke();
        }
    }
    public bool isAlive
    {
        get
        {
            return this.state == TankState.Alive;
        }
    }

    public System.Action onBombCountChanged;
    public System.Action onStateChanged;
    private System.Action onHealthPointChanged;

    private void Awake()
    {
        this.body = this.GetComponent<Rigidbody>();
        this.turret = this.transform.Find("Turret");
        this.gun = this.turret.Find("Gun");
        this.firingSound = this.gun.GetComponent<AudioSource>();
        this.moveSound = this.GetComponent<AudioSource>();
        this.bombSpawnAt = this.gun.Find("Barrel").Find("BombSpawnAt");
        foreach (Transform wheel in this.transform.Find("WheelColliders"))
        {
            this.wheels.Add(wheel.GetComponent<WheelCollider>());
        }
        foreach (Transform wheel in this.transform.Find("WheelMeshes"))
        {
            this.wheelMeshes.Add(wheel);
        }
    }

    private void Update()
    {
        // rotate tank orientation
        float oldEulerAngleY = this.transform.eulerAngles.y;
        Vector3 newEulerAngles = Vector3.zero;
        newEulerAngles.y = (this.transform.eulerAngles.y + (this.rotateAngleRatio * this.rotateAnglePerSecond * Time.deltaTime)) % 360;
        this.transform.eulerAngles = newEulerAngles;

        // direction of turret do not affect by the direction of tank
        Vector3 newTurretEulerAngles = this.turret.localEulerAngles;
        float tankAngleDiff = oldEulerAngleY - newEulerAngles.y;
        newTurretEulerAngles.y = (this.turret.localEulerAngles.y + (this.turretRotateAngleRatio * this.turretRotateAnglePerSecond * Time.deltaTime) + tankAngleDiff) % 360;
        this.turret.localEulerAngles = newTurretEulerAngles;

        // raise gun
        this.currentGunRaiseAngle = Mathf.Clamp((this.currentGunRaiseAngle - (this.gunRaiseAngleRatio * this.gunRaiseAnglePerSecond * Time.deltaTime)), -30, 0);
        this.gun.localRotation = Quaternion.AngleAxis(this.currentGunRaiseAngle, Vector3.right);

        // reduce cooldown
        if (this.currentCooldown > 0)
        {
            this.currentCooldown = Mathf.Max(0, this.currentCooldown - Time.deltaTime);
        }

        // reduce reloadLeftTime
        if (this.reloadLeftTime > 0)
        {
            this.reloadLeftTime = Mathf.Max(0, this.reloadLeftTime - Time.deltaTime);
            if (this.reloadLeftTime == 0)
            {
                this.currentBombCount = this.maxBombCount;
            }
        }

        if (!this.moveSound.isPlaying && this.body.velocity.sqrMagnitude >= 0.5)
        {
            this.moveSound.Play();
            foreach (ParticleSystem dust in this.dusts)
            {
                dust.Play();
            }
        }
        else if (this.body.velocity.sqrMagnitude < 0.5)
        {
            this.moveSound.Pause();
            foreach (ParticleSystem dust in this.dusts)
            {
                dust.Stop();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!this.isAlive)
        {
            return;
        }
        // simulate wheel rotating
        for (int i = 0; i < this.wheelMeshes.Count; i++)
        {
            wheels[i].GetWorldPose(out Vector3 pos, out Quaternion quat);
            this.wheelMeshes[i].rotation = quat;
        }

        this.wheels.ForEach((wheel) =>
        {
            if (this.moveSpeedRatio == 0)
            {
                wheel.motorTorque = 0;
                wheel.brakeTorque = this.wheelForcePerSecond * Time.fixedDeltaTime;
            }
            else
            {
                wheel.brakeTorque = 0;
                wheel.motorTorque = this.moveSpeedRatio * Time.fixedDeltaTime * this.wheelForcePerSecond;
            }
        });
    }

    public bool IsEnemy(Tank tank)
    {
        return this.tag != tank.tag;
    }

    public void UpdateMoveSpeedRatio(float value)
    {
        this.moveSpeedRatio = Mathf.Clamp(value, -1, 1);
    }

    public void UpdateRotationAngleRatio(float value)
    {
        this.rotateAngleRatio = Mathf.Clamp(value, -1, 1);
    }

    public void UpdateTurretRotateAngleRatio(float value)
    {
        this.turretRotateAngleRatio = Mathf.Clamp(value, -1, 1);
    }

    public void UpdateGunRaiseAngleRatio(float value)
    {
        this.gunRaiseAngleRatio = Mathf.Clamp(value, -1, 1);
    }

    public void ToggleHeadLight()
    {
        foreach (Light light in this.headLights)
        {
            light.gameObject.SetActive(!light.gameObject.activeSelf);
        }
    }

    public void FireBomb()
    {
        if (this.currentCooldown > 0)
        {
            return;
        }
        if (this.reloadLeftTime > 0)
        {
            return;
        }
        this.currentCooldown = this.maxCooldown;
        this.bombPool.Get().GetComponent<Bomb>().Fire(this, this.bombSpawnAt.position, this.bombSpawnAt.forward * this.bombForce);
        this.currentBombCount--;
        this.firingSound.Play();
    }

    public void ReloadBomb()
    {
        if (this.currentBombCount == this.maxBombCount)
        {
            return;
        }
        this.reloadLeftTime = this.reloadDuration;
    }

    public void Hurt(int damage)
    {
        if (this.state != TankState.Alive)
        {
            return;
        }
        this.currentHealthPoint -= damage;
    }

    public void Kill()
    {
        if (this.state != TankState.Broken)
        {
            return;
        }
        this.state = TankState.Dead;
        this.healthPointBar.gameObject.SetActive(false);
    }

    public void Rescue()
    {
        if (this.state != TankState.Broken)
        {
            return;
        }
        this.currentHealthPoint = this.maxHealthPoint;
        this.state = TankState.Alive;
        this.currentBombCount = this.maxBombCount;
        this.reloadLeftTime = 0;
        this.onStateChanged?.Invoke();
    }

    private void Break()
    {
        this.state = this.hasBrokenBefore ? TankState.Dead : TankState.Broken;
        if (!this.hasBrokenBefore)
        {
            this.hasBrokenBefore = true;
        }
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.moveSpeedRatio = 0;
        this.rotateAngleRatio = 0;
        this.wheels.ForEach(wheel => wheel.brakeTorque = float.MaxValue);
        this.onStateChanged?.Invoke();
    }

    private void DisplayHealthPoints()
    {
        this.healthPointBar.value = (float)this.currentHealthPoint / this.maxHealthPoint;
    }
}
