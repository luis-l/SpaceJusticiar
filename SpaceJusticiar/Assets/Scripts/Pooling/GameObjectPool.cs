using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Game object pool that should be used explicitly per Game Object type.
///	A Game Object Pool for bullet objects should only be used for bullets.
/// </summary>
public class GameObjectPool : MonoBehaviour
{

    private Stack<GameObject> _poolContainer;

    // The pool will make instance copies from this game object. 
    private GameObject _pooledTypeTemplate;

    /// <summary>
    /// Creates a pool of template Game Objects.
    /// </summary>
    /// <returns><c>true</c>, if pool was inited, <c>false</c> otherwise.</returns>
    /// <param name="template">Template.</param>
    /// <param name="initialReserve">Initial reserve.</param>
    public bool InitPool(GameObject template, int initialReserve = 20)
    {
        if (template == null) {
            Debug.LogError("Attempted to fill pool with a null template.");
            return false;
        }
        _pooledTypeTemplate = template;
        _pooledTypeTemplate.SetActive(false);
        _poolContainer = new Stack<GameObject>(initialReserve * 2);

        for (int i = 0; i < initialReserve; i++) {
            AddNew();
        }
        return true;
    }

    /// <summary>
    /// Returns and activates a Game Object.
    /// </summary>
    public GameObject Fetch()
    {
        if (IsPoolEmpty()) {
            AddNew();
        }
        GameObject go = _poolContainer.Pop();
        go.SetActive(true);
        return go;
    }

    /// <summary>
    /// Deactivates and adds the instance back into the pool.
    /// </summary>
    /// <param name="instance">Instance.</param>
    public void Recycle(GameObject instance)
    {
        instance.SetActive(false);
        instance.transform.SetParent(transform);
        _poolContainer.Push(instance);
    }

    private void AddNew()
    {
        GameObject go = GameObject.Instantiate(_pooledTypeTemplate) as GameObject;
        go.transform.SetParent(transform);

        // Do not add the "(clone)" at the end of the name.
        go.name = _pooledTypeTemplate.name;

        _poolContainer.Push(go);
    }

    private bool IsPoolEmpty()
    {
        return _poolContainer.Count == 0;
    }
}