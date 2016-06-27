using UnityEngine;
using System.Collections;

public class EnergyObject : MonoBehaviour {

    public float energyCost = 0.03f;

    public float damage = 0.01f;

    [SerializeField]
    protected string _explosionTypeName = "GreenEnergyExplosion";

}
