using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerTankController : TankController
{
    [SerializeField] private GameController gameController;
    [SerializeField] private Text leftBombCount;
    [SerializeField] private Text taskMessage;
    [SerializeField] private Slider taskProgress;
    [SerializeField] private GameObject aimPoint;
    [SerializeField] private CameraSwitcher cameraSwitcher;
    [SerializeField] private float rotateSensitivity = 0.5f;
    [SerializeField] private float turretRotateSensitivity = 0.5f;


    protected override void Awake()
    {
        base.Awake();
        this.tank.onBombCountChanged += () =>
        {
            this.leftBombCount.text = $"剩餘砲彈數量: {this.tank.currentBombCount}";
        };
    }

    void Update()
    {
        this.tank.UpdateTurretRotateAngleRatio(Input.GetAxis("Mouse X"));
        this.tank.UpdateGunRaiseAngleRatio(Input.GetAxis("Mouse Y"));
        if (Input.GetKeyUp(KeyCode.Q))
        {
            this.tank.ToggleHeadLight();
        }

        if (!this.gameController.isPause && this.tank.isAlive && Input.GetMouseButton(1))
        {
            this.aimPoint.SetActive(true);
            this.cameraSwitcher.DisplayInFirstPerson();
        }
        else
        {
            this.aimPoint.SetActive(false);
            this.cameraSwitcher.DisplayInThirdPerson();
        }

        if (this.tank.isAlive)
        {
            this.DecideAction(this.GetSurroundingBrokenTank());

            // fire bomb
            if (!this.gameController.isPause && Input.GetMouseButtonUp(0))
            {
                this.tank.FireBomb();
            }

            if (Input.GetKeyUp(KeyCode.R))
            {
                this.tank.ReloadBomb();
            }



            this.tank.UpdateMoveSpeedRatio(Input.GetAxis("Vertical") * this.turretRotateSensitivity);
            this.tank.UpdateRotationAngleRatio(Input.GetAxis("Horizontal") * this.rotateSensitivity);
        }

        if (this.tank.isAlive && this.tank.reloadLeftTime > 0)
        {
            this.taskProgress.gameObject.SetActive(true);
            this.taskMessage.gameObject.SetActive(true);
            this.DisplayReloadProgress((this.tank.reloadDuration - this.tank.reloadLeftTime) / this.tank.reloadDuration, "裝彈中...");
        }
        else
        {
            this.taskMessage.gameObject.SetActive(false);
            this.taskProgress.gameObject.SetActive(false);
        }
    }

    private void DisplayReloadProgress(float progress, string message)
    {
        this.taskMessage.text = message;
        this.taskProgress.value = progress;
    }

    private void DecideAction(Tank[] brokenTanks)
    {
        foreach (Tank tank in brokenTanks)
        {
            if (tank.IsEnemy(this.tank))
            {
                tank.Kill();
            }
            else
            {
                tank.Rescue();
            }
        }
    }

    private Tank[] GetSurroundingBrokenTank()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, 20);
        Collider[] tankColliders = colliders.Where(collider => collider.gameObject.layer == AITankerController.TANK_LAYER).ToArray();
        return colliders.Select(collider => collider.GetComponentInParent<Tank>()).Where(tank => tank && tank.state == TankState.Broken).ToArray();
    }
}
