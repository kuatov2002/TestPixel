using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "GameConfig")]
public class GameConfig : ScriptableObject
{
    public int unlockPrice = 100;
    public int unlockedSlots = 15;
    public int allSlots = 30;
}