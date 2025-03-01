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
        sourceSlot = slot;

        if (draggedImage == null)
        {
            draggedImage = Instantiate(dragImagePrefab, canvas.transform);
            draggedRect = draggedImage.GetComponent<RectTransform>();
        }

        Image dragImage = draggedImage.GetComponent<Image>();
        dragImage.sprite = slot.item.icon;

        Text quantityText = draggedImage.GetComponentInChildren<Text>();
        if (quantityText != null)
        {
            quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
        }

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
        if (draggedImage != null)
        {
            draggedImage.SetActive(false);
        }

        if (targetSlot == null || targetIndex == -1 || targetIndex == sourceIndex)
        {
            return;
        }

        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found in scene.");
            return;
        }

        if (targetSlot.isLocked)
        {
            Debug.Log("Cannot move item to locked slot.");
            return;
        }

        if (targetSlot.item == null)
        {
            targetSlot.item = sourceSlot.item;
            targetSlot.quantity = sourceSlot.quantity;

            sourceSlot.item = null;
            sourceSlot.quantity = 0;
        }
        else if (targetSlot.item.id == sourceSlot.item.id && targetSlot.item.maxStack>1)
        {
            int maxStack = targetSlot.item.maxStack;

            int spaceInTarget = maxStack - targetSlot.quantity;

            if (spaceInTarget > 0)
            {
                int amountToTransfer = Mathf.Min(spaceInTarget, sourceSlot.quantity);

                targetSlot.quantity += amountToTransfer;

                sourceSlot.quantity -= amountToTransfer;

                if (sourceSlot.quantity <= 0)
                {
                    sourceSlot.item = null;
                    sourceSlot.quantity = 0;
                }
            }
        }
        else
        {
            ItemData tempItem = targetSlot.item;
            int tempQuantity = targetSlot.quantity;

            targetSlot.item = sourceSlot.item;
            targetSlot.quantity = sourceSlot.quantity;

            sourceSlot.item = tempItem;
            sourceSlot.quantity = tempQuantity;
        }

        /*inventoryManager.SaveInventory();

        inventoryManager.OnInventoryChanged?.Invoke();*/
    }
}