using UnityEngine;

[SerializeField]
public enum SkillOfRange
{
    단일, 범위
}
[SerializeField]
public enum SkillMode
{
    단발, 지속
}

public class UnitSkill : MonoBehaviour
{
    [SerializeField] private SkillDataObject _skillData;
    [SerializeField] private SkillOfRange _targetOfRange;
    [SerializeField] private SkillMode _mode;

    int _editableSkillAttackRate;
    float _editableSkillRange;
    float _editableSkillsOfRange;
    int _editableSkillDemCost;
    //int _editableSingleTargetSkill;
    //int _oneTimeSkill;
    //int _onGoingSkill;
    int _targetType;
    //▼유닛이 바라보는 타겟의 정보를 받아와야함.
    GameObject _target;
    // 스킬 쏜사람
    private GameObject from;
    Vector3 _position;

    public int SkillAttackRate
    {
        get => _skillData.SkillAttackRate + _editableSkillAttackRate;
    }
    public void SetSkillAttackRaet(int skillAttackRaet)
    {
        _editableSkillAttackRate += skillAttackRaet;
    }

    public float SkillRange
    {
        get => _skillData.SkillRange + _editableSkillRange;
    }

    //▼_position은 때리던 놈의 위치
    public void SetSkillRange(Vector3 _position, Vector3 Skill)
    {
        switch (_targetOfRange)
        {
            case SkillOfRange.단일:
                SingleTargetSkill(_position, Skill);
                break;

            case SkillOfRange.범위:
                OfRangeSkill(_position, Skill);
                break;
        }
    }

    public int SkillDemCost
    {
        get => _editableSkillDemCost;
        set => _editableSkillDemCost = value;
    }

    public int TargetType
    {
        get => _targetType;
        set => _targetType = value;
    }

    //▼_position은 때리던 놈의 위치
    void SingleTargetSkill(Vector3 _position, Vector3 SkillRange)
    {
        if (_target != null)
        {
            Debug.Log($"{_target}공격했음");
            ApplySkill(_target);
        }
    }
    void OfRangeSkill(Vector3 _position, Vector3 SkillRange)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(_position, SkillAttackRate / 2);
        foreach(var hitCollider in hitColliders)
        {
            if (from.tag != hitCollider.tag)
            {
                ApplySkill(hitCollider.gameObject);
            }
        }
    }
    void ApplySkill(GameObject target)
    {
        Unit caster = from.GetComponent<Unit>();
        SetSkillAttackRaet(caster.AttackRate);
        target.GetComponent<Unit>().TakeDamage(SkillAttackRate, from);
    }
}
