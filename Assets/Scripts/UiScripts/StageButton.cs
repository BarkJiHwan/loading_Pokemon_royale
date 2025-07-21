using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    Button button;



    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();

        if(gameObject.name == "Stage1")
        {
            button.onClick.AddListener(GameObject.Find("SceneLoader").GetComponent<SceneLoader>().LoadStageOne);
        }
        else if(gameObject.name == "Stage2")
        {
            button.onClick.AddListener(GameObject.Find("SceneLoader").GetComponent<SceneLoader>().LoadStageTwe);
        }
    }

}
