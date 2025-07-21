//임시 제작 타입
using UnityEngine.UI;

public enum EPokemonType { Fire, Water, Grass, Nomal }

public class Card
{
    EPokemonType _type;
    EPokemonName _pokemon;
    EUnitType _unitType;
    int _cost;
    Image _cardImage;
    string _name;

    bool isAllCardGet;      //전체 카드중 사용 가능여부
    bool isUserCardEquip;   // 유저덱에 장착중 여부
    bool isBattleCardUse;   // 전투중 사용가능여부(최대 소환갯수 넘었는지 여부)
    bool isHandCard;        //손에 카드 들고있는지 여부

    bool isSelecte;         //이 카드가 선택될때 값줄용도.



    /// <summary>
    /// 유닛 카드 생성자 입니다.
    /// </summary>
    /// <param name="type">EPokemonType 이넘문 사용해서 타입을 지정해주세요.</param>
    /// <param name="cost">소환시 사용될 코스트를 적어주세요</param>
    /// <param name="name">소환될 몬스터의 이름을 정해주세요</param>
    /// <param name="image">소환될 몬스터의 이미지를 정해주세요</param>
    public Card(EUnitType unitType, EPokemonType type, int cost, EPokemonName pokemon, Image image)
    {
        _type = type;
        _cost = cost;

        _cardImage = image;
        _pokemon = pokemon;
        _name = pokemon.ToString();

        _unitType = unitType;

        isAllCardGet = false;
        isUserCardEquip = false;
        isBattleCardUse = true;
        IsSelecte = false;

        isHandCard = false;

    }

    /// <summary>
    /// 타워 카드 생성자 입니다.
    /// </summary>
    /// <param name="type">포켓몬 속성 EPokemonType.으로 사용</param>
    /// <param name="name">포켓몬 건물타입 이름 ETowerPokemonName. 으로 사용</param>
    /// <param name="unitType">포켓몬 유닛타입 -지상 공중 건물 EUnitType. 으로 사용</param>
    /// <param name="image">포켓몬 객체가 가지고있을 이미지</param>
    public Card(EPokemonType type, ETowerPokemonName name, Image image)
    {
        _type = type;
        _cost = 0;

        _cardImage = image;
        _name = name.ToString();

        _unitType = EUnitType.건물;

        isAllCardGet = false;
        isUserCardEquip = false;
        isBattleCardUse = false;
        IsSelecte = false;

        isHandCard = false;

    }


    public Image CardImage { get { return _cardImage; } set { _cardImage = value; } }

    /// <summary>
    ///  전투중 사용가능여부(최대 소환갯수 넘었는지 여부)
    /// </summary>
    public bool IsBattleCardUse { get { return isBattleCardUse; } set { isBattleCardUse = value; } }

    /// <summary>
    /// 현재 장착중인지 여부
    /// </summary>
    public bool IsUserCardEquip { get { return isUserCardEquip; } set { isUserCardEquip = value; } }

    /// <summary>
    /// 전체 리스트중 이 카드를 사용가능 카드 여부
    /// </summary>
    public bool IsAllCardGet { get { return isAllCardGet; } set { isAllCardGet = value; } }


    /// <summary>
    /// 손에 카드 들고있는지 여부
    /// </summary>
    public bool IsHandCard { get { return isHandCard; } set { isHandCard = value; } }

    /// <summary>
    /// 포켓몬타입 주고받는용
    /// </summary>
    public EPokemonType Type { get { return _type; } set { _type = value; } }

    /// <summary>
    /// 현재 선택중인 카드일경우 True
    /// </summary>
    public bool IsSelecte { get => isSelecte; set => isSelecte = value; }

    /// <summary>
    /// 코스트 반환만 할 예정인 프로퍼티
    /// </summary>
    public int Cost { get { return _cost; } }
    public string Name { get { return _name; } }
    public EPokemonName Pokemon { get { return _pokemon; } }
    public EPokemonType GetCardType()
    {
        return _type;
    }





}
