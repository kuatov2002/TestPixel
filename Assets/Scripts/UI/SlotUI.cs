using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private Text quantityText;
    [SerializeField] private GameObject lockOverlay;

    private InventorySlot slot;

    public void Initialize(InventorySlot slot)
    {
        this.slot = slot;
        UpdateUI();
    }

    public void UpdateUI()
    {
        icon.sprite = slot.item?.icon;
        quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
        lockOverlay.SetActive(slot.isLocked);
    }

    // Drag & Drop реализация
    public void OnBeginDrag(PointerEventData eventData)
    {
        //if (slot.item == null) return;
        //DragDropManager.Instance.StartDrag(slot);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Реализация перемещения
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //DragDropManager.Instance.EndDrag();
    }
}