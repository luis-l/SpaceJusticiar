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

    void Update()
    {
        // Play the particle system at a fixed time step.
        if (_explosionTimer <= _explosionParticles.startLifetime) {
            _explosionTimer += Time.deltaTime;
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

        if (_explosionSfx != null) {
            _explosionSfx.pitch = Random.Range(0.9f, 1.2f);
            _explosionSfx.Play();
        }
    }
}
