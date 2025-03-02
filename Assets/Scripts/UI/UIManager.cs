using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform slotsParent;
    [SerializeField] private GameObject slotPrefab;
    public static UIManager Instance { get; private set; }

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

    private void Start()
    {
        CreateInventoryUI();
    }

    private void CreateInventoryUI()
    {
        foreach (Transform child in slotsParent) Destroy(child.gameObject);

        for (int i = 0; i< InventoryManager.Instance.inventoryData.slots.Length; i++)
        {
            var slotObj = Instantiate(slotPrefab, slotsParent);
            var slotUI = slotObj.GetComponent<SlotUI>();
            slotUI.Initialize(InventoryManager.Instance.inventoryData.slots[i], i);
        }
    }

    // Обработчики кнопок
    public void OnShootButton()
    {
        InventoryManager.Instance.UseRandomAmmo();
        UpdateUI();
    }

    public void AddAmmos()
    {
        InventoryManager.Instance.AddAmmos();
        UpdateUI();
    }

    public void AddRandomItems()
    {
        InventoryManager.Instance.AddRandomItems();
        UpdateUI();
    }
    public void RemoveItem()
    {
        InventoryManager.Instance.RemoveItem();
        UpdateUI();
    }
    public void UpdateUI()
    {
        foreach (Transform child in slotsParent)
        {
            var slotUI = child.GetComponent<SlotUI>();
            if (slotUI != null)
                slotUI.UpdateUI();
        }
    }

}