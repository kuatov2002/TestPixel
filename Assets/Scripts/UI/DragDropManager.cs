// DragDropManager.cs
using UnityEngine;
using UnityEngine.UI;

public class DragDropManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject parent;
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
    }

    public void StartDrag(InventorySlot slot, int index)
    {
        sourceSlot = slot;
        sourceIndex = index;

        // Создаем временный объект для перетаскивания
        draggedImage = Instantiate(dragImagePrefab, parent.transform);  
        draggedRect = draggedImage.GetComponent<RectTransform>();

        Image dragImage = draggedImage.GetComponent<Image>();
        dragImage.sprite = slot.item.icon;

        Text quantityText = draggedImage.GetComponentInChildren<Text>();
        quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
    }

    public void UpdateDragPosition(Vector2 position)
    {
        if (draggedImage != null && canvas != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                position,
                canvas.worldCamera,
                out Vector2 localPoint);

            draggedRect.localPosition = localPoint;
        }
    }

    public void EndDrag(int targetIndex)
    {
        Destroy(draggedImage);

        // Обмен данными между слотами
        InventoryManager.Instance.SwapSlots(sourceIndex, targetIndex);
    }
}