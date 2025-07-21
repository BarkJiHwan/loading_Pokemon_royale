using UnityEngine;
using UnityEngine.UI;

public class NextCardImage : MonoBehaviour
{
    [SerializeField] Image character;
    [SerializeField] Card _card;

    public Card Card
    {
        get { return _card; }
        private set { _card = value; }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup(Card card)
    {
        this._card = card;
        character = this.Card.CardImage;
        Image nextImage = gameObject.GetComponent<Image>();
        nextImage.sprite = character.sprite;
    }
}
