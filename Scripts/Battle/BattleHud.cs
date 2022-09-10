using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    
    [SerializeField] Text currentHP;
    [SerializeField] Text maxHP;

    [SerializeField] List<Image> StatusField;

    [SerializeField] Image statusParalysis;
    [SerializeField] Image statusBurn;
    [SerializeField] Image statusSleep;
    [SerializeField] Image statusPoison;
    [SerializeField] Image statusFreeze;
    [SerializeField] Image statusNormal;
    [SerializeField] Image statusFainted;

    Image image;
    Vector3 originalPos;
    Pokemon _pokemon;

    [SerializeField] bool isPlayer;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        StatusField.Add(statusParalysis);
        StatusField.Add(statusBurn);
        StatusField.Add(statusSleep);
        StatusField.Add(statusPoison);
        StatusField.Add(statusFreeze);
        StatusField.Add(statusNormal);
        StatusField.Add(statusFainted);

        foreach (Image status in StatusField)
        {
            Debug.Log(status);
        }
    }

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.PKBase.Name;
        levelText.text = pokemon.PKLevel.ToString();
        hpBar.SetHP((float) pokemon.HP / pokemon.MaxHp);
        currentHP.text = pokemon.HP.ToString();
        maxHP.text = pokemon.MaxHp.ToString();

        SetStatusUI();
        _pokemon.OnStatusChange += SetStatusUI;
        
         StartCoroutine(PlayHUDEnterAnimation());
    }

    void DisableConditions()
    {
        foreach (Image Condition in StatusField)
        {
            Condition.gameObject.SetActive(false);
        }
    }

    void SetStatusUI()
    {
        if (this.gameObject.activeInHierarchy)
        {
            var PokemonStatus = _pokemon.Status.Affliction;

            switch (PokemonStatus)
            {
                case "Paralyzed":
                    {
                        DisableConditions();
                        if (StatusField.Exists(Status => statusParalysis))
                        {
                            statusParalysis.gameObject.SetActive(true);
                            Debug.Log("ta em shoq");
                        }
                    }
                    break;

                case "Asleep":
                    {
                        DisableConditions();
                        if (StatusField.Exists(Status => statusSleep))
                        {
                            statusSleep.gameObject.SetActive(true);
                            Debug.Log("a mimi");
                        }
                    }
                    break;

                case "Burned":
                    {
                        DisableConditions();
                        if (StatusField.Exists(Status => statusBurn))
                        {
                            statusBurn.gameObject.SetActive(true);
                            Debug.Log("queima rosca");
                        }
                    }                        
                    break;

                case "Poisoned":
                    {
                        DisableConditions();
                        if (StatusField.Exists(Status => statusPoison))
                        {
                            statusPoison.gameObject.SetActive(true);
                            Debug.Log("dontyouknowthatimtoxic");
                        }
                    }
                    break;

                case "Frozen":
                    {
                        DisableConditions();
                        if (StatusField.Exists(Status => statusFreeze))
                        {
                            statusFreeze.gameObject.SetActive(true);
                            Debug.Log("fica frio aí");
                        }
                    }
                    break;

                case "Fainted":
                    {
                        DisableConditions();
                         if (StatusField.Exists(Status => statusFainted))
                        {
                            statusFainted.gameObject.SetActive(true);
                            Debug.Log("faliceu");
                        }
                    }
                    break;

                default: 
                    foreach (Image Condition in StatusField)
                        {
                            Condition.gameObject.SetActive(false);
                        }
                    Debug.Log("ta safe"); 
                    break;
            }
        }
    }

    public IEnumerator UpdateHP()
    {
        if (_pokemon.HpChange)
        {
            yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHp);
            _pokemon.HpChange = false;
        }
        yield return currentHP.text = _pokemon.HP.ToString();
    }

    IEnumerator PlayHUDEnterAnimation()
    {
        if (isPlayer)
        {
            image.transform.localPosition = new Vector3(860f, originalPos.y);
            yield return new WaitForSeconds(1f);
            image.transform.DOLocalMoveX(originalPos.x, 0.55f);
        }
        else
        {
            image.transform.localPosition = new Vector3(-850f, originalPos.y);
            image.transform.DOLocalMoveX(originalPos.x, 0.55f);
        }
    }
}
