using UnityEngine;

public interface IUnitState
{
    void EnterState(Unit unit);
    void UpdateState(Unit unit);
    void FixedUpdateState(Unit unit);
}

public class SearchState : IUnitState
{
    public void EnterState(Unit unit)
    {

    }

    public void UpdateState(Unit unit)
    {
        if (unit.EnemyTarget != null)
        {
            unit.ChangeState(new AttackState());
        }
        else
        {
            unit.FindAttackableNearestEnemy();

            unit.ChangeState(new MovementState());
        }
    }

    public void FixedUpdateState(Unit unit)
    {

    }
}

public class AttackState : IUnitState
{
    int EnemyPos { get; set; }
    public void EnterState(Unit unit)
    {
        if (unit.EnemyTarget == null)
        {
            unit.ChangeState(new SearchState());
        }
    }

    public void UpdateState(Unit unit)
    {
        if (unit.EnemyTarget == null)
        {
            unit.ChangeState(new MovementState());
        }

        // 적 타겟이 있는데 사거리 밖에있다
        if (unit.EnemyTarget != null && unit.IsEnemyOutOfAttackRange())
        {
            unit.ChangeState(new MovementState());
        }
    }

    public void FixedUpdateState(Unit unit)
    {
        unit.Attack();
    }
}

public class SkillState : IUnitState
{
    public void EnterState(Unit unit)
    {
    }

    public void UpdateState(Unit unit)
    {
    }

    public void FixedUpdateState(Unit unit)
    {
    }
}

public class MovementState : IUnitState
{
    public void EnterState(Unit unit)
    {
    }

    public void UpdateState(Unit unit)
    {
    }

    public void FixedUpdateState(Unit unit)
    {
        // 건물이면 이동하지마
        if (unit.UnitType == EUnitType.건물)
        {
            unit.ChangeState(new SearchState());
        }

        //인식한 적이 있으면 적으로 이동한다
        if (unit.UnitType == EUnitType.공중)
        {
            if (unit.EnemyTarget != null)
            {
                unit.DecideWalkParameter(unit.gameObject, unit.EnemyTarget);
                unit.moveToFly(unit.EnemyTarget.transform.position);
                unit.ChangeState(new AttackState());
            }
            else
            {
                unit.DecideWalkParameter(unit.gameObject, GetTargetObject(unit.gameObject));
                unit.moveToFly(GetTargetObject(unit.gameObject).transform.position);
                unit.ChangeState(new SearchState());
            }
        }

        if(unit.UnitType == EUnitType.지상)
        {
            if (unit.Roads.Count == 0)
            {
                unit.MyMapPing(unit.gameObject);
            }

            if (unit._nextNode != null && (unit._nextNode.transform.position == unit.gameObject.transform.position))
            {
                unit._nextNode.GetComponent<TileInfo>().isMoveable = false;
                unit.MyMapPing(unit.gameObject);
            }

            if (unit.EnemyTarget != null)
            {
                unit.DecideWalkParameter(unit.gameObject, unit.EnemyTarget);
                unit.MapManager.MovingWalk(unit, unit.EnemyTarget);
                unit.ChangeState(new AttackState());
            }
            else
            {
                unit.DecideWalkParameter(unit.gameObject, GetTargetObject(unit.gameObject));
                unit.MapManager.MovingWalk(unit, GetTargetObject(unit.gameObject));
                unit.ChangeState(new SearchState());
            }
        }

        unit.HpMove();
    }
    public GameObject GetTargetObject(GameObject unit)
    {
        GameObject oSpawner = UnitSpawner.GetEnemySpawner(unit.tag);

        GameObject leftTower = oSpawner.GetComponent<UnitSpawner>().GetLeftTower(); 
        GameObject rightTower = oSpawner.GetComponent<UnitSpawner>().GetRightTower();
        GameObject castle = oSpawner.GetComponent<UnitSpawner>().GetCastle();

        if (unit.transform.position.x < 0)
        {
            return leftTower != null ? leftTower : castle;
        }
        else if (unit.transform.position.x > 0)
        {
            return rightTower != null ? rightTower : castle;
        }        
        else
        {
            return castle;
        }
    }
}
