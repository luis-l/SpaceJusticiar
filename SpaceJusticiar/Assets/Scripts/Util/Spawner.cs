using UnityEngine;
using System.Collections;

public class Spawner {

    private GameObject _template;
    private CountUpTimer _spawnTimer;

    // Limit the number of spawns that can occur.
    // For example, if 10 is the max, then the spawner can
    // only have 10 active spawns at a given time.
    private readonly int MAX_SPAWNS;

    private int _activeCount = 0;

    private Transform _spawnOriginTrans;

    public delegate void OnSpawnDelegate(ObjectController oc);
    public event OnSpawnDelegate OnSpawnEvent = delegate { };

    private bool _bVariableRate;
    private float _maxSpawnRate;

    public Spawner(Transform spawnOrigin, GameObject type, float spawnRate, int maxSpawns, bool bVariableRate = false)
    {
        _spawnOriginTrans = spawnOrigin;
        _template = type;
        _spawnTimer = new CountUpTimer(spawnRate);
        _maxSpawnRate = spawnRate;
        MAX_SPAWNS = maxSpawns;
        _bVariableRate = bVariableRate;
    }

    public void StartSpawning()
    {
        _spawnTimer.Start();
    }

	// Update is called once per frame
	public void Update () {

        if (_activeCount == MAX_SPAWNS) {
            _spawnTimer.Stop();
        }

        else if (_spawnTimer.IsDone()) {
            Spawn();
            _spawnTimer.Restart();

            if (_bVariableRate) {
                _spawnTimer.TargetTime = Random.Range(_maxSpawnRate / 8f, _maxSpawnRate);
            }
        }
	}

    public void OnSpawnDeath()
    {
        _activeCount--;
        if (!_spawnTimer.IsRunning()) {
            _spawnTimer.Start();
        }
    }

    void Spawn()
    {
        _activeCount++;

        GameObject spawn = GameObject.Instantiate(_template);
        spawn.transform.position = _spawnOriginTrans.position;

        ObjectController oc = spawn.GetComponent<ObjectController>();
        oc.OnDeathEvent += OnSpawnDeath;

        OnSpawnEvent(oc);
    }
}
