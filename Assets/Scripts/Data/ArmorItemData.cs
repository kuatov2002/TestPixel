using UnityEngine;
[CreateAssetMenu(fileName = "ArmorItemData", menuName = "Items/Armor")]
public class ArmorItemData : ItemData
{
    public ArmorSlot armorSlot;
    public int defense;
}

public enum ArmorSlot
{
    Head,
    Torso
}