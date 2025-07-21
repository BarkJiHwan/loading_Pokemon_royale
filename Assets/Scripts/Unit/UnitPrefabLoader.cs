using UnityEngine;

public class UnitPrefabLoader
{
    public static GameObject GetPrefab(EPokemonName pokemon)
    {
        GameObject prefab = null;
        prefab = Resources.Load<GameObject>("Prefabs/" + pokemon);

        if(prefab == null){
            Debug.LogError("프리팹이 없습니다 ㅠㅠ");
        }

        return prefab;
    }

    public static GameObject GetPrefab(ETowerPokemonName towerPokemonName)
    {
        GameObject prefab = null;
        prefab = Resources.Load<GameObject>("Prefabs/" + towerPokemonName);

        if(prefab == null){
            Debug.LogError("프리팹이 없습니다 ㅠㅠ");
        }

        return prefab;
    }

    public static UnitDataObject GetUnitDataObject(EPokemonName pokemon)
    {
        UnitDataObject unitData = null;
        unitData = Resources.Load<UnitDataObject>("ScriptableObject/" + pokemon);

        if (unitData == null)
        {
            Debug.LogError("유닛 데이터가 없습니다 ㅠㅠ");
        }

        return unitData;
    }

    public static UnitDataObject GetUnitDataObject(ETowerPokemonName towerPokemonName)
    {
        UnitDataObject unitData = null;
        unitData = Resources.Load<UnitDataObject>("ScriptableObject/" + towerPokemonName);

        if (unitData == null)
        {
            Debug.LogError("유닛 데이터가 없습니다 ㅠㅠ");
        }

        return unitData;
    }
}
