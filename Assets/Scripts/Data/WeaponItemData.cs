using UnityEngine;
[CreateAssetMenu(fileName = "WeaponItemData", menuName = "Items/Weapon")]
public class WeaponItemData : ItemData
{
    public AmmoType requiredAmmo;
    public int damage;
}