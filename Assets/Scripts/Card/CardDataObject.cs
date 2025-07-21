using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PokemonCardData", menuName = "Scriptable Object/PokemonCard Data")]
public class CardDataObject : ScriptableObject
{
    //타입,종류 설정
    [SerializeField] private ETowerPokemonName _towerNmae;
    [SerializeField] private EPokemonType _type;
    [SerializeField] private EPokemonName _pokemon;
    [SerializeField] private EUnitType _unitType;

    //세부 속성 설정
    [SerializeField] int _cost;
    [SerializeField] Image _cardImage;

    public EPokemonType Type { get { return _type; }  }
    public EPokemonName Pokemon { get { return _pokemon; } }
    public ETowerPokemonName TowerNmae { get { return _towerNmae; } }
    public EUnitType EUnitType { get { return _unitType; } }
    public int Cost { get { return _cost; } }
    public Image CardImage { get { return _cardImage; } }

}
