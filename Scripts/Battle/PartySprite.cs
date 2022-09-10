using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartySprite : MonoBehaviour
{
    Image image;

    Pokemon _pokemon;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        image.sprite = pokemon.PKBase.MenuSprite;
    }
}
