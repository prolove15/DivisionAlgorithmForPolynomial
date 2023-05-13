using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Concept2 : MonoBehaviour
{
    //---------------------------------------------------------------------- fields
    [SerializeField]
    Controller controller_Cp;

    [SerializeField]
    Animator resultAnim_Cp;

    [SerializeField]
    GameObject solutionPanel_GO;

    [SerializeField]
    GameObject ballsPanel_GO, coordinateSearchersPanel_GO;

    [SerializeField]
    Transform problemOrderPanel_Tf;

    [SerializeField]
    TMP_Text evaluateBtnText_Cp;

    int lastSolutionPointIndex;

    int curProblemOrder = 0;

    string m_evaluateBtnState;
    
    bool showPointValueFlag = true;

    //---------------------------------------------------------------------- properties
    Graph graph_Cp
    {
        get { return controller_Cp.graph_Cp; }
    }

    List<int> solutionPointIndexes
    {
        get
        {
            List<int> value = new List<int>();

            for(int i = 0; i <= lastSolutionPointIndex; i++)
            {
                value.Add(i);
            }

            return value;
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

    // SubmitProblem
    void SubmitProblem()
    {
        float[] randPolynomial = GetRandomPolynomial().ToArray();

        // draw graph
        graph_Cp.DrawGraph(randPolynomial);

        // set points on graph
        Dictionary<float, float> randGraphPoints = GetRandomGraphPoints(randPolynomial);
        graph_Cp.SetPointsOnGraph(randGraphPoints, true, true);
        
        // init solution panel
        ShowSolutionPanel(false);
    }

    // Get random polynomial
    List<float> GetRandomPolynomial(int degreeMin = 2, int degreeMax = 4)
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

        return value;
    }

    // Get random graph points
    Dictionary<float, float> GetRandomGraphPoints(params float[] coefficients)
    {
        Dictionary<float, float> value = new Dictionary<float, float>();

        // get zero points x
        List<float> zeroPointsX = graph_Cp.GetZeroPoints(coefficients);
        for(int i = 0; i < zeroPointsX.Count; i++)
        {
            float yFromX = graph_Cp.GetYFromX(zeroPointsX[i], coefficients);
            
            if((zeroPointsX[i] <= graph_Cp.maxXValue && zeroPointsX[i] >= graph_Cp.minXValue ) &&
                (yFromX <= graph_Cp.maxYValue && yFromX >= graph_Cp.minYValue))
            {
                value[zeroPointsX[i]] = 0f;
            }
        }

        // set last solution index
        lastSolutionPointIndex = value.Count - 1;

        // get y when x = 0
        float yWhenXisZero = graph_Cp.GetYFromX(0f, coefficients);
        if(yWhenXisZero <= graph_Cp.maxYValue && yWhenXisZero >= graph_Cp.minYValue)
        {
            if(!zeroPointsX.Contains(0f))
            {
                value[0f] = yWhenXisZero;
            }
        }

        // complete random value
        while(value.Count < 5)
        {
            float randX = Random.Range(graph_Cp.minXValue, graph_Cp.maxXValue);
            float yFromX = graph_Cp.GetYFromX(randX, coefficients);

            if(yFromX > graph_Cp.maxYValue || yFromX < graph_Cp.minYValue)
            {
                continue;
            }

            bool nearestExist = false;
            foreach(float i in value.Keys)
            {
                if(Vector2.Distance(new Vector2(i, graph_Cp.GetYFromX(i)), new Vector2(randX, yFromX))
                    < 0.1f)
                {
                    nearestExist = true;
                    break;
                }
            }
            if(nearestExist)
            {
                continue;
            }

            value[randX] = yFromX;
        }

        return value;
    }

    // show selected graph points on solution panel
    void ShowSolutionPanel(bool flag = true)
    {
        TMP_Text[] solutionText_Cps_tp = solutionPanel_GO.GetComponentsInChildren<TMP_Text>();
        solutionPanel_GO.GetComponentInChildren<TMP_Text>().text = string.Empty;

        if(flag && showPointValueFlag)
        {
            foreach(string graphPointText_tp in graph_Cp.GetSelectedGraphPointTexts())
            {
                solutionPanel_GO.GetComponentInChildren<TMP_Text>().text += graphPointText_tp + "\n";
            }
        }
    }

    // evaluate result
    void EvaluateResult()
    {
        StartCoroutine(CorouEvaluateResult());
    }

    IEnumerator CorouEvaluateResult()
    {
        List<int> selectedGraphPoints = graph_Cp.GetSelectedGraphPoints();

        bool evaluate = true;
        for(int i = 0; i < solutionPointIndexes.Count; i++)
        {
            bool existSolution = false;
            for(int j = 0; j < selectedGraphPoints.Count; j++)
            {
                if(solutionPointIndexes[i] == selectedGraphPoints[j])
                {
                    existSolution = true;
                    break;
                }
            }

            if(!existSolution)
            {
                evaluate = false;
                break;
            }
        }
        if(solutionPointIndexes.Count != selectedGraphPoints.Count)
        {
            evaluate = false;
        }

        // action when finished evaluate
        if(evaluate)
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
            resultAnim_Cp.SetInteger("flag", -1);

            evaluateBtnState = "TryAgain";
        }

        yield return null;
    }

    // Init ball positions
    void InitBallPositions()
    {
        foreach(BallDragDrop ball_Cp_tp in ballsPanel_GO.GetComponentsInChildren<BallDragDrop>())
        {
            ball_Cp_tp.InitPosition();
        }
    }

    //------------------------------------------------ public methods
    // called when clicked evaluate button
    public void OnClickEvaluate()
    {
        if(evaluateBtnState == "Next" || evaluateBtnState == "Restart")
        {
            OnClickNextProblem();
        }
        else if(evaluateBtnState == "CheckAnswer" || evaluateBtnState == "TryAgain")
        {
            EvaluateResult();
        }
    }

    public void OnClickGraphPoint()
    {
        ShowSolutionPanel(true);
    }

    public void OnClickToggleGraphPointValue(bool flag)
    {
        showPointValueFlag = flag;

        ShowSolutionPanel(flag);
    }

    // Next problem
    public void OnClickNextProblem()
    {
        resultAnim_Cp.SetInteger("flag", 0);

        SubmitProblem();

        InitBallPositions();

        InitCoordinateSearchers();

        evaluateBtnState = "CheckAnswer";
    }

    public void OnClickInitCoordinateSearcher()
    {
        InitCoordinateSearchers();
    }

    public void OnClickInitBallPositions()
    {
        InitBallPositions();
    }
}
