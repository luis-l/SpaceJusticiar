using UnityEngine;
using System.Collections;

public class LaserCannon : MonoBehaviour
{

    private float _energyConsumption = 0.04f;

    // The amount of seconds between each shot.
    private float _firingDelay = 0.1f;

    private float _lowestFiringDelay = 0.015f;
    private float _highestFiringDelay = 0.3f;

    private float _firingSpeed = 60f;

    private float _firingTimer = 0;

    public GameObject _projectileType = null;

    private Transform _nozzleTrans = null;

    public AudioSource firingSfx = null;

    void Start()
    {
        if (_nozzleTrans == null)
            _nozzleTrans = transform;
    }

    /// <summary>
    /// Fires a projectile at the target position, and consumes
    /// energy from the cell.
    /// </summary>
    /// <param name="targetPos"></param>
    public void Fire(Vector2 targetPos, string targetTag, EnergyCell energyCell)
    {
        // Make sure we have enough energy to use.
        if (!energyCell.UseEnergy(_energyConsumption))
            return;

        if (_firingTimer > _firingDelay) {
            GameObject proj = Pools.Instance.Fetch(_projectileType.name);

            Vector2 toTarget = targetPos - (Vector2)_nozzleTrans.position;
            toTarget.Normalize();

            proj.transform.position = _nozzleTrans.position;
            proj.transform.rotation = _nozzleTrans.rotation;
            proj.GetComponent<Rigidbody2D>().velocity = toTarget * _firingSpeed;
            proj.GetComponent<ProjectileBehavior>().targetTag = targetTag;

            firingSfx.Play();

            _firingTimer = 0f;
        }
        else {
            _firingTimer += Time.deltaTime;
        }
    }
}