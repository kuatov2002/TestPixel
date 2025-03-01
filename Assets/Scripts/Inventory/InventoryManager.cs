using System.Collections.Generic;
using System.Linq;
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
        inventoryData.slots = new InventorySlot[config.allSlots];
        for (int i = 0; i < config.allSlots; i++)
        {
            inventoryData.slots[i] = new InventorySlot
            {
                isLocked = i >= config.unlockedSlots
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
        if (item == null || quantity <= 0)
        {
            Debug.LogWarning("Invalid item or quantity");
            return;
        }

        // First try to stack the item with existing ones of the same type
        for (int i = 0; i < inventoryData.slots.Length; i++)
        {
            var slot = inventoryData.slots[i];

            // Skip locked or empty slots
            if (slot.isLocked) continue;

            // If the slot has the same item and is not at max stack
            if (slot.item != null && slot.item.id == item.id && slot.quantity < slot.item.maxStack)
            {
                int spaceAvailable = slot.item.maxStack - slot.quantity;
                int amountToAdd = Mathf.Min(spaceAvailable, quantity);

                // Add as much as possible to this slot
                slot.quantity += amountToAdd;
                quantity -= amountToAdd;

                // If we've added all items, we're done
                if (quantity <= 0)
                {
                    SaveInventory();
                    return;
                }
            }
        }

        // If we still have items to add or couldn't stack, find empty slots
        for (int i = 0; i < inventoryData.slots.Length; i++)
        {
            var slot = inventoryData.slots[i];

            // Skip locked slots
            if (slot.isLocked) continue;

            // If the slot is empty
            if (slot.item == null)
            {
                // Calculate how many items to add to this slot
                int amountToAdd = Mathf.Min(item.maxStack, quantity);

                // Add the item to the slot
                slot.item = item;
                slot.quantity = amountToAdd;
                quantity -= amountToAdd;

                // If we've added all items, we're done
                if (quantity <= 0)
                {
                    SaveInventory();
                    return;
                }
            }
        }

        // If we still have items but couldn't add them, inventory is full
        if (quantity > 0)
        {
            Debug.LogWarning($"Could not add {quantity} of {item.itemName} - Inventory is full!");
        }

        // Save inventory after changes
        SaveInventory();
    }

    public void RemoveItem()
    {
        // Получаем список слотов, где есть предметы
        List<InventorySlot> slots = inventoryData.slots.Where(slot => slot.item != null).ToList();

        // Если список пуст, выводим сообщение об ошибке
        if (slots.Count == 0)
        {
            Debug.LogError("Нет предметов для удаления!");
            return;
        }

        // Выбираем случайный слот из списка
        int randomIndex = Random.Range(0, slots.Count);
        InventorySlot randomSlot = slots[randomIndex];

        // Удаляем предмет из выбранного слота
        randomSlot.item = null;
        randomSlot.quantity = 0;
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