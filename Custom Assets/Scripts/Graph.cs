using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Graph : MonoBehaviour
{
    //---------------------------------------------------------------------- fields
    // SerializeField
    [Tooltip("Camera to be displayed the graph")]
    [SerializeField]
    public Camera camGraph_Cp;

    [Tooltip("LineRenderer component for describe graph line")]
    [SerializeField]
    LineRenderer lineRend_Cp;

    [SerializeField]
    GameObject graphPoint_Pf;

    [SerializeField]
    Transform graphPointHolder_Tf;

    [SerializeField]
    int lineRendPosCount = 1000;

    [SerializeField]
    float screenWidthBase = 1024f, screenHeightBase = 768f, camWidthBase = 0.6f, camHeightBase = 0.8f;
    
    [SerializeField]
    public float clingDist = 0.1f;

    bool showPointValueFlag = true;

    float[] curCoefficients;

    // public fields
    public List<GameObject> graphPoint_GOs = new List<GameObject>();
    
    //---------------------------------------------------------------------- properties
    Controller controller_Cp
    {
        get { return GameObject.FindWithTag("GameController").GetComponent<Controller>(); }
    }

    Concept2 concept2
    {
        get { return controller_Cp.concept2_Cp; }
    }
    
    float ratioX
    {
        get { return (maxXValue - minXValue) / lineRend_Cp.positionCount; }
    }

    //------------------------------------------------ public properties
    public float minXValue = -10f, maxXValue = 10f, minYValue = -10f, maxYValue = 10f;

    //---------------------------------------------------------------------- methods
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Init
    void Init()
    {
        InitCameraGraphSize();

        lineRend_Cp.positionCount = lineRendPosCount;
    }

    // InitCameraGraphSize
    void InitCameraGraphSize()
    {
        float screenWidthCur = Screen.width;
        float screenHeightCur = Screen.height;
        float screenRatioCur = screenHeightCur / screenWidthCur;
        float screenRatioBase = screenHeightBase / screenWidthBase;
        
        float camWidthCur = camWidthBase;
        float camHeightCur = camHeightBase;
        if(screenRatioCur < screenRatioBase)
        {
            camWidthCur = camWidthBase / (screenRatioBase / screenRatioCur);
        }
        else
        {
            camHeightCur = camHeightBase * (screenRatioBase / screenRatioCur);
        }

        camGraph_Cp.rect = new Rect(camGraph_Cp.rect.xMin, camGraph_Cp.rect.yMin, camWidthCur, camHeightCur);
    }

    //------------------------------------------------ public methods
    // Draw graph
    public void DrawGraph(params float[] coefficients)
    {
        curCoefficients = coefficients;

        float x = minXValue, y = 0f;

        for(int i = 0; i < lineRend_Cp.positionCount; i++)
        {
            y = GetYFromX(x, coefficients);

            float lineX = x / maxXValue * camGraph_Cp.orthographicSize;
            float lineY = y / maxYValue * camGraph_Cp.orthographicSize;
            lineRend_Cp.SetPosition(i, new Vector3(lineX, lineY, 0f));

            x += ratioX;
            y = 0f;
        }
    }

    // Set points on graph
    public void SetPointsOnGraph(Dictionary<float, float> graphPoints_pr, bool showValue_pr = true, bool showToggle_pr = false)
    {
        foreach(GameObject graphPoint_GO_tp in graphPoint_GOs)
        {
            GameObject.Destroy(graphPoint_GO_tp);
        }
        graphPoint_GOs.Clear();

        foreach(float graphPointX_tp in graphPoints_pr.Keys)
        {
            // instant graph point object
            GameObject graphPoint_GO_tp = Instantiate(graphPoint_Pf, graphPointHolder_Tf);
            graphPoint_GOs.Add(graphPoint_GO_tp);

            RectTransform graphPoint_RT_tp = graphPoint_GO_tp.GetComponent<RectTransform>();
            graphPoint_RT_tp.anchorMin = new Vector2(graphPointX_tp / (maxXValue - minXValue) + 0.5f,
                graphPoints_pr[graphPointX_tp] / (maxYValue - minYValue) + 0.5f);
            graphPoint_RT_tp.anchorMax = graphPoint_RT_tp.anchorMin;

            // set value of graph point
            string graphPointXtext = graphPointX_tp.ToString();
            if(graphPointXtext.Length > 5)
            {
                graphPointXtext = graphPointXtext.Substring(0, 5);
                if(!graphPointXtext.StartsWith("-"))
                {
                    graphPointXtext = graphPointXtext.Substring(0, 4);
                }
            }

            string graphPointYtext = graphPoints_pr[graphPointX_tp].ToString();
            if(graphPointYtext.Length > 5)
            {
                graphPointYtext = graphPointYtext.Substring(0, 5);
                if(!graphPointYtext.StartsWith("-"))
                {
                    graphPointYtext = graphPointYtext.Substring(0, 4);
                }
            }

            TMP_Text[] graphPointText_Cps_tp = graphPoint_GO_tp.transform.Find("PointValues Panel").gameObject
                .GetComponentsInChildren<TMP_Text>();
            for(int j = 0; j < graphPointText_Cps_tp.Length; j++)
            {
                graphPointText_Cps_tp[j].text = "(" + graphPointXtext + ", " + graphPointYtext + ")";
            }

            // set toggle on/off
            // graphPoint_GO_tp.transform.Find("Select Toggle").GetComponent<Toggle>().interactable = showToggle_pr;
            graphPoint_GO_tp.transform.Find("Select Toggle").GetComponent<Toggle>().interactable = false;
        }

        ToggleGraphPointValue(showValue_pr);
    }

    // Toggle graph point value
    public void ToggleGraphPointValue(bool flag)
    {
        for(int i = 0; i < graphPoint_GOs.Count; i++)
        {
            bool toggleFlag = (flag && showPointValueFlag);

            graphPoint_GOs[i].transform.Find("PointValues Panel").GetChild(0).gameObject
                .SetActive(toggleFlag);
        }
    }

    // Get y from x
    public float GetYFromX(float x, params float[] coefficients)
    {
        float y = 0f;

        for(int j = coefficients.Length - 1; j >= 0; j--)
        {
            y += coefficients[j] * Mathf.Pow(x, (coefficients.Length - 1 - j));
        }

        return y;
    }

    // Get zero points
    public List<float> GetZeroPoints(params float[] coefficients)
    {
        List<float> value = new List<float>();

        if(coefficients.Length == 2)
        {
            float a = coefficients[0];
            float b = coefficients[1];
            float zero = -1f * b / a;

            value.Add(zero);
        }
        else if(coefficients.Length == 3)
        {
            float a = coefficients[0];
            float b = coefficients[1];
            float c = coefficients[2];
            float b24ac = Mathf.Pow(b, 2) - 4f * a * c;

            if(b24ac >= 0f)
            {
                float zero1 = (-1f * b - Mathf.Sqrt(b24ac)) / 2f / a;
                float zero2 = (-1f * b + Mathf.Sqrt(b24ac)) / 2f / a;

                value.Add(zero1);
                value.Add(zero2);
            }
        }
        else if(coefficients.Length >= 4)
        {
            float x = minXValue,  y = 0f;

            float oldX = 0f, oldY = 0f, zeroX = 0f;
            for(int i = 0; i < lineRend_Cp.positionCount; i++)
            {
                for(int j = coefficients.Length - 1; j >= 0; j--)
                {
                    y += coefficients[j] * Mathf.Pow(x, (coefficients.Length - 1 - j));
                }

                if((oldY < 0f && y > 0f) || (oldY > 0f && y < 0f))
                {
                    zeroX = x;
                    value.Add(zeroX);
                }

                oldX = x;
                oldY = y;
                x += ratioX;
                y = 0f;
            }
        }

        return value;
    }

    // Set zero points on graph
    public void SetZeroPointsOnGraph(bool showValue_pr, params float[] coefficients)
    {
        List<float> zeroXPoints = GetZeroPoints(coefficients);
        Dictionary<float, float> zeroPoints = new Dictionary<float, float>();
        foreach(float zeroXPoint_tp in zeroXPoints)
        {
            zeroPoints[zeroXPoint_tp] = 0f;
        }

        SetPointsOnGraph(zeroPoints, showValue_pr);
    }

    // Get selected graph points
    public List<int> GetSelectedGraphPoints()
    {
        List<int> value = new List<int>();

        for(int i = 0; i < graphPoint_GOs.Count; i++)
        {
            if(graphPoint_GOs[i].transform.Find("Select Toggle").GetComponent<Toggle>().isOn)
            {
                value.Add(i);
            }
        }

        return value;
    }

    // Get selected graph point texts
    public List<string> GetSelectedGraphPointTexts()
    {
        List<string> value = new List<string>();

        for(int i = 0; i < graphPoint_GOs.Count; i++)
        {
            if(graphPoint_GOs[i].transform.Find("Select Toggle").GetComponent<Toggle>().isOn)
            {
                value.Add(graphPoint_GOs[i].transform.Find("PointValues Panel")
                    .GetComponentInChildren<TMP_Text>().text);
            }
        }

        return value;
    }

    // Click graph point
    public void ClickGraphPoint(int pointIndex, bool clickFlag)
    {
        graphPoint_GOs[pointIndex].transform.Find("Select Toggle").GetComponent<Toggle>().isOn = clickFlag;

        concept2.OnClickGraphPoint();
    }
    
    // Get shortest specification point from given point
    public Vector3 GetShortestSpecificPointFromGivenPoint(Vector3 givenPoint,
        out bool existShortestPoint, out int shortestPointIndex, float allowDist)
    {
        Vector3 value = new Vector3();
        existShortestPoint = true;

        // get short points from given point
        List<int> shortPoints = new List<int>();
        for(int i = 0; i < graphPoint_GOs.Count; i++)
        {
            if(Vector3.Distance(givenPoint, graphPoint_GOs[i].transform.position) <= allowDist)
            {
                shortPoints.Add(i);
            }
        }

        // filter shortest point from short points
        if(shortPoints.Count == 0)
        {
            existShortestPoint = false;
            shortestPointIndex = -1;

            return value;
        }

        int shortestPointIndexID = 0;
        for(int i = 0; i < shortPoints.Count; i++)
        {
            if(Vector3.Distance(givenPoint, graphPoint_GOs[shortPoints[i]].transform.position)
                < Vector3.Distance(givenPoint, graphPoint_GOs[shortPoints[shortestPointIndexID]].transform.position))
            {
                shortestPointIndexID = i;
            }
        }

        // set correct values
        shortestPointIndex = shortPoints[shortestPointIndexID];
        value = graphPoint_GOs[shortestPointIndex].transform.position;

        return value;
    }

    // Get shortest graph point from given point
    public Vector2 GetShortestGraphPointFromGivenPoint(Vector2 givenPoint,
        out bool existShortestPoint, float allowDist)
    {
        Vector2 value = new Vector2();
        existShortestPoint = false;

        float graphY = GetYFromX(givenPoint.x, curCoefficients);

        if(Mathf.Abs(givenPoint.y - graphY) <= allowDist)
        {
            existShortestPoint = true;
            value = new Vector2(givenPoint.x, graphY);
        }
        value = GetWorldPositionFromGraphPosition(value);
        
        return value;
    }

    // Get shortest graph point from give point
    public Vector2 GetShortestGraphPointFromGivenPoint(Vector2 givenPoint)
    {
        Vector2 value = new Vector2();

        float graphY = GetYFromX(givenPoint.x, curCoefficients);

        value = GetWorldPositionFromGraphPosition(new Vector2(givenPoint.x, graphY));

        return value;
    }

    // Get graph position corresponding world position
    public Vector2 GetGraphPosFromWorldPos(Vector2 worldPos_pr)
    {
        Vector2 value = new Vector2();

        value = new Vector2(worldPos_pr.x / camGraph_Cp.orthographicSize * maxXValue,
            worldPos_pr.y / camGraph_Cp.orthographicSize * maxYValue);

        return value;
    }

    // Get world position coressponding graph position
    public Vector2 GetWorldPositionFromGraphPosition(Vector2 graphPos_pr)
    {
        Vector2 value = new Vector2();

        value = new Vector2(graphPos_pr.x / maxXValue * camGraph_Cp.orthographicSize,
            graphPos_pr.y / maxYValue * camGraph_Cp.orthographicSize);

        return value;
    }

    // Get given point is nearest at graph
    public bool IsNearestAtGraph(Vector2 givenPoint_pr, float allowDist = 0.001f)
    {
        bool value = false;
        float y = GetYFromX(givenPoint_pr.x, curCoefficients);
        
        if(Mathf.Abs(y - givenPoint_pr.y) < allowDist)
        {
            value = true;
        }

        return value;
    }

    //------------------------------------------------ callback from UI
    public void OnClickToggleGraphPointValue(bool flag)
    {
        // showPointValueFlag = flag;

        // ToggleGraphPointValue(flag);
    }

}
