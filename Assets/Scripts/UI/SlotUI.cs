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

    public void Initialize(InventorySlot slot, int index)
    {
        this.slot = slot;
        this.slotIndex = index;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (slot.item == null)
        {
            icon.sprite = null;
            Color transparent = icon.color;
            transparent.a = 0;
            icon.color = transparent;
        }
        else
        {
            icon.sprite = slot.item.icon;
            Color opaque = icon.color;
            opaque.a = 1;
            icon.color = opaque;
        }

        quantityText.text = (slot.item != null && slot.quantity > 1) ? slot.quantity.ToString() : "";
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
        SlotUI targetSlot = dropTarget?.GetComponentInParent<SlotUI>();

        if (targetSlot != null && targetSlot.slot.isLocked)
        {
            Debug.Log("Ячейка заблокирована!");
            DragDropManager.Instance.EndDrag(-1);
            return;
        }

        DragDropManager.Instance.EndDrag(targetSlot?.slotIndex ?? -1);
    }
}