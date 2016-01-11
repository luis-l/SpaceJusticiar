using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A timer manager that updates timers automatically.
/// </summary>
public class SystemTimer : MonoBehaviour
{

    private static List<CountUpTimer> _timers;

    // Use this for initialization
    void Start()
    {
        _timers = new List<CountUpTimer>();
    }

    // Update is called once per frame
    void Update()
    {

        // Iterate in reverse so null timers can be removed.
        for (int i = _timers.Count - 1; i >= 0; i--) {

            CountUpTimer timer = _timers[i];
            if (timer == null) {
                _timers.Remove(timer);
            }
            else {
                _timers[i].CountUpTick();
            }
        }
    }

    public static void RemoveTimer(CountUpTimer timer)
    {
        _timers.Remove(timer);
    }

    public static void RegisterTimer(CountUpTimer timer){
        _timers.Add(timer);
    }
}
