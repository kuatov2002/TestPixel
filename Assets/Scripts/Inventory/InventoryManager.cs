using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameConfig config;
    public InventoryData inventoryData;
    public static InventoryManager Instance { get; private set; }

    public List<AmmoItemData> bullets = new List<AmmoItemData>();
    public List<WeaponItemData> weapons = new List<WeaponItemData>();
    public List<ArmorItemData> armors = new List<ArmorItemData>();
    void Awake()
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

        LoadInventory();
        InitializeSlots();
    }

    private void InitializeSlots()
    {
        inventoryData.slots = new InventorySlot[30];
        for (int i = 0; i < 30; i++)
        {
            inventoryData.slots[i] = new InventorySlot
            {
                isLocked = i >= config.defaultSlots
            };
        }
    }

    public void AddAmmos()
    {
        foreach (AmmoItemData ammo in bullets)
        {
            AddItem(ammo,ammo.maxStack);
        }
    }

    public void AddRandomItems()
    {
        // Добавляем случайное оружие
        if (weapons != null && weapons.Count > 0)
        {
            int weaponIndex = Random.Range(0, weapons.Count);
            WeaponItemData randomWeapon = weapons[weaponIndex];
            AddItem(randomWeapon);
            Debug.Log($"Добавлено оружие: {randomWeapon.name}");
        }
        else
        {
            Debug.LogWarning("Список оружия пуст!");
        }

        // Фильтруем доспехи для головы
        List<ArmorItemData> headArmors = armors.FindAll(armor => armor.armorSlot == ArmorSlot.Head);
        if (headArmors != null && headArmors.Count > 0)
        {
            int headIndex = Random.Range(0, headArmors.Count);
            ArmorItemData randomHeadArmor = headArmors[headIndex];
            AddItem(randomHeadArmor);
            Debug.Log($"Добавлен головной доспех: {randomHeadArmor.name}");
        }
        else
        {
            Debug.LogWarning("Нет доспехов для головы!");
        }

        // Фильтруем доспехи для торса
        List<ArmorItemData> torsoArmors = armors.FindAll(armor => armor.armorSlot == ArmorSlot.Torso);
        if (torsoArmors != null && torsoArmors.Count > 0)
        {
            int torsoIndex = Random.Range(0, torsoArmors.Count);
            ArmorItemData randomTorsoArmor = torsoArmors[torsoIndex];
            AddItem(randomTorsoArmor);
            Debug.Log($"Добавлен доспех для торса: {randomTorsoArmor.name}");
        }
        else
        {
            Debug.LogWarning("Нет доспехов для торса!");
        }

        // Сохраняем изменения в инвентаре
        SaveInventory();
    }

    public void AddItem(ItemData item, int quantity = 1)
    {
        // Логика добавления предметов со стакированием
    }

    public void RemoveItem(int slotIndex)
    {
        // Логика удаления предметов
    }

    private void LoadInventory()
    {
        // Загрузка через InventorySaveSystem
    }

    private void SaveInventory()
    {
        // Сохранение через InventorySaveSystem
    }

    public bool UseRandomAmmo()
    {
        // Get all slots with ammo items
        List<int> slotsWithAmmo = new List<int>();

        for (int i = 0; i < inventoryData.slots.Length; i++)
        {
            var slot = inventoryData.slots[i];

            // Skip if slot is locked or empty
            if (slot.isLocked || slot.item == null) continue;

            // Check if item is ammo type
            if (slot.item is AmmoItemData && slot.quantity > 0)
            {
                slotsWithAmmo.Add(i);
            }
        }

        // If no ammo found, return false
        if (slotsWithAmmo.Count == 0)
        {
            Debug.LogWarning("No ammunition available to shoot.");
            return false;
        }

        // Select random ammo slot
        int randomIndex = Random.Range(0, slotsWithAmmo.Count);
        int selectedSlotIndex = slotsWithAmmo[randomIndex];

        // Reduce ammo quantity
        inventoryData.slots[selectedSlotIndex].quantity--;

        // If quantity reaches zero, remove item
        if (inventoryData.slots[selectedSlotIndex].quantity <= 0)
        {
            inventoryData.slots[selectedSlotIndex].item = null;
            inventoryData.slots[selectedSlotIndex].quantity = 0;
        }

        // Save inventory after changes
        SaveInventory();

        // Notify UI to update
        //OnInventoryChanged?.Invoke();

        Debug.Log($"Used 1 ammunition from slot {selectedSlotIndex}");
        return true;
    }
}