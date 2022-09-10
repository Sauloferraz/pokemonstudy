using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]

public class BattleUnit : MonoBehaviour
{
   
    [SerializeField] bool isPlayer;
    [SerializeField] BattleHud hud;

    public BattleHud Hud
    {
        get { return hud; }
    }

    public bool IsPlayerUnit
    {
        get { return isPlayer; }
    }

    public Pokemon Pokemon { get; set; }

    Image image;
    Vector3 originalPos;
    Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    public void Setup(Pokemon pokemon)
    {
        Pokemon = pokemon;
        if (isPlayer) 
            image.sprite = Pokemon.PKBase.BackSprite;
        else 
            image.sprite = Pokemon.PKBase.FrontSprite;

        hud.SetData(pokemon);

        image.color = originalColor;

        StartCoroutine(PlayEnterAnimation());
    }

    IEnumerator PlayEnterAnimation()
    {
        if (isPlayer)
        {
            image.transform.localPosition = new Vector3(-800f, originalPos.y);
            yield return new WaitForSeconds(0.75f);
            image.transform.DOLocalMoveX(originalPos.x, 1f);
        }
        else
        {
            image.transform.localPosition = new Vector3(700f, originalPos.y);
            image.transform.DOLocalMoveX(originalPos.x, 1f);
        }
            
    }
    public void PlayMoveAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayer)
        {
           sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.1f));
        }else sequence.Append(image.transform.DOLocalMoveX(originalPos.x + -50f, 0.1f));

        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.15f));
    }

    public void PlayStatusMoveAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayer)
        {
            sequence.Append(image.DOColor(Color.yellow, 0.6f));
            sequence.Join(image.transform.DOLocalMoveY(originalPos.y + 10f, 0.15f));
            sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 10f, 0.15f));
            sequence.Append(image.transform.DOLocalMoveY(originalPos.y + 10f, 0.15f));
            sequence.Join(image.DOColor(originalColor, 0.15f));
        }
        else
        {
            sequence.Append(image.DOColor(Color.yellow, 0.6f));
            sequence.Join(image.transform.DOLocalMoveY(originalPos.y + 10f, 0.15f));
            sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 10f, 0.15f));
            sequence.Append(image.transform.DOLocalMoveY(originalPos.y + 10f, 0.15f));
            sequence.Join(image.DOColor(originalColor, 0.15f));
        }

        sequence.Append(image.transform.DOLocalMoveY(originalPos.y, 0.15f));
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }
    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();

        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.25f));
        sequence.Join(image.DOFade(0f, 0.25f));
    }
}
