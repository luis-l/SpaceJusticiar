using UnityEngine;
using System.Collections;

public class ObjectController : MonoBehaviour
{
    private CelestialBody _planetTarget = null;
    private HealthComponent _health = null;

    public delegate void OnDeathDelegate();
    public event OnDeathDelegate OnDeathEvent = delegate { };

    void Awake()
    {
        _health = new HealthComponent();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _health.Update();
    }

    public void Destroy()
    {
        OnDeathEvent();
        Destroy(gameObject, 0.001f);

        // Do no let the camera die.
        if (Camera.main.transform.parent == transform) {
            Camera.main.transform.parent = null;
        }
    }

    private void ApplyDamage(float damage)
    {
        _health.DealDamage(damage);

        SpawnDamageTextResponse(damage);

        if (_health.GetHealth() == 0) {
            GameObject explosion = Pools.Instance.Fetch("BlueEnergyExplosion");
            explosion.transform.position = transform.position;

            ParticleSystem effect = explosion.GetComponent<ParticleSystem>();
            effect.Play();

            Destroy();
        }
    }

    void SpawnDamageTextResponse(float damage)
    {
        GameObject textDamage = UIPools.Instance.Fetch("DamageText");
        TextBehavior textBehavior = textDamage.GetComponent<TextBehavior>();
        textBehavior.life = 1.5f;
        textBehavior.lerpSpeed = Random.Range(0.15f, 0.5f);
        textBehavior.Text.text = (damage * 100).ToString("0.##");

        Vector2 screenCoord = Camera.main.WorldToScreenPoint(transform.position);
        textBehavior.RectTransform.position = screenCoord;
        textBehavior.endPosition = screenCoord + Random.insideUnitCircle * 300;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Projectile") {

            // Projectile is meant this object
            ProjectileBehavior proj = other.gameObject.GetComponent<ProjectileBehavior>();
            if (proj.targetTag == gameObject.tag) {
                ApplyDamage(proj.damage);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        float impactForce = collision.relativeVelocity.magnitude;
        float damage = impactForce * impactForce * 0.01f;

        if (damage > 0.08f) {
            ApplyDamage(damage);
        }
    }

    public CelestialBody PlanetTarget
    {
        get { return _planetTarget; }
        set
        {
            transform.parent = value.gameObject.transform;
            _planetTarget = value;
        }
    }

    public HealthComponent Health
    {
        get { return _health; }
    }
}
