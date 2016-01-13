using UnityEngine;

public class CountUpTimer
{

    // The goal time to count up to in seconds.
    private float _targetTime;
    private float _counter;

    /// <summary>
    /// Scales how much the tick incrementation should be.
    /// Value of 1 means it updates by Time.deltaTime at every frame.
    /// </summary>
    private readonly float _incrementScale;

    /// <summary>
    /// If the timer should reset itself and keep on counting.
    /// </summary>
    public bool autoReset = false;

    private bool _running = false;

    private bool _done = false;

    private bool _started = false;

    /// <summary>
    /// Creates a count up timer.
    /// Target time is the goal amount of time to reach.
    /// CurrentTick sets the starting time of the timer instead of zero.
    /// </summary>
    /// <param name="targetTime"></param>
    /// <param name="currentTick"></param>
    public CountUpTimer(float targetTime, float incrementScale = 1f, float currentTick = 0f)
    {
        _targetTime = targetTime;
        _incrementScale = incrementScale;
        _counter = currentTick;

        SystemTimer.RegisterTimer(this);
    }

    /// <summary>
    /// Starts the timer from 0.
    /// </summary>
    public void Start()
    {
        _running = true;
        _counter = 0f;
        _done = false;

        _started = true;
    }

    /// <summary>
    /// Stops the timer and resets it to 0.
    /// </summary>
    public void Stop()
    {
        _running = false;
        _counter = 0f;
        _done = false;
        _started = false;
    }

    /// <summary>
    /// Pauses the timer at the current tick.
    /// </summary>
    public void Pause()
    {
        _running = false;
    }

    /// <summary>
    /// Resumes the timer.
    /// </summary>
    public void Resume()
    {
        _running = true;
    }

    /// <summary>
    /// Returns true of the timer reached the goal time.
    /// Ticks the timer by Time.deltaTime otherwise.
    /// </summary>
    /// <returns></returns>
    public void CountUpTick()
    {
        if (_counter < _targetTime) {

            if (_done) {
                _done = false;
            }

            _counter += _incrementScale * Time.deltaTime;
        }

        else {

            _done = true;

            if (autoReset) {
                _counter = 0f;
            }

            // The timer is no longer running once it reaches the goal time.
            else {
                _running = false;
            }
        }
    }

    public float CurrentTick()
    {
        return _counter;
    }


    public float TargetTime
    {
        get { return _targetTime; }
        set { _targetTime = value; }
    }

    public void Reset()
    {
        _counter = 0f;
        _done = false;
        _running = true;
    }

    public bool IsRunning()
    {
        return _running;
    }

    public bool IsDone()
    {
        return _done;
    }

    public bool HasStarted()
    {
        return _started;
    }

}
