﻿using UnityEngine;

public class LaserCannon : MonoBehaviour
{
    private float _lowestFiringDelay = 0.015f;
    private float _highestFiringDelay = 0.25f;

    public float firingForce = 5000f;

    // Timer to limit the firing rate.
    private CountUpTimer _firingTimer = null;

    [SerializeField]
    private Transform _nozzleTrans = null;

    private GameObject _projectileType = null;
    private ProjectileBehavior _projBehavior = null;

    public AudioSource firingSfx = null;

    void Awake()
    {
        if (_nozzleTrans == null)
            _nozzleTrans = transform;

        _firingTimer = new CountUpTimer(0.1f);
    }

    /// <summary>
    /// Fires a projectiles at the target position, and consumes
    /// energy from the cell.
    /// </summary>
    /// <param name="targetPos"></param>
    public void Fire(Vector2 targetPos, string targetTag, EnergyCell energyCell, Vector2 initialVelocity)
    {
        if (!_firingTimer.HasStarted()) {
            _firingTimer.Start();
        }

        if (_firingTimer.IsDone() && energyCell.UseEnergy(_projBehavior.energyCost)) {
            FireProjectile(targetPos, targetTag, energyCell, initialVelocity);

            // Allow for 'single fire' mode by doing a forced reset of the timer.
            _firingTimer.Restart();
        }
    }

    public float FiringDelay
    {
        get { return _firingTimer.TargetTime; }
        set
        {
            if (value > _highestFiringDelay)
                _firingTimer.TargetTime = _highestFiringDelay;
            
            else if (value < _lowestFiringDelay)
                _firingTimer.TargetTime = _lowestFiringDelay;
            
            else
                _firingTimer.TargetTime = value;
        }
    }

    private void FireProjectile(Vector2 targetPos, string targetTag, EnergyCell energyCell, Vector2 initialVelocity)
    {
        GameObject proj = Pools.Instance.Fetch(_projectileType.name);

        Vector2 toTarget = targetPos - (Vector2)_nozzleTrans.position;
        toTarget.Normalize();

        proj.transform.position = _nozzleTrans.position;
        proj.transform.rotation = _nozzleTrans.rotation;

        Rigidbody2D projRigid = proj.GetComponent<Rigidbody2D>();
        projRigid.velocity = initialVelocity;
        projRigid.AddForce(toTarget * firingForce);
        proj.GetComponent<ProjectileBehavior>().targetTag = targetTag;

        firingSfx.Play();
    }

    public GameObject ProjectileType
    {
        get { return _projectileType; }

        set
        {
            _projectileType = value;
            _projBehavior = _projectileType.GetComponent<ProjectileBehavior>();
        }
    }
}