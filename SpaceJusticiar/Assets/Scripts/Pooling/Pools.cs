
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This class creates the game object pools and organizes them in a nice hierarchy.
/// </summary>
public class Pools : MonoBehaviour
{

    private static Pools _instance;

    private Pools() { }

    public static Pools Instance
    {
        get
        {
            if (_instance == null) {
                _instance = FindObjectOfType<Pools>();
            }

            return _instance;
        }
    }

    private Transform _poolRoot;

    // The Key is the name for the GameObject type.
    private Dictionary<string, GameObjectPool> _gameObjectPools;

    public GameObject[] poolableObjects;

    void Start()
    {
        Init();

        foreach (GameObject go in poolableObjects){
            AddSubPool(go);
        }
    }

    /// <summary>
    /// Creates the pools to be used. Must be created after SystemData and SystemEntities are created.
    /// </summary>
    public void Init()
    {
        _gameObjectPools = new Dictionary<string, GameObjectPool>();

        // Get root to place all pools under.
        string rootPath = "PoolRoot/";
        _poolRoot = GameObject.Find(rootPath).transform;
        if (_poolRoot == null)
            throw new UnityException("Failed to find Object Pool Root");
    }

    /// <summary>
    /// Add a pool to hold plain GameObjects.
    /// </summary>
    /// <param name="template"></param>
    public void AddSubPool(GameObject template, int initialReserve = 10)
    {
        if (template != null) {
            // Create the game object to attach pool script.
            GameObject newPoolObject = new GameObject();
            newPoolObject.transform.SetParent(_poolRoot);
            newPoolObject.name = template.name;

            // Init pool and add to dictionary.
            GameObjectPool newPool = newPoolObject.AddComponent<GameObjectPool>();

            if (newPool.InitPool(template, initialReserve))
                _gameObjectPools.Add(template.name, newPool);
        }
        else {
            Debug.Log("Attempted to pool a null GameObject template in Class Pools");
        }
    }

    /// <summary>
    /// Returns and removes a GameObject from the pool.
    /// </summary>
    public GameObject Fetch(string name)
    {
        if (_gameObjectPools.ContainsKey(name)) {
            return _gameObjectPools[name].Fetch();
        }
        return null;
    }

    /// <summary>
    /// Deactivate and return the GameObject back into the pool.
    /// </summary>
    /// <param name="go"></param>
    public void Recycle(GameObject go)
    {
        string name = go.name; ;
        if (_gameObjectPools.ContainsKey(name)) {
            _gameObjectPools[name].Recycle(go);
        }
        else {
            Debug.LogError("Failed to recycle into pool. The GameObject (name=" + name + ") doesn't have a pool to be put in. Add one if needed.");
        }
    }
}
