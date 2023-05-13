using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{

    //----------------------------------------------- fields
    [SerializeField]
    BgdManager bgdManager_Cp;

    [SerializeField]
    int sceneID;

    public GraphManager graphManager_Cp;

    public Graph graph_Cp;

    public Concept1 concept1_Cp;

    public Concept2 concept2_Cp;

    public Concept3 concept3_Cp;

    public Concept4 concept4_Cp;

    //----------------------------------------------- methods
    void Awake()
    {
        // if(Application.platform == RuntimePlatform.WindowsPlayer)
        // {
        //     Screen.SetResolution(1024, 768, true);
        // }
        // else
        // {
        //     Screen.SetResolution(1024, 768, false);
        // }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // // Update is called once per frame
    void Update()
    {
        
    }

    void Init()
    {
        StartCoroutine(CorouInit());
    }

    IEnumerator CorouInit()
    {
        bgdManager_Cp.CurtainUp();
        yield return new WaitUntil(() => bgdManager_Cp.curtainAnim_Cp.transform.localScale.x == 0f);

        if(sceneID == 0)
        {
            graphManager_Cp.Init();
        }
        else if(sceneID == 1)
        {
            concept1_Cp.Init();
        }
        else if(sceneID == 2)
        {
            concept2_Cp.Init();
        }
        else if(sceneID == 3)
        {
            concept3_Cp.Init();
        }
        else if(sceneID == 4)
        {
            concept4_Cp.Init();
        }
    }

    public void OnClickQuit()
    {
        StartCoroutine(CorouOnClickQuit());
    }

    IEnumerator CorouOnClickQuit()
    {
        bgdManager_Cp.CurtainDown();
        yield return new WaitUntil(() => bgdManager_Cp.curtainAnim_Cp.gameObject.GetComponent<Image>().color.a == 1f);
        
        QuitGame();
    }

    void QuitGame()
    {
        Application.Quit();
    }
}
