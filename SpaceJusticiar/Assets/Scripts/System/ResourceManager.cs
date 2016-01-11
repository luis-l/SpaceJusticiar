using UnityEngine;
using System.Collections;

public class ResourceManager : MonoBehaviour
{

    private static AudioManager _audioManager;

    // Use this for initialization
    void Start()
    {
        _audioManager = new AudioManager();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static AudioManager Audio
    {
        get
        {
            return _audioManager;
        }
    }
}
