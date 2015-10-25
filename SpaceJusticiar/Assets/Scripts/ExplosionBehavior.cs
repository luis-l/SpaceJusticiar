using UnityEngine;
using System.Collections;

public class ExplosionBehavior : MonoBehaviour
{

    private ParticleSystem _explosionParticles;
    private AudioSource _explosionSfx;

    private float _explosionTimer = 0f;

    // Use this for initialization
    void Start()
    {
        _explosionParticles = gameObject.GetComponent<ParticleSystem>();
    }

    void FixedUpdate()
    {

        // Play the particle system at a fixed time step.
        if (_explosionTimer <= _explosionParticles.startLifetime) {
            _explosionParticles.Simulate(Time.fixedDeltaTime, false, false);
            _explosionTimer += Time.fixedDeltaTime;
        }

        // Reset and recycle particles.
        else {
            _explosionTimer = 0f;
            _explosionParticles.Simulate(0, false, true);
            _explosionParticles.Stop();
            Pools.Instance.Recycle(gameObject);
        }

    }

    void OnEnable()
    {
        if (_explosionSfx == null) {
            _explosionSfx = GetComponent<AudioSource>();
        }
        _explosionSfx.Play();
    }
}
