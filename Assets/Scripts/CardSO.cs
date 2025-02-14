using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card Game/Card")]
public class CardSO : ScriptableObject
{
    public string cardName;
    public Sprite Sprite;
    public string description;
    public int manaCost;
}