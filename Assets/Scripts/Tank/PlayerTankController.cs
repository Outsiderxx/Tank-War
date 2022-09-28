using UnityEngine;
using UnityEngine.UI;

public class PlayerTankController : TankController
{
    [SerializeField] private Text leftBombCount;
    [SerializeField] private Text taskMessage;
    [SerializeField] private Slider taskProgress;
    [SerializeField] private GameObject aimPoint;

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
        this.aimPoint.SetActive(Input.GetMouseButton(1));
        this.tank.UpdateTurretRotationValue(Input.GetAxis("Mouse X"));
        this.tank.UpdateGunRaiseAngleValue(Input.GetAxis("Mouse Y"));

        if (this.tank.isAlive)
        {

            // fire bomb
            if (Input.GetMouseButtonUp(0))
            {
                this.tank.FireBomb();
            }

            if (Input.GetKeyUp(KeyCode.R))
            {
                this.tank.ReloadBomb();
            }

            if (Input.GetKeyUp(KeyCode.Q))
            {
                this.tank.ToggleHeadLight();
            }

            this.tank.UpdateMovementValue(Input.GetAxis("Vertical"));
            this.tank.UpdateRotationValue(Input.GetAxis("Horizontal"));
        }

        if (this.tank.isAlive && this.tank.reloadLeftTime > 0)
        {
            this.taskProgress.gameObject.SetActive(true);
            this.taskMessage.gameObject.SetActive(true);
            this.DisplayTaskProgress((this.tank.reloadDuration - this.tank.reloadLeftTime) / this.tank.reloadDuration, "裝彈中...");
        }
        else
        {
            this.taskMessage.gameObject.SetActive(false);
            this.taskProgress.gameObject.SetActive(false);
        }
    }

    private void DisplayTaskProgress(float progress, string message)
    {
        this.taskMessage.text = message;
        this.taskProgress.value = progress;
    }
}
