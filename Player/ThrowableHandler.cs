using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// This class manages how many throwable objects the player has on them currently, and can pick up objects.
public class ThrowableHandler : MonoBehaviour
{
    public float grabDistance = 20f;

    public float throwForce = 10f;

    // Amount of current throwables
    public int treeCount;
    public int boulderCount;

    [Header("References")] 
    public GameObject[] projectiles; // List of projectiles
    [SerializeField, Tooltip("Where the throwables are instantiated")] private Transform throwPoint;
    [SerializeField] private TMP_Text treeAmountText, boulderAmountText;
    [SerializeField] private Image treeSelectedOutline, boulderSelectedOutline;
    private GameManager gameManager;

    private Camera cam;

    private int currentProjectileIndex = 0; // Index of the current projectile

    // Attack Interval Cooldown
    public float attackInterval;
    private float nextThrowTime;
    public bool IsThrowCoolingDown => Time.time < nextThrowTime;
    public void StartCoolDown() => nextThrowTime = Time.time + attackInterval;

    void Start()
    {
        gameManager = GameManager.Instance;
        cam = Camera.main;
        /*treeCount = 0;
        boulderCount = 0;*/
        treeAmountText.text = treeCount.ToString();
        boulderAmountText.text = boulderCount.ToString();
        treeSelectedOutline.enabled = true; // TREE MUST BE DEFAULT IN currentProjectileIndex
        boulderSelectedOutline.enabled = false;
    }

    void Update()
    {
        var mouse = Mouse.current;
        if (mouse.scroll.ReadValue().y > 0)
        {
            SwitchProjectile(1); // Scroll up
        }
        else if (mouse.scroll.ReadValue().y < 0)
        {
            SwitchProjectile(-1); // Scroll down
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (!gameManager.hasGameStarted) return;

        if (IsThrowCoolingDown) return;

        if (context.performed)
        {
            // TODO: Refactor as switch statement
            if (currentProjectileIndex == 0)
            {
                if (treeCount < 1)
                    return;
                else
                {
                    treeCount--;
                    treeAmountText.text = treeCount.ToString();
                    StartCoolDown();
                }
            }

            if(currentProjectileIndex == 1)
            {
                if (boulderCount < 1)
                    return;
                else
                {
                    boulderCount--;
                    boulderAmountText.text = boulderCount.ToString();
                    StartCoolDown();
                }
            }

            Quaternion rot = throwPoint.rotation * Quaternion.AngleAxis(Mathf.Abs(90), Vector3.forward);
            GameObject proj = ObjectPoolManager.SpawnObject(projectiles[currentProjectileIndex], throwPoint.position, rot, ObjectPoolManager.PoolType.GameObject);
            proj.GetComponent<Rigidbody>().AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
        }
    }

    void SwitchProjectile(int direction)
    {
        currentProjectileIndex += direction;
        if (currentProjectileIndex >= projectiles.Length)
        {
            currentProjectileIndex = 0;
        }
        else if (currentProjectileIndex < 0)
        {
            currentProjectileIndex = projectiles.Length - 1;
        }

        if (currentProjectileIndex == 0)
        {
            treeSelectedOutline.enabled = true;
            boulderSelectedOutline.enabled = false;
        }

        if (currentProjectileIndex == 1)
        {
            treeSelectedOutline.enabled = false;
            boulderSelectedOutline.enabled = true;
        }
    }

    public GameObject GetCurrentProjectile()
    {
        return projectiles[currentProjectileIndex];
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        if (!gameManager.hasGameStarted) return;

        if (context.performed)
        {
            Debug.Log("Attempting Grab");
            // Try to grab an object
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, grabDistance))
            {
                if (hit.transform.TryGetComponent<ThrowablePickup>(out ThrowablePickup throwable))
                {
                    if (!throwable.IsGrowing)
                    {
                        throwable.OnGrabPickup();
                        // Play pick up sound
                        switch (throwable.type)
                        {
                            case ThrowableType.Tree:
                                treeCount++;
                                treeAmountText.text = treeCount.ToString();
                                break;
                            case ThrowableType.Boulder:
                                boulderCount++;
                                boulderAmountText.text = boulderCount.ToString();
                                break;
                        }
                    }
                }
            }
        }
    }
}
