using UnityEngine;

public class EnergyCell
{
    private float _currentCharge;
    private float _regenRate;

    public const float MAX_ENERGY = 1f;
    public const float MIN_ENERGY = 0f;

    /// <summary>
    /// This timer is to provide a wait time between successive (quick) usage of the cell,
    /// before being able to recharge again.
    /// </summary>
    private CountUpTimer _intermediateUsageTimer;

    /// <summary>
    /// If the energy cell is depleted of energy then there is a wait time
    /// before the cell starts recharging again.
    /// </summary>
    private CountUpTimer _emptiedCellTimer;

    public EnergyCell(float regenRate = 0.15f, float currentCharge = 1f)
    {
        _regenRate = regenRate;
        Charge = currentCharge;

        _intermediateUsageTimer = new CountUpTimer(1f);
        _emptiedCellTimer = new CountUpTimer(5f);
    }

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
        if (_currentCharge < MAX_ENERGY && !_intermediateUsageTimer.IsRunning() && !_emptiedCellTimer.IsRunning()) {
            Charge += _regenRate * Time.deltaTime;
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
        if (!HasCharge())
            return false;

        Charge -= amountRequested;

        // Cell is empty/overused
        if (Charge == 0) {
            _emptiedCellTimer.Start();
            _intermediateUsageTimer.Stop();
        }

        _intermediateUsageTimer.Start();
        return true;
    }

    /// <summary>
    /// Sets the time interval needed to wait between successive cell usage before recharge.
    /// </summary>
    public void setIntermediateWaitTime(float waitTime)
    {
        _intermediateUsageTimer.TargetTime = waitTime;
    }

    /// <summary>
    /// Sets the time interval needed to wait before recharging an emptied cell.
    /// </summary>
    public void setEmptiedCellWaitTime(float waitTime)
    {
        _emptiedCellTimer.TargetTime = waitTime;
    }

    public bool HasCharge()
    {
        return Charge > 0f;
    }
}
