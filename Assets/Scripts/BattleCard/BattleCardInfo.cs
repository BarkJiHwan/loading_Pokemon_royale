using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCardInfo : MonoBehaviour
{
    [SerializeField] Image BgImage;
    [SerializeField] Image Character;
    [SerializeField] Card _card;
    [SerializeField] Text CostText;
    private UnitSpawner _unitSpawner;
    public Card Card
    {
        get { return _card; }
        private set { _card = value; }
    }

    private void Awake()
    {
        BgImage = GetComponent<Image>();
    }

    public void Setup(Card card)
    {
        Card = card;
        Character = Card.CardImage;
        CostText.text = Card.Cost.ToString();
        Image charaterImage = gameObject.transform.GetChild(1).GetComponent<Image>();
        charaterImage.sprite = Character.sprite;

        if (Card.GetCardType() == EPokemonType.Fire)
        {
            BgImage.GetComponent<Image>().color = new Color(1, 0, 0, 1);
        }
        else if (Card.GetCardType() == EPokemonType.Grass)
        {
            BgImage.GetComponent<Image>().color = new Color(0, 1, 0, 1);
        }
        else if (Card.GetCardType() == EPokemonType.Water)
        {
            BgImage.GetComponent<Image>().color = new Color(0, 0, 1, 1);
        }
        else if (Card.GetCardType() == EPokemonType.Grass)
        {
            BgImage.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1);
        }
    }
}
