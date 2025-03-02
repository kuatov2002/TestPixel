using UnityEngine;
public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform slotsParent;
    [SerializeField] private GameObject slotPrefab;
    public static UIManager Instance { get; private set; }

    private SlotUI[] slotUIComponents;

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

        slotUIComponents = new SlotUI[InventoryManager.Instance.inventoryData.slots.Length];

        for (int i = 0; i < InventoryManager.Instance.inventoryData.slots.Length; i++)
        {
            var slotObj = Instantiate(slotPrefab, slotsParent);
            var slotUI = slotObj.GetComponent<SlotUI>();
            slotUI.Initialize(InventoryManager.Instance.inventoryData.slots[i], i);

            slotUIComponents[i] = slotUI;
        }
    }

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
        for (int i = 0; i < slotUIComponents.Length; i++)
        {
            if (slotUIComponents[i] != null)
                slotUIComponents[i].Initialize(InventoryManager.Instance.inventoryData.slots[i], i);
        }
    }
}