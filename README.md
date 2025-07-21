# 로딩중 유니티 2D 프로젝트

## 이 버전은 리소스파일이 포함되어 있지 않아 구동되지 않습니다.
loading_clash_royale의 리소스와 이미지 파일을 제거한 버전입니다.
해당 프로젝트의 리소스파일을 요청하시는 분이라면 제가 입사지원한 회사라고 생각합니다.
구현 환경을 보고 싶으신 분은 wlghksdl159@naver.com을 통해 요청바랍니다.

## 자세한 사항은 노션에서~
* https://www.notion.so/dduckchul/170c2881272780459a5ff2df888928c3?v=170c288127278143b23a000c46d1f8c7

## 프로젝트 초기 세팅
1. 깃 템플릿 추가 (커밋 메시지 샘플 등록)
> git config --local commit.template /.gitsettings/.gitmessage.txt
2. 깃훅 추가 (커밋 메시지 맞추기)
> git config core.hooksPath .gitsettings
3. 개행문자 수정 (https://dsaint31.tistory.com/209)
> ## windows
> git config --global core.autocrlf true
> ## mac
> git config --global core.autocrlf input

## 컨벤셔널 커밋
- https://www.conventionalcommits.org/ko/v1.0.0/

### 커밋 메시지 예시
feat: 캐릭터 기능 업데이트

1. 포켓몬 업데이트 했습니다~
2. 전투상태 업데이트


## 푸시 잘 되는지 테스트~
* https://discord.gg/wSkfuzBt 채팅방으로 웹훅 푸시함

## 클래스 다이어그램
```mermaid
classDiagram

note for CardLibrary "플레이어의 덱과\n모든 카드정보 들고있는 클래스"
note for CardManager "인게임 게임 내\n카드 셔플 및 UI 동작"
note for IUnitState "유닛의 동작\n상태 패턴으로 정의\n탐색,공격,이동"
note for SceneLoader "메인 씬과 스테이지 간\n정보 전달 (레벨,장착카드)"
note for GameManager "타이머, 포케블럭 등\n시간에 따른 게임 관리"
note for MapManager "길찾기 관련 로직들"

Card --o CardLibrary
Card --o CardManager
Card --o Unit
CardManager --o UnitSpawner
Unit -- UnitSpawner
Unit -- IUnitState
Unit -- MapManager
SceneLoader -- UnitSpawner
SceneLoader -- CardManager
SceneLoader -- GameManager
SceneLoader -- CardLibrary
EPokemonName --o Unit
EPokemonName --o Card
EPokemonName --o CardLibrary
EUnitType --o Card
EUnitType --o Unit

class EPokemonName {
    피카츄,라이츄,파이리,꼬부기,버터플,야도란,피존투,또가스,등등등
}

class EUnitType{
    공중,지상,타워
}

class Card {
    EPokemonType _type
    EPokemonName _pokemon
    EUnitType _unitType
    int _cost
    Image _cardImage
    string _name
}

class CardLibrary {
    -List<Card> cardLibrary
    -List<Card> getCards
    +Card[] equipCards
    +Card[] equipTowerCards
    +Card[] enemyEquipCards
    +Card[] enemyTowerEquipCards
    -void AcquisitionDeckAddImage(EPokemonName name)
    -void UnacquiredDeckAddImage(EPokemonName name)
    -void CostSort(List<Card> cards)
    -void MergeSort(List<Card> cards, List<Card> temp, int start, int end)
    -void AddLibrary(CardDataObject cdObj)
    +void StageSet(int index)
}

class CardManager {
    -static CardManager instance;
    -List<Card> CardBuffer;
    -Card nextCard;
    +void SetCardBuffer()
    +void CardBufferCardShuffle()
    +Card PopCard()
    +void DrawCard()
    +void ShowNextCard()
    +void ReMoveHand(GameObject gameObject)
}

class Unit {
    -각종 스테이터스
    -EUnitType _unitType
    -bool[] 공격대상
    -enemyTarget
    -List<Unit> followingUnits
    -HealthUi
    +void ChangeState(IUnitState newState)
    +void TakeDamage(int damage, GameObject from)
    -void Die()
    +void ChangeEnemyTarget()
    +double CalculateDistance(Vector3 myPos, Vector3 otherPos)
    +void FindAttackableNearestEnemy()
    +void Attack()
}

class UnitSpawner {
    +Dictionary fieldDict
    +Dictionary unitPool
    +void initUnitPool()
    +void initTower()
    +void SpawnUnit()
    +void ReturnUnit()
}

class IUnitState {
    +EnterState
    +ChangeState
    +FixedUpdateState
}

class SceneLoader {
    -SceneLoader instance
    -CardLibrary _cardLibrary
    -Card[] _equipCards
    -Card[] _towerCards
    -Card[] _enemyCards
    -Card[] _enemyTowerCards
    -int _LevelValue
    -void OnSceneLoaded()
    +void LoadStage()
}

class GameManager {
    -GameManager instance
    -bool _isGameOver
    -bool _isGameClear
    -bool _isGameStart
    -bool _isextralTime
    -bool _isSuddenDeath
    -bool _isSuddenExtra
    -bool _istimeOver
    +void GameClear()
    +void GameOver()
    +void OnExtraTimer()
    +void OnSuddenDeath()
}

class MapManager{
    +DrawingMap map
    +void Move()
}
```
