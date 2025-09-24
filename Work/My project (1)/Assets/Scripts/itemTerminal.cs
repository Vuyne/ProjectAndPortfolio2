using System.Collections.Generic;
using UnityEngine;

public class keyTerminal : MonoBehaviour
{
    [Header("Terminal Storage")]
    [SerializeField] keyInventory terminalInventory;

    [Header("Visuals")]
    [SerializeField] Renderer[] doorIndicators; // one indicator per linked door
    [SerializeField] Color lockedColor = Color.red;
    [SerializeField] Color unlockedColor = Color.green;

    [Header("Linked Doors")]
    [SerializeField] DoorController[] linkedDoors; // linked doors corresponding to indicators

    private bool playerInRange = false;
    private keyInventory playerInv;

    private void Start()
    {
        UpdateIndicators();
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            DepositAllKeys();
        }

        UpdateIndicators();
    }

    private void DepositAllKeys()
    {
        if (playerInv == null) return;

        foreach (item key in new List<item>(playerInv.GetAllKeys()))
        {
            terminalInventory.AddKey(key);
            playerInv.UseKey(key);
            Debug.Log($"Deposited {key} into terminal.");
        }

        UpdateIndicators();
        UpdateLinkedDoors();
    }

    private void UpdateIndicators()
    {
        if (doorIndicators == null || linkedDoors == null) return;

        for (int i = 0; i < doorIndicators.Length && i < linkedDoors.Length; i++)
        {
            if (doorIndicators[i] != null && linkedDoors[i] != null)
            {
                doorIndicators[i].material.color = linkedDoors[i].IsUnlocked() ? unlockedColor : lockedColor;
            }
        }
    }

    private void UpdateLinkedDoors()
    {
        if (linkedDoors == null) return;

        foreach (DoorController door in linkedDoors)
        {
            if (door != null)
                door.CheckTerminalKey(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInv = other.GetComponent<keyInventory>();
            if (playerInv != null)
                playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<keyInventory>() == playerInv)
        {
            playerInRange = false;
            playerInv = null;
        }
    }

    public bool HasKey(item keyID)
    {
        return terminalInventory.HasKey(keyID);
    }
}
