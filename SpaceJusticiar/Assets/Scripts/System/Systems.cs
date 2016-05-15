using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class Systems : MonoBehaviour {

    private static Systems _instance;

    private Systems() { }

    public static Systems Instance
    {
        get
        {
            if (_instance == null) {
                _instance = FindObjectOfType<Systems>();
            }

            return _instance;
        }
    }

    List<SystemBase> _systems = new List<SystemBase>();

    private SystemTimer _sysTimer;
    private SystemUI _sysUI;
    private SpaceEngine _spaceEngine;

    private void Init()
    {
        ResourceManager.Init();

        _spaceEngine = new SpaceEngine();
        _sysTimer = new SystemTimer();
        _sysUI = new SystemUI();
    }

	// Use this for initialization
    void Awake()
    {

        Init();

        // Use reflection to collect all systems inside the class.
        BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

        FieldInfo[] fieldInfos = GetType().GetFields(flags);

        foreach (FieldInfo fieldInfo in fieldInfos) {

            if (fieldInfo.FieldType.BaseType == typeof(SystemBase)) {
                SystemBase system = (SystemBase)fieldInfo.GetValue(this);
                _systems.Add(system);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < _systems.Count; i++) {
            _systems[i].Update();
        }
	}

    public SystemTimer SystemTimer { get { return _sysTimer; } }
    public SystemUI SystemUI { get { return _sysUI; } }
    public SpaceEngine SpaceEngine { get { return _spaceEngine; } }
}
