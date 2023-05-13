using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphManager : MonoBehaviour
{

    //----------------------------------------------- fields
    [SerializeField]
    LineRenderer lineRenderer_Cp;

    [SerializeField]
    Camera graphCam_Cp;

    [SerializeField]
    int lineRendPosCount = 2000;

    [SerializeField]
    float graphCamSize = 5, minXValue = -100f, maxXValue = 100f, minYValue = -100f, maxYValue = 100f;

    [SerializeField]
    TMP_InputField[] formula_tmpIF_Cps;

    [SerializeField]
    TMP_Text[] zeroPointText_Cp;

    [SerializeField]
    GameObject[] zeroPoint_GOs;

    [SerializeField]
    GameObject[] formula_GOs;

    [SerializeField]
    float stepAmount = 0.03f;

    [SerializeField]
    Animator[] formulaAnim_Cp;

    int currentFieldPageIndex = 0;

    //----------------------------------------------- properties
    float ratioX
    {
        get { return (maxXValue - minXValue) / lineRenderer_Cp.positionCount; }
    }

    List<float> curCoefList
    {
        get
        {
            List<float> value = new List<float>();

            int firstTurn = 0, lastTurn = 0;
            if(currentFieldPageIndex == 0)
            {
                firstTurn = 0;
                lastTurn = 1;
            } else if(currentFieldPageIndex == 1)
            {
                firstTurn = 2;
                lastTurn = 4;
            } else if(currentFieldPageIndex == 2)
            {
                firstTurn = 5;
                lastTurn = 8;
            }
            for(int j = firstTurn; j <= lastTurn; j++)
            {
                value.Add(float.Parse(formula_tmpIF_Cps[j].text));
            }

            return value;
        }
    }

    //----------------------------------------------- methods
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Init()
    {
        lineRenderer_Cp.positionCount = lineRendPosCount;
        
        DrawGraph();

        formulaAnim_Cp[currentFieldPageIndex].SetBool("flag", true);
    }

    public void DrawGraph()
    {
        float[] coefficients = curCoefList.ToArray();
        float x = minXValue,  y = 0f;

        for(int i = 0; i < lineRenderer_Cp.positionCount; i++)
        {
            for(int j = coefficients.Length - 1; j >= 0; j--)
            {
                y += coefficients[j] * Mathf.Pow(x, (coefficients.Length - 1 - j));
            }

            // print("ratio = " + 0.75f / ((float)Screen.height / (float)Screen.width));

            float lineX = x / maxXValue * graphCamSize;
            float lineY = y / maxYValue * graphCamSize;
            lineRenderer_Cp.SetPosition(i, new Vector3(lineX, lineY, 0f));

            x += ratioX;
            y = 0f;
        }

        SetZeroPointNotification();

        SetZeroPointOnGraph();
    }

    List<float> GetZeroPoints()
    {
        List<float> value = new List<float>();

        if(currentFieldPageIndex == 0)
        {
            float a = curCoefList[0];
            float b = curCoefList[1];
            float x = -1f * b / a;
            value.Add(x);
        }
        else if(currentFieldPageIndex == 1)
        {
            float a = curCoefList[0];
            float b = curCoefList[1];
            float c = curCoefList[2];
            float b24ac = Mathf.Pow(b, 2) - 4f * a * c;
            if(b24ac >= 0f)
            {
                float x1 = ((-1f * b - Mathf.Sqrt(b24ac)) / 2f) / a;
                float x2 = ((-1f * b + Mathf.Sqrt(b24ac)) / 2f) / a;
                value.Add(x1);
                value.Add(x2);
            }
        }
        else if(currentFieldPageIndex == 2)
        {
            float x = minXValue,  y = 0f;

            float oldX = 0f, oldY = 0f, zeroX = 0f;
            for(int i = 0; i < lineRenderer_Cp.positionCount; i++)
            {
                for(int j = curCoefList.Count - 1; j >= 0; j--)
                {
                    y += curCoefList[j] * Mathf.Pow(x, (curCoefList.Count - 1 - j));
                }

                if(Mathf.Abs(y) <= 0.00001f)
                {
                    value.Add(x);
                }
                else if((oldY < 0f && y > 0f) || (oldY > 0f && y < 0f))
                {
                    zeroX = x;
                    value.Add(zeroX);
                }

                float lineX = x / maxXValue * graphCamSize;
                float lineY = y / maxYValue * graphCamSize;
                lineRenderer_Cp.SetPosition(i, new Vector3(lineX, lineY, 0f));

                oldX = x;
                oldY = y;
                x += ratioX;
                y = 0f;
            }
        }

        return value;
    }

    void SetZeroPointNotification()
    {
        List<float> curZeroPoints = GetZeroPoints();

        zeroPointText_Cp[currentFieldPageIndex].text = "<b>zero of quadratic" + "\n" + "polynomial:</b>";

        foreach(float value_tp in curZeroPoints)
        {
            zeroPointText_Cp[currentFieldPageIndex].text += ("\n" + "(" +
                (Mathf.Round(value_tp * 10000f) / 10000f).ToString() + ", 0)");
        }
    }

    void SetZeroPointOnGraph()
    {
        List<float> curZeroPoints = GetZeroPoints();

        for(int i = 0; i < zeroPoint_GOs.Length; i++)
        {
            if(i >= curZeroPoints.Count)
            {
                zeroPoint_GOs[i].SetActive(false);
            }
            else
            {
                zeroPoint_GOs[i].SetActive(true);
            }
        }

        for(int i = 0; i < curZeroPoints.Count; i++)
        {
            zeroPoint_GOs[i].GetComponent<RectTransform>().anchorMin =
                new Vector2(curZeroPoints[i] / (maxXValue - minXValue) + 0.5f, 0.5f);
            zeroPoint_GOs[i].GetComponent<RectTransform>().anchorMax =
                zeroPoint_GOs[i].GetComponent<RectTransform>().anchorMin;

            zeroPoint_GOs[i].transform.Find("Value Text (TMP)").GetComponent<TMP_Text>().text =
                (Mathf.Round(curZeroPoints[i] * 10000f) / 10000f).ToString();
        }
    }

    //----------------------------------------------- callback from UI
    public void OnChangeCoefficients(int coefficientID_pr)
    {
        List<float> coefList = new List<float>();

        int firstTurn = 0, lastTurn = 0;
        if(coefficientID_pr >= 0 && coefficientID_pr <= 1)
        {
            firstTurn = 0;
            lastTurn = 1;
        } else if(coefficientID_pr >= 2 && coefficientID_pr <= 4)
        {
            firstTurn = 2;
            lastTurn = 4;
        } else if(coefficientID_pr >= 5 && coefficientID_pr <= 8)
        {
            firstTurn = 5;
            lastTurn = 8;
        }
        for(int j = firstTurn; j <= lastTurn; j++)
        {
            if(j == firstTurn)
            {
                float outValue = 0f;
                float.TryParse(formula_tmpIF_Cps[firstTurn].text, out outValue);
                if(outValue == 0f)
                {
                    if(coefficientID_pr != firstTurn)
                    {
                        formula_tmpIF_Cps[firstTurn].text = 1f.ToString();
                    }
                    else
                    {
                        return;
                    }
                }
            }

            float outValue2;
            if(float.TryParse(formula_tmpIF_Cps[j].text, out outValue2))
            {
                coefList.Add(outValue2);
            }
            else
            {
                return;
            }
        }

        // DrawGraph(coefList.ToArray());
        DrawGraph();
    }

    public void OnPlusCoefficients(int coefficientID_pr, float step = 0f)
    {
        float oldValue = 0f;
        float.TryParse(formula_tmpIF_Cps[coefficientID_pr].text, out oldValue);
        formula_tmpIF_Cps[coefficientID_pr].text = (oldValue + step * stepAmount).ToString();

        OnChangeCoefficients(coefficientID_pr);
    }

    public void OnClickNextFormula()
    {
        StartCoroutine(CorouOnClickNextFormula());
    }

    IEnumerator CorouOnClickNextFormula()
    {
        int nextFieldPageIndex = 0;
        if(currentFieldPageIndex == 2)
        {
            nextFieldPageIndex = 0;
        }
        else
        {
            nextFieldPageIndex = currentFieldPageIndex + 1;
        }

        formulaAnim_Cp[currentFieldPageIndex].SetBool("flag", false);
        formulaAnim_Cp[nextFieldPageIndex].SetBool("flag", true);
        yield return new WaitForSeconds(Time.deltaTime);

        currentFieldPageIndex = nextFieldPageIndex;

        DrawGraph();
    }

    // public void OnMinusCoefficients(int coefficientID_pr)
    // {
    //     float oldValue = float.Parse(formula_tmpIF_Cps[coefficientID_pr].text);
    //     formula_tmpIF_Cps[coefficientID_pr].text = (oldValue - 1f).ToString();

    //     OnChangeCoefficients(coefficientID_pr);
    // }
}
