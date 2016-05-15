using UnityEngine;
using System.Collections;

public abstract class SystemBase
{
    // Update is called once per frame
    public abstract void Update();

    public virtual void FixedUpdate()
    {

    }
}
