using UnityEngine;

[CreateAssetMenu(fileName = "AmmoItemData", menuName = "Items/Ammo")]
public class AmmoItemData : ItemData
{
    public AmmoType ammoType;
}
public enum AmmoType
{
    Type1,
    Type2
}