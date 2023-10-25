using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Suit
{
    DIAMONDS,
    HEARTS,
    CLUBS,
    SPADES
}

[CreateAssetMenu(fileName = "SO_Card", menuName = "Card", order = 0)]
public class SO_Card : ScriptableObject
{
    public string cardValue;
    public Suit suit;
    public Sprite sprite;
}
