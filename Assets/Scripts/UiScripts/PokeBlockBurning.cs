using UnityEngine;

public class PokeBlockBurning : MonoBehaviour
{
    public void SetSuddenDeathBurningStart()
    {
        gameObject.SetActive(true);
    }
    public void SetSuddenDeathBurningOver()
    {
        gameObject.SetActive(false);
    }
}
