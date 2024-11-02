using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreLabel : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nicknameDisplay, scoreDisplay;

    public void Construct(CharacterData characterData, string nickname, string score)
    {
        icon.sprite = characterData.CharacterIcon;
        nicknameDisplay.text = nickname;
        scoreDisplay.text = score;
    }
}
