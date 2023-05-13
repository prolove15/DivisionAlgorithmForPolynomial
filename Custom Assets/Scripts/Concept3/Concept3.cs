using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Concept3 : MonoBehaviour
{

    //---------------------------------------------------------------------- fields
    [SerializeField]
    Controller controller_Cp;

    [SerializeField]
    TMP_InputField[] linearInputF_Cps, quadraticInputF_Cps;

    [SerializeField]
    TMP_Text[] linearSolutionText_Cp, quadraticSolutionText_Cps;

    [SerializeField]
    Animator[] movePageAnim_Cps;

    [SerializeField]
    float plusStepAmount = 0.03f;

    [SerializeField]
    int curPageIndex = 0;

    [SerializeField]
    GameObject coordinateSearchersPanel_GO;

    //---------------------------------------------------------------------- properties
    Graph graph_Cp
    {
        get { return controller_Cp.graph_Cp; }
    }

    TMP_InputField[] curPolynomialInputF_Cp
    {
        get
        {
            if(curPageIndex == 0)
            {
                return linearInputF_Cps;
            }
            else if(curPageIndex == 1)
            {
                return quadraticInputF_Cps;
            }

            return null;
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
        movePageAnim_Cps[curPageIndex].SetBool("flag", true);
        
        RedrawGraph();
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

    void RedrawGraph()
    {
        graph_Cp.DrawGraph(GetCoefficients(curPolynomialInputF_Cp).ToArray());

        graph_Cp.SetZeroPointsOnGraph(false, GetCoefficients(curPolynomialInputF_Cp).ToArray());

        // set zero points
        float[] coefficients = GetCoefficients(curPolynomialInputF_Cp).ToArray();
        List<float> zeroPoints = graph_Cp.GetZeroPoints(coefficients);

        if(curPageIndex == 0)
        {
            SetLinearSolution(string.Empty);

            if(zeroPoints.Count == 1)
            {
                SetLinearSolution(zeroPoints[0].ToString());
            }
        }
        else if(curPageIndex == 1)
        {
            SetQuadraticSolution(0, string.Empty);
            SetQuadraticSolution(1, string.Empty);

            if(zeroPoints.Count == 0)
            {
                SetQuadraticSolution(0, "α does not exist");
                SetQuadraticSolution(1, "β does not exist");
            }
            else if(zeroPoints.Count == 1)
            {
                SetQuadraticSolution(0, "α=" + zeroPoints[0].ToString());
                SetQuadraticSolution(1, "β does not exist");
            }
            else if(zeroPoints.Count == 2)
            {
                SetQuadraticSolution(0, "α=" + zeroPoints[0].ToString());
                SetQuadraticSolution(1, "β=" + zeroPoints[1].ToString());
            }
        }
    }

    void SetLinearSolution(string value_pr)
    {
        int solutionLength = Mathf.Clamp(value_pr.Length, 0, 7);

        linearSolutionText_Cp[0].text = value_pr.Substring(0, solutionLength);
    }

    void SetQuadraticSolution(int index, string value_pr)
    {
        int solutionLength = value_pr.Length;

        float value_tp = 0f;
        // if(float.TryParse(value_pr, out value_tp))
        // {
        //     solutionLength = Mathf.Clamp(value_pr.Length, 0, 7);
        // }

        quadraticSolutionText_Cps[index].text = value_pr.Substring(0, solutionLength);
    }

    List<float> GetCoefficients(TMP_InputField[] targetInputF_Cp_pr)
    {
        List<float> value = new List<float>();

        // check validate
        for(int i = 0; i < targetInputF_Cp_pr.Length; i++)
        {
            float coef_tp = 0f;
            if(!float.TryParse(targetInputF_Cp_pr[i].text, out coef_tp))
            {
                targetInputF_Cp_pr[i].text = 1f.ToString();
            }
        }
        if(float.Parse(targetInputF_Cp_pr[0].text) == 0f)
        {
            targetInputF_Cp_pr[0].text = 1f.ToString();
        }

        // get coefficients of polynomial
        for(int i = 0; i < targetInputF_Cp_pr.Length; i++)
        {
            value.Add(float.Parse(targetInputF_Cp_pr[i].text));
        }

        return value;
    }

    // Move to next page
    void MoveToNextPage()
    {
        StartCoroutine(CorouMoveToNextPage());
    }

    IEnumerator CorouMoveToNextPage()
    {
        movePageAnim_Cps[curPageIndex].SetBool("flag", false);

        curPageIndex++;
        if(curPageIndex == 2)
        {
            curPageIndex = 0;
        }
        movePageAnim_Cps[curPageIndex].SetBool("flag", true);

        yield return new WaitUntil(() => movePageAnim_Cps[curPageIndex].transform.localScale == Vector3.one);
        
        RedrawGraph();
    }

    //------------------------------------------------ Callback from UI
    public void OnEndEditCoefficients()
    {
        // redraw graph
        RedrawGraph();
    }

    // public void OnEndEditQuadraticCoefficients()
    // {
    //     // draw graph
    //     float[] coefficients = GetCoefficients(quadraticInputF_Cps).ToArray();

    //     graph_Cp.DrawGraph(coefficients);

    //     // set zero points
    //     List<float> zeroPoints = graph_Cp.GetZeroPoints(coefficients);

    //     string alphaSolutionText = string.Empty, betaSolutionText = string.Empty;
        
    //     for(int i = 0; i < zeroPoints.Count; i++)
    //     {
    //         switch(i)
    //         {
    //             case 0:
    //                 alphaSolutionText = zeroPoints[0].ToString();
    //                 break;
    //             case 1:
    //                 alphaSolutionText = zeroPoints[0].ToString();
    //                 betaSolutionText = zeroPoints[1].ToString();
    //                 break;
    //         }
    //     }

    //     SetQuadraticAlphaSolution(alphaSolutionText);
    //     SetQuadraticBetaSolution(betaSolutionText);
    // }

    public void OnPlusMinusCoefficients(int coefficientID_pr, float flag)
    {
        if(coefficientID_pr >= 0 && coefficientID_pr <= 1)
        {
            float oldValue = float.Parse(linearInputF_Cps[coefficientID_pr].text);
            float newValue = oldValue + plusStepAmount * flag;
            if(newValue == 0f)
            {
                newValue += + plusStepAmount * flag;
            }

            linearInputF_Cps[coefficientID_pr].text = newValue.ToString();
        }
        else if(coefficientID_pr >= 2 && coefficientID_pr <= 4)
        {
            coefficientID_pr -= 2;
            float oldValue = float.Parse(quadraticInputF_Cps[coefficientID_pr].text);
            float newValue = oldValue + plusStepAmount * flag;
            if(newValue == 0f)
            {
                newValue += plusStepAmount * flag;
            }

            quadraticInputF_Cps[coefficientID_pr].text = newValue.ToString();
        }

        RedrawGraph();
    }

    public void OnClickNextPage()
    {
        MoveToNextPage();
        
        InitCoordinateSearchers();
    }

    public void OnClickInitCoordinateSearcher()
    {
        InitCoordinateSearchers();
    }

}
