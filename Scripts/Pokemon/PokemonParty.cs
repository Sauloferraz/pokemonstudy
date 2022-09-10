using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> party;

    public List<Pokemon> Party
    {
        get
        {
            return party;
        }
    }

    private void Start()
    {
        foreach(var pokemon in party)
        {
            pokemon.Init();
        }
    }

    public Pokemon GetFirstPokemon()
    {
        return party.Where(x => x.HP > 0).FirstOrDefault();
    }
}
