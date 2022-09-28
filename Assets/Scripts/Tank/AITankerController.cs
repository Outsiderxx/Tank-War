using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class AITankerController : TankController
{
    [SerializeField] Transform[] targetPoints;
    [SerializeField] float detectRadius = 0;
    [SerializeField] bool pathDirection = false;
    [SerializeField] private Transform currentTarget;

    public static readonly int TANK_LAYER = 30;
    private NavMeshAgent agent;
    private int currentPointIndex = 1;
    private int checkDelayFrame = 3;
    private Vector3? reloadPosition = null;

    protected override void Awake()
    {
        base.Awake();
        this.agent = this.GetComponentInParent<NavMeshAgent>();
        SkyBoxController.instance.onTimeStateChanged += () =>
        {
            this.tank.ToggleHeadLight();
        };
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, this.detectRadius);
    }

    void Update()
    {
        this.tank.UpdateTurretRotateAngleRatio(0);
        this.tank.UpdateGunRaiseAngleRatio(0);
        if (this.tank.isAlive)
        {
            this.DecideAction(this.GetSurroundingTank());
        }
        else
        {
            this.agent.isStopped = true;
        }

        if (this.checkDelayFrame > 0)
        {
            this.checkDelayFrame--;
        }

        if (this.tank.reloadLeftTime == 0)
        {
            this.reloadPosition = null;
        }
    }

    private Tank[] GetSurroundingTank()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, this.detectRadius);
        Collider[] tankColliders = colliders.Where(collider => collider.gameObject.layer == AITankerController.TANK_LAYER).ToArray();
        return colliders.Select(collider => collider.GetComponentInParent<Tank>()).Where(tank =>
        {
            if (!tank)
            {
                return false;
            }
            if (!this.tank.IsEnemy(tank) && tank.state == TankState.Broken)
            {
                return true;
            }
            if (this.tank.IsEnemy(tank) && tank.state != TankState.Dead)
            {
                return true;
            }
            return false;
        }).ToArray();
    }

    private void DecideAction(Tank[] tanks)
    {
        bool haveActionToDo = false;
        bool needReload = false;
        Tank[] tanksInDescendingOrder = tanks.GroupBy(tank => tank).
        Select(tankGroup => tankGroup.First()).
        OrderBy(tank => Vector3.Distance(this.transform.position, tank.transform.position)).ToArray();
        foreach (Tank tank in tanksInDescendingOrder)
        {
            if (!this.tank.IsEnemy(tank))
            {
                this.RescueAlly(tank);
                haveActionToDo = true;
                break;
            }
            if (tank.state == TankState.Broken)
            {
                this.killEnemy(tank);
                haveActionToDo = true;
                break;
            }
            else if (this.tank.reloadLeftTime == 0)
            {
                this.AttackEnemy(tank);
                haveActionToDo = true;
                break;
            }
            else
            {
                needReload = true;
            }
        }
        if (haveActionToDo)
        {
            return;
        }
        if (needReload)
        {
            if (this.reloadPosition == null)
            {
                Vector3 reloadPosition = this.transform.position;
                int length = 100;
                float randomDegree = Random.Range(0, 360);
                reloadPosition.x += Mathf.Cos(Mathf.Deg2Rad * randomDegree) * length;
                reloadPosition.z += Mathf.Sin(Mathf.Deg2Rad * randomDegree) * length;
                this.reloadPosition = reloadPosition;
            }
            this.MoveTo((Vector3)this.reloadPosition);
            return;
        }
        if (this.checkDelayFrame == 0 && this.CheckIfIsArriveTargetPoint())
        {
            if (this.currentPointIndex == 0 && this.pathDirection)
            {
                this.pathDirection = false;

            }
            else if (this.currentPointIndex == this.targetPoints.Length - 1 && !this.pathDirection)
            {
                this.pathDirection = true;
            }
            this.currentPointIndex += this.pathDirection ? -1 : 1;
            this.checkDelayFrame = 3;
        }
        this.MoveTo(this.targetPoints[this.currentPointIndex]);
    }

    private void MoveTo(Transform target)
    {
        this.currentTarget = target;
        this.agent.isStopped = false;
        this.agent.SetDestination(target.position);
    }

    private void MoveTo(Vector3 position)
    {
        this.agent.isStopped = false;
        this.agent.SetDestination(position);
    }

    private void RescueAlly(Tank ally)
    {
        if (Vector3.Distance(this.tank.transform.position, ally.transform.position) < 20)
        {
            ally.Rescue();
        }
        else
        {
            this.MoveTo(target: ally.transform);
        }
    }

    private void AttackEnemy(Tank enemy)
    {
        // calculate distance
        float distance = Vector3.Distance(this.transform.position, enemy.transform.position);
        if (distance < this.detectRadius)
        {
            // adjust turret and gun angle
            if (this.AimToTarget(enemy))
            {
                this.agent.isStopped = true;
                this.tank.FireBomb();
            };
        }
        else
        {
            this.MoveTo(target: enemy.transform);
        }
    }

    private bool AimToTarget(Tank tank)
    {
        bool canFire = true;
        // adjust turret orientation
        float targetTurretLocalEulerAngleY;
        Vector3 directionVector = tank.turret.position - this.tank.turret.position;
        targetTurretLocalEulerAngleY = -Vector3.SignedAngle(directionVector, Vector3.forward, Vector3.up) - this.tank.transform.localEulerAngles.y;
        float deltaAngle = Mathf.DeltaAngle(this.tank.turret.localEulerAngles.y, targetTurretLocalEulerAngleY);
        if (Mathf.Abs(deltaAngle) > 5)
        {
            this.tank.UpdateTurretRotateAngleRatio(deltaAngle < 0 ? -1 : 1);
            canFire = false;
        }

        // adjust gun raise angle
        float distance = Vector3.Distance(tank.transform.position, this.tank.transform.position);
        float targetGunEulerAngleX = this.tank.gun.localEulerAngles.y;
        targetGunEulerAngleX = Mathf.Clamp(-30 * Mathf.Max(distance - 115, 0) / 100 + Random.Range(-3, 3), -30, 0);
        deltaAngle = Mathf.DeltaAngle(this.tank.gun.localEulerAngles.x, targetGunEulerAngleX);
        if (Mathf.Abs(deltaAngle) > 3)
        {
            this.tank.UpdateGunRaiseAngleRatio(deltaAngle < 0 ? 1 : -1);
            canFire = false;
        }
        return canFire;
    }

    private void killEnemy(Tank enemy)
    {
        if (Vector3.Distance(this.tank.transform.position, enemy.transform.position) < 20)
        {
            enemy.Kill();
        }
        else
        {
            this.MoveTo(target: enemy.transform);
        }
    }

    private bool CheckIfIsArriveTargetPoint()
    {
        if (!this.agent.pathPending)
        {
            if (this.agent.remainingDistance <= this.agent.stoppingDistance)
            {
                if (!this.agent.hasPath || this.agent.velocity.sqrMagnitude < 0.1f)
                {
                    return true;
                }
            }
        }
        return false;
    }


}
