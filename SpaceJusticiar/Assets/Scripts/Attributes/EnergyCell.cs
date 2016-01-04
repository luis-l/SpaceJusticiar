using UnityEngine;

public class EnergyCell
{

    private float _currentCharge = 1f;
    public const float MAX_ENERGY = 1f;
    public const float MIN_ENERGY = 0f;

    private float _regenRate = 0.1f;

    public float Charge
    {
        get { return _currentCharge; }

        set
        {
            // Cap energy.
            _currentCharge = value;
            if (_currentCharge > MAX_ENERGY)
                _currentCharge = MAX_ENERGY;

            else if (_currentCharge < MIN_ENERGY)
                _currentCharge = MIN_ENERGY;
        }
    }

    /// <summary>
    /// Return the amount of energy charge as a percent out of 100.
    /// </summary>
    /// <returns></returns>
    public int GetPercentage()
    {
        return (int)(_currentCharge * 100);
    }

    public void Update()
    {
        if (_currentCharge < MAX_ENERGY) {
            _currentCharge += _regenRate;
        }
    }

    /// <summary>
    /// To use up energy from the cell. Returns true if there is enough energy and consumes the energy requested.
    /// Returns false if there is not enough energy.
    /// </summary>
    /// <param name="amountRequested"></param>
    /// <returns></returns>
    public bool UseEnergy(float amountRequested)
    {
        if (_currentCharge - amountRequested >= MIN_ENERGY) {
            _currentCharge -= amountRequested;
            return true;
        }
        return false;
    }
}
