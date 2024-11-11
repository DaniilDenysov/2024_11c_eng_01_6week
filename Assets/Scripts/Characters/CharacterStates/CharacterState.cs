using Cards;
using General;
using UnityEngine;

public abstract class CharacterState
{ 
    public abstract bool IsCardUsable(Card card);
    public abstract bool IsCardDiscardable(Card card);
    public abstract bool IsMovable();
}