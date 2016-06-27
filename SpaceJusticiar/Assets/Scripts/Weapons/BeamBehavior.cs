using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class BeamBehavior : EnergyObject
{
    public float range = 10f;

    [SerializeField]
    private float _trailLife = 0.1f;

    private CountUpTimer _trailTimer;

    private LineRenderer _lineRenderer;

    /// <summary>
    /// The position the beam will orient towards to.
    /// </summary>
    public Vector2 targetPosition;

    void Awake()
    {
        _trailTimer = new CountUpTimer(_trailLife);

        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_trailTimer.IsDone()) {
            Pools.Instance.Recycle(gameObject);
        }

        DecayTrailAlpha();
    }

    public void Project()
    {
        Vector2 start = transform.position;
        Vector2 toTarget = (targetPosition - start).normalized;
        Vector2 end = start + toTarget * range;

        RaycastHit2D hit = Physics2D.Raycast(start, toTarget, range);

        Vector3 lineStart = start;
        Vector3 lineEnd = end;

        lineStart.z = lineEnd.z = 10f;

        // Draw beam trail.
        _lineRenderer.SetPosition(0, lineStart);
        _lineRenderer.SetPosition(1, lineEnd);

        if (hit.collider != null) {

            _lineRenderer.SetPosition(1, hit.point);

            // Deal damage
            ObjectController oc = hit.collider.gameObject.GetComponent<ObjectController>();
            if (oc != null) {

                // Less damage is dealt at farther ranges.
                float beamLength = (start - hit.point).magnitude;
                float hitDistRatio = beamLength / range;
                float finalDamage = damage * (1 - hitDistRatio);
                oc.ApplyDamage(finalDamage);

                // Make bullet impacts responsive near the player by shaking the camera a bit.
                if (hit.collider.tag != "Player") {
                    NearImpactShake();
                }

                // Player is hit.
                else {
                    OnHitShake(finalDamage);
                }
            }

            GameObject explosion = Pools.Instance.Fetch(_explosionTypeName);
            explosion.transform.position = hit.point;

            ParticleSystem effect = explosion.GetComponent<ParticleSystem>();
            effect.Play();
        }

        _trailTimer.Start();
    }

    void OnDisable()
    {
        _trailTimer.Stop();
    }

    private void DecayTrailAlpha()
    {
        Color c = _lineRenderer.material.color;
        c.a = (1 - _trailTimer.CurrentTick()) / _trailLife;
        _lineRenderer.material.color = c;
    }

    public float TrailLife
    {
        set
        {
            _trailLife = value;
            _trailTimer.TargetTime = _trailLife;
        }
    }

    private void NearImpactShake()
    {
        Vector2 distToCam = Camera.main.transform.position - transform.position;
        float mag = distToCam.magnitude;
        if (mag < 20) {

            // The closer the impact, the stronger the camera shake.
            float shakeScalar = 1f / (mag * mag);
            if (shakeScalar > 0.4f) {
                shakeScalar = 0.4f;
            }

            CameraShake camShake = Systems.Instance.SystemUI.CameraController.CameraShake;
            camShake.duration = 0.3f;
            camShake.magnitude = shakeScalar;
            camShake.speed = 3f;
            camShake.PlayShake();
        }
    }

    private void OnHitShake(float damageScalar)
    {
        CameraShake camShake = Systems.Instance.SystemUI.CameraController.CameraShake;
        camShake.duration = 0.75f;
        camShake.magnitude = Mathf.Clamp(damageScalar, 0.3f, 1.5f);
        camShake.speed = 5f;
        camShake.PlayShake();

        Systems.Instance.SystemUI.CameraController.FillScreen(new Color(1, 1, 1, damage * 10), 0.1f);
    }
}
