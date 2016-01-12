using UnityEngine;
using System.Collections;

public class LaserCannon : MonoBehaviour
{
    private float _lowestFiringDelay = 0.015f;
    private float _highestFiringDelay = 0.3f;

    public float firingSpeed = 60f;

    // Timer to limit the firing rate.
    private CountUpTimer _firingTimer = null;

    private Transform _nozzleTrans = null;
    public GameObject projectileType = null;
    public AudioSource firingSfx = null;

    void Start()
    {
        if (_nozzleTrans == null)
            _nozzleTrans = transform;

        _firingTimer = new CountUpTimer(0.1f);
        _firingTimer.autoReset = true;
    }

    /// <summary>
    /// Fires a projectile at the target position, and consumes
    /// energy from the cell.
    /// </summary>
    /// <param name="targetPos"></param>
    public void Fire(Vector2 targetPos, string targetTag, EnergyCell energyCell)
    {
        if (!_firingTimer.IsRunning()) {
            _firingTimer.Start();
        }

        if (_firingTimer.IsDone() && energyCell.UseEnergy(ProjectileBehavior.energyConsumption)) {

            GameObject proj = Pools.Instance.Fetch(projectileType.name);

            Vector2 toTarget = targetPos - (Vector2)_nozzleTrans.position;
            toTarget.Normalize();

            proj.transform.position = _nozzleTrans.position;
            proj.transform.rotation = _nozzleTrans.rotation;
            proj.GetComponent<Rigidbody2D>().velocity = toTarget * firingSpeed;
            proj.GetComponent<ProjectileBehavior>().targetTag = targetTag;

            firingSfx.Play();
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
}