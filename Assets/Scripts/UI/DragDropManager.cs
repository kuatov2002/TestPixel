using UnityEngine;
using UnityEngine.UI;

public class DragDropManager : MonoBehaviour
{
    

    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject dragImagePrefab;

    private GameObject draggedImage;
    private RectTransform draggedRect;
    private InventorySlot sourceSlot;
    private int sourceIndex;
    public static DragDropManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void StartDrag(InventorySlot slot)
    {
        // Store source slot information
        sourceSlot = slot;

        // Create drag visual
        if (draggedImage == null)
        {
            draggedImage = Instantiate(dragImagePrefab, canvas.transform);
            draggedRect = draggedImage.GetComponent<RectTransform>();
        }

        // Set image to item icon
        Image dragImage = draggedImage.GetComponent<Image>();
        dragImage.sprite = slot.item.icon;

        // Set quantity text if needed
        Text quantityText = draggedImage.GetComponentInChildren<Text>();
        if (quantityText != null)
        {
            quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
        }

        // Show dragged image
        draggedImage.SetActive(true);
    }

    public void OnDrag(Vector2 position)
    {
        if (draggedImage != null && draggedImage.activeSelf)
        {
            draggedRect.position = position;
        }
    }

    public void EndDrag(InventorySlot targetSlot = null, int targetIndex = -1)
    {
        // Hide dragged image
        if (draggedImage != null)
        {
            draggedImage.SetActive(false);
        }

        // If no target provided or same as source, do nothing
        if (targetSlot == null || targetIndex == -1 || targetIndex == sourceIndex)
        {
            return;
        }

        // Get reference to inventory manager
        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found in scene.");
            return;
        }

        // Handle different drag scenarios
        if (targetSlot.isLocked)
        {
            Debug.Log("Cannot move item to locked slot.");
            return;
        }

        // If target is empty, simple move
        if (targetSlot.item == null)
        {
            // Move item to target slot
            targetSlot.item = sourceSlot.item;
            targetSlot.quantity = sourceSlot.quantity;

            // Clear source slot
            sourceSlot.item = null;
            sourceSlot.quantity = 0;
        }
        // If both slots have same item, try to stack
        else if (targetSlot.item.id == sourceSlot.item.id && targetSlot.item.maxStack>1)
        {
            // Get max stack size
            int maxStack = targetSlot.item.maxStack;

            // Calculate how many can be added to target
            int spaceInTarget = maxStack - targetSlot.quantity;

            if (spaceInTarget > 0)
            {
                // Calculate amount to transfer
                int amountToTransfer = Mathf.Min(spaceInTarget, sourceSlot.quantity);

                // Add to target
                targetSlot.quantity += amountToTransfer;

                // Remove from source
                sourceSlot.quantity -= amountToTransfer;

                // If source is empty, clear it
                if (sourceSlot.quantity <= 0)
                {
                    sourceSlot.item = null;
                    sourceSlot.quantity = 0;
                }
            }
        }
        // If different items, swap them
        else
        {
            // Swap items and quantities
            ItemData tempItem = targetSlot.item;
            int tempQuantity = targetSlot.quantity;

            targetSlot.item = sourceSlot.item;
            targetSlot.quantity = sourceSlot.quantity;

            sourceSlot.item = tempItem;
            sourceSlot.quantity = tempQuantity;
        }

        // Save inventory after changes
        /*inventoryManager.SaveInventory();

        // Notify UI to update
        inventoryManager.OnInventoryChanged?.Invoke();*/
    }
}