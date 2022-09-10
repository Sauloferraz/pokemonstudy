using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;
    List<Pokemon> party;

    PartyMemberUI[] memberSlots;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Pokemon> pokemon)
    {
        this.party = pokemon;
        
        for(int i = 0; i < memberSlots.Length; i++)
        {
            if (i < pokemon.Count)
                memberSlots[i].SetData(pokemon[i]);
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        messageText.text = "Choose a Pokémon";
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < party.Count; i++)
        {
            if (i == selectedMember)
                memberSlots[i].SetSelected(true);
            else 
                memberSlots[i].SetSelected(false);
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
