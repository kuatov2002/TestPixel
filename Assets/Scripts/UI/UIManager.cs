using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform slotsParent;
    [SerializeField] private GameObject slotPrefab;
    public static UIManager Instance { get; private set; }

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

    private void Start()
    {
        CreateInventoryUI();
    }

    private void CreateInventoryUI()
    {
        foreach (Transform child in slotsParent) Destroy(child.gameObject);

        foreach (var slot in InventoryManager.Instance.inventoryData.slots)
        {
            var slotObj = Instantiate(slotPrefab, slotsParent);
            var slotUI = slotObj.GetComponent<SlotUI>();
            slotUI.Initialize(slot);
        }
    }

    // Обработчики кнопок
    public void OnShootButton()
    {
        InventoryManager.Instance.UseRandomAmmo();
    }

    public void AddAmmos()
    {
        InventoryManager.Instance.AddAmmos();
    }

    public void AddRandomItems()
    {
        InventoryManager.Instance.AddRandomItems();
    }
}