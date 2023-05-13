using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Concept1 : MonoBehaviour
{
    //---------------------------------------------------------------------- fields
    [SerializeField]
    Controller controller_Cp;
    
    [SerializeField]
    Animator resultAnim_Cp;

    [SerializeField]
    GameObject solutionPanel_GO;

    [SerializeField]
    int correctSolutionIndex = 0;

    [SerializeField]
    Transform problemOrderPanel_Tf;

    [SerializeField]
    TMP_Text evaluateBtnText_Cp;

    [SerializeField]
    TMP_Text evaluateFalseText_Cp;

    [SerializeField]
    GameObject coordinateSearchersPanel_GO;

    int solutionIndex_pr;

    int curProblemOrder = 0;

    string m_evaluateBtnState;
    
    //---------------------------------------------------------------------- properties
    Graph graph_Cp
    {
        get { return controller_Cp.graph_Cp; }
    }

    List<Image> problemOrderImage_Cps
    {
        get
        {
            List<Image> value = new List<Image>();

            for(int i = 0; i < problemOrderPanel_Tf.childCount; i++)
            {
                value.Add(problemOrderPanel_Tf.GetChild(i).GetComponent<Image>());
            }

            return value;
        }
    }

    string evaluateBtnState
    {
        get { return m_evaluateBtnState; }
        set
        {
            m_evaluateBtnState = value;

            switch(value)
            {
            case "TryAgain":
                evaluateBtnText_Cp.text = "Try again";
                break;
            case "Next":
                evaluateBtnText_Cp.text = "Next example";
                break;
            case "Restart":
                evaluateBtnText_Cp.text = "Restart";
                InitProblemOrders();
                break;
            case "CheckAnswer":
                evaluateBtnText_Cp.text = "Check answer";
                break;
            }
        }
    }

    //---------------------------------------------------------------------- methods
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Init
    public void Init()
    {
        SubmitProblem();
        
        evaluateBtnState = "CheckAnswer";
    }

    // Init problem order image colors
    void InitProblemOrders()
    {
        for(int i = 0; i < problemOrderImage_Cps.Count; i++)
        {
            problemOrderImage_Cps[i].color = new Color(1f, 1f, 1f, 1f);
        }
    }

    // Init coordinate searchers
    void InitCoordinateSearchers()
    {
        foreach(CoordinateSearcher value_tp in coordinateSearchersPanel_GO
            .GetComponentsInChildren<CoordinateSearcher>())
        {
            value_tp.InitPosition();
        }
    }

    // Set problem order
    void SetProblemOrder(int index, int correctAnswerFlag)
    {
        if(correctAnswerFlag == 1)
        {
            problemOrderImage_Cps[index].color = new Color(0f, 1f, 0f, 1f);
        }
        else if(correctAnswerFlag == 2)
        {
            problemOrderImage_Cps[index].color = new Color(1f, 0f, 0f, 1f);
        }

        curProblemOrder++;
        if(curProblemOrder == problemOrderImage_Cps.Count)
        {
            curProblemOrder = 0;
        }
    }

    // Get random polynomial
    List<float> GetRandomPolynomial(int degreeMin, int degreeMax)
    {
        List<float> value = new List<float>();

        int degree = Random.Range(degreeMin, degreeMax + 1);
        for(int i = 0; i < degree; i++)
        {
            float randCoefficient = (float)Random.Range(-2f, 2f);
            value.Add(randCoefficient);
        }
        if(value[0] == 0f)
        {
            value[0] = 1f;
        }
        correctSolutionIndex = degree - 1;

        return value;
    }

    // Check result
    void CheckResult(int solutionIndex_pr)
    {
        StartCoroutine(CorouCheckResult(solutionIndex_pr));
    }

    IEnumerator CorouCheckResult(int solutionIndex_pr)
    {
        if(correctSolutionIndex == solutionIndex_pr)
        {
            resultAnim_Cp.SetInteger("flag", 1);

            if(evaluateBtnState == "CheckAnswer")
            {
                SetProblemOrder(curProblemOrder, 1);
            }
            else if(evaluateBtnState == "TryAgain")
            {
                SetProblemOrder(curProblemOrder, 2);
            }

            if(curProblemOrder == 0)
            {
                evaluateBtnState = "Restart";
            }
            else
            {
                evaluateBtnState = "Next";
            }
        }
        else
        {
            switch(correctSolutionIndex)
            {
            case 1:
                evaluateFalseText_Cp.text = "Straight line is linear polynomial";
                break;
            case 2:
                evaluateFalseText_Cp.text = "Symmetrical curve is quadratic polynomial";
                break;
            case 3:
                evaluateFalseText_Cp.text = "Asymmetrical curve's degree is larger than 2";
                break;
            case 4:
                evaluateFalseText_Cp.text = "";
                break;
            }

            resultAnim_Cp.SetInteger("flag", -1);

            evaluateBtnState = "TryAgain";
        }

        yield return null;
    }

    // reset solutions
    void ResetSolutions()
    {
        Toggle[] solutionToggle_Cps = solutionPanel_GO.GetComponentsInChildren<Toggle>();
        for(int i = 0; i < solutionToggle_Cps.Length; i++)
        {
            solutionToggle_Cps[i].isOn = false;
        }
    }

    //------------------------------------------------ public methods
    // Submit problem
    public void SubmitProblem()
    {
        List<float> randPolynomial = GetRandomPolynomial(2, 4);

        graph_Cp.DrawGraph(randPolynomial.ToArray());

        graph_Cp.SetZeroPointsOnGraph(true, randPolynomial.ToArray());
    }

    // Callback from UI
    public void OnClickEvaluate()
    {
        int solutionIndex_pr = 0;

        Toggle[] solutionToggle_Cps = solutionPanel_GO.GetComponentsInChildren<Toggle>();
        for(int i = 0; i < solutionToggle_Cps.Length; i++)
        {
            if(solutionToggle_Cps[i].isOn)
            {
                solutionIndex_pr = i + 1;
                break;
            }
        }

        if(evaluateBtnState == "Next" || evaluateBtnState == "Restart")
        {
            OnClickNextProblem();
        }
        else if(evaluateBtnState == "CheckAnswer" || evaluateBtnState == "TryAgain")
        {
            CheckResult(solutionIndex_pr);
        }
    }

    // Next problem
    public void OnClickNextProblem()
    {
        resultAnim_Cp.SetInteger("flag", 0);

        ResetSolutions();

        InitCoordinateSearchers();
        
        SubmitProblem();

        evaluateBtnState = "CheckAnswer";
    }

    public void OnClickInitCoordinateSearcher()
    {
        InitCoordinateSearchers();
    }

}
