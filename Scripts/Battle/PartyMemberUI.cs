using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Color highlightedColor;

    Pokemon _pokemon;
    public Image menuSprite;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.PKBase.Name;
        levelText.text = "Lv. " + pokemon.PKLevel;
        menuSprite.sprite = pokemon.PKBase.MenuSprite;

        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHp);
    }

    public void SetSelected(bool selected)
    {
        if (selected)
            nameText.color = highlightedColor;
        else
        {
            nameText.color = Color.black;
        }
    }

}
