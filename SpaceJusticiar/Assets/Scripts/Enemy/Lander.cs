using UnityEngine;
using System.Collections;

public class Lander : MonoBehaviour
{
    [SerializeField]
    private ObjectController _oc;

    public LaserCannon mainGun = null;
    public TargetingSystem targetingSystem = null;

    private EnergyCell _energyCell = null;

    public Sprite secondFormSprite = null;
    public GameObject secondFormProjectileType = null;

    // Use this for initialization
    void Start()
    {
        _energyCell = new EnergyCell(100f);
        _energyCell.setEmptiedCellWaitTime(1f);
        mainGun.FiringDelay = 0.25f;

        Vector2 up = CelestialBody.GetUp(_oc.PlanetTarget, transform);
        Vector2 down = -up;

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = down * 3f;
        //rigid.MovePosition();

        Vector2 pointOnSurface = (Vector2)_oc.PlanetTarget.transform.position + _oc.PlanetTarget.GetSurfaceRadius() * up;

    }

    // Update is called once per frame
    void Update()
    {
        _energyCell.Update();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // Convert fighter to second form
        if (other.gameObject.name == _oc.PlanetTarget.Graphic.name) {
            Rigidbody2D rigid = GetComponent<Rigidbody2D>();
            rigid.velocity.Set(0, 0);

            GetComponent<SpriteRenderer>().sprite = secondFormSprite;
            mainGun.ProjectileType = secondFormProjectileType;
            mainGun.FiringDelay = 0.06f;
            mainGun.firingForce = 800;
            mainGun.spread = 2f;

            AudioSource gunSound = mainGun.GetComponent<AudioSource>();
            gunSound.pitch = 1.4f;
            gunSound.volume = 0.14f;
        }
    }
}
