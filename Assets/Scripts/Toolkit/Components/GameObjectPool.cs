using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initializeCount;
    private Queue<GameObject> queue = new Queue<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < this.initializeCount; i++)
        {
            this.Put(this.SpawnInstance());
        }
    }

    public GameObject Get()
    {
        GameObject gameObject;
        if (queue.Count > 0)
        {
            gameObject = queue.Dequeue();
        }
        else
        {
            gameObject = this.SpawnInstance();
        }
        this.Reuse(gameObject);
        return gameObject;
    }

    public void Put(GameObject gameObject)
    {
        this.Unuse(gameObject);
        this.queue.Enqueue(gameObject);
    }

    protected virtual GameObject SpawnInstance()
    {
        GameObject gameObject = Instantiate(this.prefab);
        gameObject.transform.SetParent(this.transform);
        return gameObject;
    }

    protected virtual void Reuse(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }

    protected virtual void Unuse(GameObject gameObject)
    {
        gameObject.SetActive(false);
        gameObject.transform.SetParent(this.transform);
    }
}
