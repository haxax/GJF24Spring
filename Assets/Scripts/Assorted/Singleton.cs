using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	private static T _instance;

	/// <summary> Static global instance of the object. </summary>
	public static T Instance
	{
		get
		{
			// If instance isn't set, try to find existing one or create new.
			if (_instance == null)
			{
				_instance = FindObjectOfType<T>();
				if (_instance == null)
				{
					GameObject singletonObject = new GameObject(typeof(T).Name);
					_instance = singletonObject.AddComponent<T>();
					_instance.InstantiateSingleton();
				}
			}
			return _instance;
		}
	}


	/// <summary> Check if OnInstantiate is called. Call only once per lifetime. </summary>
	private bool _instantiated = false;



	/// <summary> Use OnInstantiate instead to ensure the actions are performed before the singleton is used by other scripts. </summary>
	private void Awake()
	{
		if (_instance == null) { _instance = (T)this; InstantiateSingleton(); }
		else if (_instance != this)
		{
			Debug.LogWarning($"Duplicate instance of {typeof(T).Name} found. Destroying the new instance.");
			Destroy(gameObject);
		}
		else { InstantiateSingleton(); }
	}

	private void InstantiateSingleton()
	{
		// Check if OnInstantiate is called during this lifetime.
		if (_instantiated) { return; }
		_instantiated = true;

		OnInstantiate();
	}

	/// <summary> Called by Awake and when created if Instance is null - before other scripts gets to use this singleton. </summary>
	protected virtual void OnInstantiate() { }
}