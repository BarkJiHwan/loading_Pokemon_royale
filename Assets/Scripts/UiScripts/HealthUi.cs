using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    Image hpImage; //이미지 컴포넌트 담을 변수
    //GameObject Unitobj; //플레이어를 담을 변수
    float UnitMaxHP; //플레이어의 최대 체력을 담을 변수
    float UnitCurrHP; //플레이어의 현재 체력 담을 변수
    Unit myUnit;//유닛을 담을 변수

    Transform unitPos;

    public void SetUnit(GameObject unitobj)
    {
        hpImage = GetComponent<Image>(); //이미지 컴포넌트 담기


        unitPos = unitobj.GetComponent<Transform>();
        transform.position = Camera.main.WorldToScreenPoint(unitPos.position);

        myUnit = unitobj.GetComponent<Unit>();
        UnitMaxHP = myUnit.UnitData.MaxHp; //최대 체력 담아두기
        UnitCurrHP = UnitMaxHP;//처음 소환할때는 최대최력이니깐 최대 체력을담아줌
    }


    public void HpUpdate()
    {

        UnitCurrHP = myUnit.Hp;
        float toFill; //채워야 할 양을 임시로 저장할 변수
        toFill = UnitCurrHP / UnitMaxHP; //현재 체력 나누기 최대 체력
        hpImage.fillAmount = toFill; //이미지 컴포넌트의 fillamount에 수치 대입 가능
    }

    public void HpMove()
    {
        transform.position = Camera.main.WorldToScreenPoint(unitPos.position + new Vector3(0, 0.3f, 0));
    }

    public void HpDestroy()
    {
        Destroy(this.gameObject);
    }

}
