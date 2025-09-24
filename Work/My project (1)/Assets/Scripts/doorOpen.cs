using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] item requiredKey;

    [Header("Visuals")]
    [SerializeField] Renderer[] doorIndicators;
    [SerializeField] Color lockedColor = Color.red;
    [SerializeField] Color unlockedColor = Color.green;

    [Header("Sinking Settings")]
    [SerializeField] Transform doorMesh;
    [SerializeField] float sinkDistance = 5f;
    [SerializeField] float sinkSpeed = 2f;

    private bool isOpen = false;
    private Vector3 initialPosition;
    private Vector3 targetPosition;

    private void Start()
    {
        initialPosition = doorMesh.position;
        targetPosition = initialPosition + Vector3.down * sinkDistance;

        UpdateIndicators();
    }

    private void Update()
    {
        // Smooth sinking motion
        if (isOpen && doorMesh.position != targetPosition)
        {
            doorMesh.position = Vector3.MoveTowards(doorMesh.position, targetPosition, sinkSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Called by terminal to notify the door about its keys.
    /// </summary>
    public void CheckTerminalKey(keyTerminal terminal)
    {
        if (!isOpen && terminal.HasKey(requiredKey))
        {
            Unlock();
        }
        else
        {
            UpdateIndicators();
        }
    }

    private void Unlock()
    {
        if (isOpen) return;
        isOpen = true;

        UpdateIndicators();
        Debug.Log($"{name} unlocked with key: {requiredKey}");
    }

    private void UpdateIndicators()
    {
        Color color = isOpen ? unlockedColor : lockedColor;
        foreach (Renderer rend in doorIndicators)
        {
            if (rend != null)
                rend.material.color = color;
        }
    }


    public bool IsUnlocked()
    {
        return isOpen;
    }
}
