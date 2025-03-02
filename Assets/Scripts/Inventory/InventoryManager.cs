using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
        if (inventoryData==null)
        { 
            InitializeSlots();
        }

    }
    // InventoryManager.cs (�������� �����)
    public void SwapSlots(int sourceIndex, int targetIndex)
    {
        if (targetIndex == -1 || sourceIndex == targetIndex)
        {
            Debug.Log($"Swap cancelled. Source: {sourceIndex}, Target: {targetIndex}");
            return;
        }

        Debug.Log($"Swapping slots: {sourceIndex} <-> {targetIndex}");

        // ����� ������� ����� �������
        (inventoryData.slots[targetIndex], inventoryData.slots[sourceIndex]) =
            (inventoryData.slots[sourceIndex], inventoryData.slots[targetIndex]);

        
        SaveInventory();
        UIManager.Instance.UpdateUI();
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
        // ��������� ��������� ������
        if (weapons != null && weapons.Count > 0)
        {
            int weaponIndex = Random.Range(0, weapons.Count);
            WeaponItemData randomWeapon = weapons[weaponIndex];
            AddItem(randomWeapon);
            Debug.Log($"��������� ������: {randomWeapon.name}");
        }
        else
        {
            Debug.LogWarning("������ ������ ����!");
        }

        // ��������� ������� ��� ������
        List<ArmorItemData> headArmors = armors.FindAll(armor => armor.armorSlot == ArmorSlot.Head);
        if (headArmors != null && headArmors.Count > 0)
        {
            int headIndex = Random.Range(0, headArmors.Count);
            ArmorItemData randomHeadArmor = headArmors[headIndex];
            AddItem(randomHeadArmor);
            Debug.Log($"�������� �������� ������: {randomHeadArmor.name}");
        }
        else
        {
            Debug.LogWarning("��� �������� ��� ������!");
        }

        // ��������� ������� ��� �����
        List<ArmorItemData> torsoArmors = armors.FindAll(armor => armor.armorSlot == ArmorSlot.Torso);
        if (torsoArmors != null && torsoArmors.Count > 0)
        {
            int torsoIndex = Random.Range(0, torsoArmors.Count);
            ArmorItemData randomTorsoArmor = torsoArmors[torsoIndex];
            AddItem(randomTorsoArmor);
            Debug.Log($"�������� ������ ��� �����: {randomTorsoArmor.name}");
        }
        else
        {
            Debug.LogWarning("��� �������� ��� �����!");
        }

        // ��������� ��������� � ���������
        SaveInventory();
    }

    public void AddItem(ItemData item, int quantity = 1)
    {
        if (item == null || quantity <= 0)
        {
            Debug.LogWarning("Invalid item or quantity");
            return;
        }

        for (int i = 0; i < inventoryData.slots.Length; i++)
        {
            var slot = inventoryData.slots[i];

            if (slot.isLocked) continue;

            if (slot.item != null && slot.item.id == item.id && slot.quantity < slot.item.maxStack)
            {
                int spaceAvailable = slot.item.maxStack - slot.quantity;
                int amountToAdd = Mathf.Min(spaceAvailable, quantity);

                slot.quantity += amountToAdd;
                quantity -= amountToAdd;

                if (quantity <= 0)
                {
                    SaveInventory();
                    return;
                }
            }
        }

        for (int i = 0; i < inventoryData.slots.Length; i++)
        {
            var slot = inventoryData.slots[i];

            if (slot.isLocked) continue;

            if (slot.item == null)
            {
                int amountToAdd = Mathf.Min(item.maxStack, quantity);

                slot.item = item;
                slot.quantity = amountToAdd;
                quantity -= amountToAdd;

                if (quantity <= 0)
                {
                    SaveInventory();
                    return;
                }
            }
        }

        if (quantity > 0)
        {
            Debug.LogWarning($"Could not add {quantity} of {item.itemName} - Inventory is full!");
        }

        SaveInventory();
    }

    public void RemoveItem()
    {
        // �������� ������ ������, ��� ���� ��������
        List<InventorySlot> slots = inventoryData.slots.Where(slot => slot.item != null).ToList();

        // ���� ������ ����, ������� ��������� �� ������
        if (slots.Count == 0)
        {
            Debug.LogError("��� ��������� ��� ��������!");
            return;
        }

        // �������� ��������� ���� �� ������
        int randomIndex = Random.Range(0, slots.Count);
        InventorySlot randomSlot = slots[randomIndex];

        // ������� ������� �� ���������� �����
        randomSlot.item = null;
        randomSlot.quantity = 0;
        SaveInventory();
    }


    private void LoadInventory()
    {
        // ��������� ������ ��������� �� �����
        inventoryData = InventorySaveSystem.LoadData();

        // ���� ����������� ������ ��� ��� ����� �� ������, ����� ���������������� �� �� ���������
        if (inventoryData.slots == null || inventoryData.slots.Length == 0)
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
    }

    void OnApplicationQuit()
    {
        SaveInventory();
    }
    private void SaveInventory()
    {
        InventorySaveSystem.SaveData(inventoryData);
    }

    public bool UseRandomAmmo()
    {
        List<int> slotsWithAmmo = new List<int>();

        for (int i = 0; i < inventoryData.slots.Length; i++)
        {
            var slot = inventoryData.slots[i];

            if (slot.isLocked || slot.item == null) continue;

            if (slot.item is AmmoItemData && slot.quantity > 0)
            {
                slotsWithAmmo.Add(i);
            }
        }

        if (slotsWithAmmo.Count == 0)
        {
            Debug.LogWarning("No ammunition available to shoot.");
            return false;
        }

        int randomIndex = Random.Range(0, slotsWithAmmo.Count);
        int selectedSlotIndex = slotsWithAmmo[randomIndex];

        inventoryData.slots[selectedSlotIndex].quantity--;

        if (inventoryData.slots[selectedSlotIndex].quantity <= 0)
        {
            inventoryData.slots[selectedSlotIndex].item = null;
            inventoryData.slots[selectedSlotIndex].quantity = 0;
        }

        SaveInventory();
        return true;
    }
}