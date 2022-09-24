using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPool : GameObjectPool
{
    protected override GameObject SpawnInstance()
    {
        GameObject gameObject = base.SpawnInstance();
        gameObject.GetComponent<Bomb>().OnExplodeEndAction += () =>
        {
            this.Put(gameObject);
        };
        return gameObject;
    }

    protected override void Reuse(GameObject gameObject)
    {
        Bomb bomb = gameObject.GetComponent<Bomb>();
        if (!bomb)
        {
            throw new System.Exception("gameObject do not have bomb component");
        }
        base.Reuse(gameObject);

        bomb.Rigidbody.isKinematic = false;
        bomb.Body.SetActive(true);
    }

    protected override void Unuse(GameObject gameObject)
    {
        Bomb bomb = gameObject.GetComponent<Bomb>();
        if (!bomb)
        {
            throw new System.Exception("gameObject do not have bomb component");
        }
        base.Unuse(gameObject);

        bomb.Trail.Clear();
        bomb.Rigidbody.detectCollisions = true;
        bomb.Rigidbody.useGravity = true;
    }
}
