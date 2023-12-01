using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public enum ThrowableType
{
    Tree,
    Boulder
}

public class ThrowablePickup : MonoBehaviour
{
    public ThrowableType type;

    public float growTime = 10f;
    private float nextGrowTime;
    private float growCooldownTimer;

    public bool IsGrowing => Time.time < nextGrowTime;
    public void StartCoolDown() => nextGrowTime = Time.time + growTime;

    private Vector3 startScaleSize;

    private void Start()
    {
        startScaleSize = transform.localScale;
        growCooldownTimer = growTime;
    }

    private void Update()
    {
        if (growCooldownTimer < growTime)
        {
            growCooldownTimer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, startScaleSize, growCooldownTimer / growTime);
        }
    }

    public void OnGrabPickup()
    {
        growCooldownTimer = 0;
        transform.localScale = Vector3.zero;
        StartCoolDown();
    }
}
