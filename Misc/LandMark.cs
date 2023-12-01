using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMark : MonoBehaviour
{
    private static LandMark _instance;
    // private GameManager gameManager;
    public static LandMark Instance
    {
        get { return _instance; }
    }

    public delegate void DeathEvent();
    public static event DeathEvent OnDeath;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
            _instance = this;
    }

    public void OnLandmarkDestroyed()
    {
        OnDeath?.Invoke();
    }
}
