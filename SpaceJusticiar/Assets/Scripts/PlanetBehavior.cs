﻿using UnityEngine;
using System.Collections;

using Vectrosity;

public class PlanetBehavior : MonoBehaviour
{
    private float _boundThickness = 5;
    private int _segments = 100;

    private Color _planetColor = new Color(210/255f, 180/255f, 140/255f);

    // Use this for initialization
    void Start()
    {

        // Set the collider radius.
        CircleCollider2D collider = GetComponent<CircleCollider2D>();

        // Set up the visual representation for the planet.
        Vector3[] points = new Vector3[_segments];

        VectorLine planetBounds = new VectorLine("planetBounds", points, null, _boundThickness, LineType.Continuous);
        planetBounds.SetColor(_planetColor);
        planetBounds.MakeCircle(transform.position, collider.radius, _segments - 1, 0);
        planetBounds.Draw3DAuto();
    }

    // Update is called once per frame
    void Update()
    {

    }
}