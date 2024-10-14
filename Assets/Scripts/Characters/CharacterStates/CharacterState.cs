using General;
using UnityEngine;

public abstract class CharacterState
{ 
    public abstract bool OnCardUsed(CardPoolable card);
}