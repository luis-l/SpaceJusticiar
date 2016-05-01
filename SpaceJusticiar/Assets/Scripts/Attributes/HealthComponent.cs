using UnityEngine;

public class HealthComponent
{

    private float _health = 1f;
    private float _regenRate;

    // The wait time needed, after a hit, before starting to regenerate health.
    private CountUpTimer _hitWaitTimer = null;

    public const float MAX_HEALTH = 1f;
    public const float MIN_HEALTH = 0f;

    public HealthComponent(float regenRate = 0.04f, float initialHealth = 1f)
    {
        _regenRate = regenRate;
        _hitWaitTimer = new CountUpTimer(1f);
        Health = initialHealth;
    }

    public void Update()
    {
        if (_health < MAX_HEALTH && !_hitWaitTimer.IsRunning()) {
            Health += _regenRate * Time.deltaTime;
        }
    }

    public float GetHealth()
    {
        return _health;
    }

    private float Health
    {
        get { return _health; }
        set
        {
            if (value > MAX_HEALTH)
                _health = MAX_HEALTH;

            else if (value < MIN_HEALTH)
                _health = MIN_HEALTH;

            else
                _health = value;
        }
    }

    public void DealDamage(float dmg)
    {
        Health -= dmg;
        _hitWaitTimer.Start();
    }

    /// <summary>
    /// Return the amount of health as a percent out of 100.
    /// </summary>
    /// <returns></returns>
    public int GetPercentage()
    {
        return (int)(_health * 100);
    }

    // Reset the values for the health component;
    public void ReInit(float regen, float initHealth)
    {
        Health = initHealth;
        _regenRate = regen;
    }
}
