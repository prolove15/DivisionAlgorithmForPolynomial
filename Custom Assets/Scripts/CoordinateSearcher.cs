using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoordinateSearcher : MonoBehaviour
{
    
    //---------------------------------------------------------------------- fields
    [SerializeField]
    TMP_Text coordinateSearcherText_Cp;

    bool isHeld, isFarawayFromOrigin, isClingAtGraph, isClosetAtGraph, isClingAtShortestPoint;

    Vector3 originPos, offset;

    //---------------------------------------------------------------------- properties
    Controller controller_Cp
    {
        get { return GameObject.FindWithTag("GameController").GetComponent<Controller>(); }
    }

    Graph graph_Cp
    {
        get { return controller_Cp.graph_Cp; }
    }

    Camera ownerCam_Cp
    {
        get { return graph_Cp.camGraph_Cp; }
    }

    float clingOriginDist
    {
        get { return graph_Cp.clingDist; }
    }

    float clingShortestDist
    {
        get { return graph_Cp.clingDist; }
    }

    Vector2 curCoordinate
    {
        get
        {
            Vector2 value = new Vector2();

            Vector3 screenPos = transform.position / ownerCam_Cp.orthographicSize * graph_Cp.maxXValue;

            value = new Vector2(screenPos.x, screenPos.y);

            return value;
        }
    }

    bool isNearestAtGraph
    {
        get { return graph_Cp.IsNearestAtGraph(curCoordinate, 0.001f); }
    }

    //---------------------------------------------------------------------- methods
    // Start is called before the first frame update
    void Start()
    {
        originPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Dragging();

        ShowCoordinateValue();

        ClingGraph();

        ClingOriginPosition();
    }

    // Show coordinate value
    void ShowCoordinateValue()
    {
        if(!isClosetAtGraph)
        {
            coordinateSearcherText_Cp.text = string.Empty;
            return;
        }

        string coordinateX = curCoordinate.x.ToString();
        string coordinateY = curCoordinate.y.ToString();
        int xLength = Mathf.Clamp(coordinateX.Length, 0, 5);
        int yLength = Mathf.Clamp(coordinateY.Length, 0, 5);
        coordinateX = coordinateX.Substring(0, xLength);
        coordinateY = coordinateY.Substring(0, yLength);

        coordinateSearcherText_Cp.text = "(" + coordinateX + "," + coordinateY + ")";
    }

    // Cling graph
    void ClingGraph()
    {
        if(!isHeld)
        {
            return;
        }

        bool existShortestPoint = false;
        Vector2 shortestPoint = graph_Cp.GetShortestGraphPointFromGivenPoint(curCoordinate,
            out existShortestPoint, graph_Cp.clingDist);

        if(existShortestPoint && !isClingAtGraph && !isClosetAtGraph)
        {
            Vector3 newTransformPos = new Vector3(shortestPoint.x, shortestPoint.y, transform.position.z);
            transform.position = newTransformPos;
            
            isClingAtGraph = true;
            isClosetAtGraph = true;
        }
        else if(!existShortestPoint && isClingAtGraph && !isClosetAtGraph)
        {
            isClingAtGraph = false;
        }
    }

    // Dragging object
    void Dragging()
    {
        Vector3 mousePosition = ownerCam_Cp.ScreenToWorldPoint(Input.mousePosition);

        // check transform is held
        if(Input.GetMouseButtonDown(0))
        {
            Collider2D draggingTargetCollider_Cp = Physics2D.OverlapPoint(mousePosition);

            if(draggingTargetCollider_Cp)
            {
                if(draggingTargetCollider_Cp.transform.IsChildOf(transform))
                {
                    isHeld = true;
                    offset = transform.position - mousePosition;
                }
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            isHeld = false;
            offset = Vector3.zero;
        }

        // move transform tracking mouse position
        if(isHeld)
        {
            // filter position of transform
            if(isClosetAtGraph)
            {
                Vector2 targetCoordinate = graph_Cp.GetGraphPosFromWorldPos(new Vector2((mousePosition + offset).x,
                    (mousePosition + offset).y));

                Vector2 shortestPoint_tp = graph_Cp.GetShortestGraphPointFromGivenPoint(
                    new Vector2(targetCoordinate.x, targetCoordinate.y));
                
                transform.position = new Vector3(shortestPoint_tp.x, shortestPoint_tp.y, transform.position.z);
            }
            else
            {
                transform.position = mousePosition + offset;
                float xFilter = Mathf.Clamp(transform.position.x, -1f * ownerCam_Cp.orthographicSize, ownerCam_Cp.orthographicSize);
                float yFilter = Mathf.Clamp(transform.position.y, -1f * ownerCam_Cp.orthographicSize, ownerCam_Cp.orthographicSize);
                transform.position = new Vector3(xFilter, yFilter, transform.position.z);
            }
        }
    }

    // Cling origin position
    void ClingOriginPosition()
    {
        if(!isHeld)
        {
            return;
        }

        if(isFarawayFromOrigin && Vector3.Distance(transform.position, originPos) <= clingOriginDist)
        {
            transform.position = originPos;

            isFarawayFromOrigin = false;
            isHeld = false;
        }
        else if(Vector3.Distance(transform.position, originPos) > clingOriginDist)
        {
            isFarawayFromOrigin = true;
        }
    }

    // Init position
    public void InitPosition()
    {
        isHeld = false;
        isClingAtGraph = false;
        isClosetAtGraph = false;

        transform.position = originPos;
    }
}
