using UnityEngine;
//유닛과 만드는 방법 동일
[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Object/Skill Data")]

public class SkillDataObject : ScriptableObject
{//스킬데미지, 유닛의 마나코스트?, 공격횟수(?), 단일타깃, 범위타깃, 일회성?, 지속성?, 지속시간
    [SerializeField] int _skillAttackRate; //스킬 공격력
    [SerializeField] float _skillRange; //스킬 사거리
    [SerializeField] float _skillsOfRange; //스킬 범위
    [SerializeField] int _skillDemCost; //스킬 공격 횟수
    [SerializeField] int _singleTargetSkill; //단일타겟스킬 0
    [SerializeField] int _oneTimeSkill; //단발형 스킬
    [SerializeField] int _onGoingSkill; //지속형 스킬
    [SerializeField] int _targetType; //타겟의 타입(건물 or 공중 or 지상)
    //[SerializeField] float _skillCasting; //캐스팅 시간(유닛의 공격속도로 치환)

    public int SkillAttackRate { get { return _skillAttackRate; } }
    public float SkillRange { get { return _skillRange; } }
    public float SkillsOfRange { get { return _skillsOfRange; } }
    public int SkillDemCost { get { return _skillDemCost; } }
    public int SingleTargetSkill { get { return _singleTargetSkill; } }
    public int OneTimeSkill { get { return _oneTimeSkill; } }
    public int OnGoingSkill { get { return _onGoingSkill; } }
    public int TargetType { get { return _targetType; } }
    //public float SkillCasting { get { return _skillCasting; } }

}
