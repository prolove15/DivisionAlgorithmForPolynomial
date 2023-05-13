using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Concept4 : MonoBehaviour
{

    //---------------------------------------------------------------------- fields
    [SerializeField]
    Controller controller_Cp;

    [SerializeField]
    int[] a, b, c, a1, b1, c1, d1;

    [SerializeField]
    Toggle[] solutionToggle_Cps;

    [SerializeField]
    TMP_Text[] coefficientText_Cps;

    [SerializeField]
    Text[] solutionText_Cps;

    [SerializeField]
    Animator evaluateAnim_Cp;

    [SerializeField]
    GameObject coordinateSearchersPanel_GO;

    [SerializeField]
    TMP_Text evaluateBtnText_Cp;

    [SerializeField]
    Transform problemOrderPanel_Tf;

    int correctAnswerIndex = -1;

    int pageIndex = 0;

    string m_evaluateBtnState;

    //---------------------------------------------------------------------- properties
    Graph graph_Cp
    {
        get { return controller_Cp.graph_Cp; }
    }

    float[] curCoefficients
    {
        get { return new float[]{(float)a[pageIndex], (float)b[pageIndex], (float)c[pageIndex]}; }
    }

    float[] correctSolutions
    {
        get { return new float[]{(float)a1[pageIndex], (float)b1[pageIndex], (float)c1[pageIndex], (float)d1[pageIndex]}; }
    }

    // float[] curSolutions
    // {
    //     get
    //     {
    //         float[] value = new float[solutionInputF_Cps.Length];

    //         for(int i = 0; i < solutionInputF_Cps.Length; i++)
    //         {
    //             float curSolution_tp = 0f;
    //             if(!float.TryParse(solutionInputF_Cps[i].text, out curSolution_tp))
    //             {
    //                 solutionInputF_Cps[i].text = 0f.ToString();
    //                 curSolution_tp = 0f;
    //             }

    //             value[i] = curSolution_tp;
    //         }
    //         if(value[0] == 0f)
    //         {
    //             solutionInputF_Cps[0].text = 1f.ToString();

    //             value[0] = 1f;
    //         }

    //         return value;
    //     }
    // }

    bool evaulationResult
    {
        get
        {
            bool result = false;

            if(solutionToggle_Cps[correctAnswerIndex].isOn)
            {
                result = true;
            }

            return result;
        }
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
        evaluateBtnState = "CheckAnswer";

        InitAllCoefficient();

        SubmitProblem();
    }

    // Init all coefficients
    void InitAllCoefficient()
    {
        int problemNum = problemOrderImage_Cps.Count;

        a = new int[problemNum];
        b = new int[problemNum];
        c = new int[problemNum];
        a1 = new int[problemNum];
        b1 = new int[problemNum];
        c1 = new int[problemNum];
        d1 = new int[problemNum];
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

    // Submit problem
    void SubmitProblem()
    {
        // generate random problem
        GenerateRandomProblem();

        // draw graph
        float[] coefficients_tp = new float[]{(float)a[pageIndex], (float)b[pageIndex], (float)c[pageIndex]};
        graph_Cp.DrawGraph(coefficients_tp);
        graph_Cp.SetZeroPointsOnGraph(false, coefficients_tp);

        // set coefficient
        coefficientText_Cps[0].text = a[pageIndex].ToString() + "x";

        coefficientText_Cps[1].text = string.Empty;
        if(b[pageIndex] >= 0)
        {
            coefficientText_Cps[1].text = "+";
        }
        coefficientText_Cps[1].text += b[pageIndex].ToString() + "x";

        coefficientText_Cps[2].text = string.Empty;
        if(c[pageIndex] >= 0)
        {
            coefficientText_Cps[2].text = "+";
        }
        coefficientText_Cps[2].text += c[pageIndex].ToString();

        // init solution input field
        List<string> randSolutionTexts = GetRandomSolution();
        for(int i = 0; i < solutionText_Cps.Length; i++)
        {
            solutionText_Cps[i].text = randSolutionTexts[i];
        }
        for(int i = 0; i < solutionToggle_Cps.Length; i++)
        {
            solutionToggle_Cps[i].isOn = false;
        }
    }

    // generate random problem
    void GenerateRandomProblem()
    {
        for (int i = 0; i < a.Length; i++)
        {
            a1[i] = Random.Range(1, 3);
            b1[i] = Random.Range(-3, 4);
            c1[i] = Random.Range(1, 3);
            d1[i] = Random.Range(-3, 4);
            a[i] = a1[i] * c1[i];
            b[i] = a1[i] * d1[i] + b1[i] * c1[i];
            c[i] = b1[i] * d1[i];
        }
    }

    // Generate random solution
    List<string> GetRandomSolution()
    {
        List<string> value = new List<string>();

        correctAnswerIndex = Random.Range(0, solutionText_Cps.Length);

        for(int i = 0; i < solutionText_Cps.Length; i++)
        {
            string randText = string.Empty;

            if(i == correctAnswerIndex)
            {
                randText = "a=" + correctSolutions[0].ToString() + ", b=" + correctSolutions[1].ToString()
                    + ", c=" + correctSolutions[2].ToString() + ", d=" + correctSolutions[3].ToString();
            }
            else
            {
                randText = "a=" + Random.Range(1, 3).ToString() + ", b=" + Random.Range(-3, 4).ToString()
                    + ", c=" + Random.Range(1, 3).ToString() + ", d=" + Random.Range(-3, 4).ToString();
            }

            value.Add(randText);
        }

        return value;
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

        pageIndex++;
        if(pageIndex == problemOrderImage_Cps.Count)
        {
            pageIndex = 0;
        }
    }

    // Evaulate
    void Evaluate()
    {
        StartCoroutine(CorouEvaluate());
    }

    IEnumerator CorouEvaluate()
    {
        if(evaulationResult)
        {
            evaluateAnim_Cp.SetInteger("flag", 1);

            if(evaluateBtnState == "CheckAnswer")
            {
                SetProblemOrder(pageIndex, 1);
            }
            else if(evaluateBtnState == "TryAgain")
            {
                SetProblemOrder(pageIndex, 2);
            }

            if(pageIndex == 0)
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
            evaluateAnim_Cp.SetInteger("flag", -1);

            evaluateBtnState = "TryAgain";
        }

        yield return null;
    }

    //----------------------------------------------- Callback from UI
    // called when clicked evaluate button
    public void OnClickEvaluate()
    {
        if(evaluateBtnState == "Next" || evaluateBtnState == "Restart")
        {
            OnClickNextProblem();
        }
        else if(evaluateBtnState == "CheckAnswer" || evaluateBtnState == "TryAgain")
        {
            Evaluate();
        }
    }

    // Next problem
    public void OnClickNextProblem()
    {
        evaluateAnim_Cp.SetInteger("flag", 0);

        SubmitProblem();

        evaluateBtnState = "CheckAnswer";
    }

    public void OnClickInitCoordinateSearcher()
    {
        InitCoordinateSearchers();
    }

}
