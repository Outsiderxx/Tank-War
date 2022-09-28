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
    public float reloadDuration = 3;
    [SerializeField] private GameObjectPool bombPool;
    [SerializeField] private Slider healthPointBar;
    [SerializeField] private Light[] headLights;
    [SerializeField] private float maxCooldown = 0.3f;
    [SerializeField] private int maxBombCount = 10;
    [SerializeField] private int maxHealthPoint = 100;
    [SerializeField] private float bombForce = 100;
    [SerializeField] private float wheelForce = 10000;
    [SerializeField] private float rotateSensitivity = 0.5f;
    [SerializeField] private float turretRotateSensitivity = 0.5f;

    public Transform turret;
    public Transform gun;
    private Transform bombSpawnAt;
    private List<WheelCollider> wheels = new List<WheelCollider>();
    private List<Transform> wheelMeshes = new List<Transform>();
    private float movementValue = 0;
    private float rotationValue = 0;
    private float gunRaiseAngleValue = 0;
    private float turretRotationValue = 0;
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
        this.turret = this.transform.Find("Turret");
        this.gun = this.turret.Find("Gun");
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
        if (Input.GetKeyUp(KeyCode.B))
        {
            this.Break();
        }
        // rotate tank orientation
        float oldEulerAngleY = this.transform.eulerAngles.y;
        Vector3 newEulerAngles = Vector3.zero;
        newEulerAngles.y = (this.transform.eulerAngles.y + this.rotationValue * this.rotateSensitivity) % 360;
        this.transform.eulerAngles = newEulerAngles;

        // direction of turret do not affect by the direction of tank
        Vector3 newTurretEulerAngles = this.turret.localEulerAngles;
        newTurretEulerAngles.y = (this.turret.localEulerAngles.y + this.turretRotationValue * this.turretRotateSensitivity + (oldEulerAngleY - this.transform.eulerAngles.y)) % 360;
        this.turret.localEulerAngles = newTurretEulerAngles;

        // raise gun
        this.currentGunRaiseAngle = Mathf.Clamp((this.currentGunRaiseAngle - this.gunRaiseAngleValue), -30, 0);
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
            if (this.movementValue == 0)
            {
                wheel.motorTorque = 0;
                wheel.brakeTorque = this.wheelForce * Time.fixedDeltaTime;
            }
            else
            {
                wheel.brakeTorque = 0;
                wheel.motorTorque = this.movementValue * this.wheelForce * Time.fixedDeltaTime;
            }
        });
    }

    public bool IsEnemy(Tank tank)
    {
        return this.tag != tank.tag;
    }

    public void UpdateMovementValue(float value)
    {
        this.movementValue = value;
    }

    public void UpdateRotationValue(float value)
    {
        this.rotationValue = value;
    }

    public void UpdateTurretRotationValue(float value)
    {
        this.turretRotationValue = value;
    }

    public void UpdateGunRaiseAngleValue(float value)
    {
        this.gunRaiseAngleValue = value;
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
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.movementValue = 0;
        this.rotationValue = 0;
        this.wheels.ForEach(wheel => wheel.brakeTorque = float.MaxValue);
        this.onStateChanged?.Invoke();
    }

    private void DisplayHealthPoints()
    {
        this.healthPointBar.value = (float)this.currentHealthPoint / this.maxHealthPoint;
    }
}
