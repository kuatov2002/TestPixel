// SlotUI.cs
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private Text quantityText;
    [SerializeField] private GameObject lockOverlay;

    private InventorySlot slot;
    private int slotIndex;
    private Canvas canvas;

    public void Initialize(InventorySlot slot, int index)
    {
        this.slot = slot;
        this.slotIndex = index;
        UpdateUI();
    }

    public void UpdateUI()
    {
        icon.sprite = slot.item?.icon;
        quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
        lockOverlay.SetActive(slot.isLocked);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slot.item == null || slot.isLocked) return;
        DragDropManager.Instance.StartDrag(slot, slotIndex);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (slot.item == null || slot.isLocked) return;
        DragDropManager.Instance.UpdateDragPosition(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (slot.item == null || slot.isLocked) return;

        GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;
        SlotUI targetSlot = dropTarget?.GetComponentInParent<SlotUI>(); // Изменено на GetComponentInParent

        if (targetSlot != null && targetSlot.slot.isLocked)
        {
            // Нельзя бросать в заблокированный слот
            Debug.Log("Target slot is locked!");
            DragDropManager.Instance.EndDrag(-1);
            return;
        }

        DragDropManager.Instance.EndDrag(targetSlot?.slotIndex ?? -1);
    }
}