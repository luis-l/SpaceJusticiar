﻿using UnityEngine;
using System.Collections;

public class Lander : MonoBehaviour
{
    [SerializeField]
    private ObjectController _oc;

    public LaserCannon mainGun = null;
    public TargetingSystem targetingSystem = null;

    public Sprite secondFormSprite = null;
    public GameObject secondFormProjectileType = null;

    private Rigidbody2D _rigid;
    private bool _bLanded = false;

    // Use this for initialization
    void Start()
    {
        _oc.EnergyCell = new EnergyCell(100f);
        _oc.EnergyCell.setEmptiedCellWaitTime(1f);
        mainGun.FiringDelay = 0.25f;
        
        _rigid = GetComponent<Rigidbody2D>();

        targetingSystem.EnergyCell = _oc.EnergyCell;
    }

    void FixedUpdate()
    {
        if (!_bLanded) {
            // Land on surface
            Vector2 up = CelestialBody.GetUp(_oc.PlanetTarget, transform);
            Vector2 pointOnSurface = (Vector2)_oc.PlanetTarget.transform.position + _oc.PlanetTarget.GetSurfaceRadius() * up * 0.85f;
            
            Vector2 nextPos = Vector2.Lerp(transform.position, pointOnSurface, Time.fixedDeltaTime * 0.1f);
            _rigid.MovePosition(nextPos);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // Convert fighter to second form
        if (other.gameObject.name == _oc.PlanetTarget.Graphic.name) {
            
            _bLanded = true;

            Rigidbody2D rigid = GetComponent<Rigidbody2D>();
            rigid.velocity.Set(0, 0);

            GetComponent<SpriteRenderer>().sprite = secondFormSprite;
            mainGun.ProjectileType = secondFormProjectileType;
            mainGun.FiringDelay = 0.07f;
            mainGun.firingForce = 800;
            mainGun.spread = 2f;

            AudioSource gunSound = mainGun.GetComponent<AudioSource>();
            gunSound.pitch = 1.4f;
            gunSound.volume = 0.14f;
        }
    }

}
