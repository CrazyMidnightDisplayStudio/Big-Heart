using ItemSystem;
using Services;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player _instance;

    public static Player Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = FindObjectOfType<Player>();
            if (_instance != null) return _instance;
            GameObject singleton = new GameObject(nameof(Player));
            _instance = singleton.AddComponent<Player>();
            DontDestroyOnLoad(singleton);
            return _instance;
        }
    }

    private void Awake()
    {
        MakeSingleton();
    }

    private void MakeSingleton()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}