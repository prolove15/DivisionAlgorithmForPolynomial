using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDragDrop : MonoBehaviour
{

    //---------------------------------------------------------------------- fields
    [SerializeField]
    Camera ownerCam_Cp;

    bool isHeld, isFarawayFromOrigin, isClingAtShortestPoint;

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

    float clingOriginDist
    {
        get { return graph_Cp.clingDist; }
    }

    float clingShortestDist
    {
        get { return graph_Cp.clingDist; }
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

        ClingOriginPosition();

        ClingShortestSpecificPoint();
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
            transform.position = mousePosition + offset;

            // filter position of transform
            float xFilter = Mathf.Clamp(transform.position.x, -1f * ownerCam_Cp.orthographicSize, ownerCam_Cp.orthographicSize);
            float yFilter = Mathf.Clamp(transform.position.y, -1f * ownerCam_Cp.orthographicSize, ownerCam_Cp.orthographicSize);
            transform.position = new Vector3(xFilter, yFilter, transform.position.z);
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

    // Cling shortest specific point
    void ClingShortestSpecificPoint()
    {
        if(!isHeld)
        {
            return;
        }

        bool existShortestPoint = true;
        int shortestPointIndex = 0;
        Vector3 shortestPoint = graph_Cp.GetShortestSpecificPointFromGivenPoint
            (transform.position, out existShortestPoint, out shortestPointIndex, clingShortestDist);

        if(existShortestPoint && !isClingAtShortestPoint)
        {
            transform.position = shortestPoint;
            
            isClingAtShortestPoint = true;
            isHeld = false;

            graph_Cp.ClickGraphPoint(shortestPointIndex, true);
        }
        else if(existShortestPoint && isClingAtShortestPoint)
        {
            graph_Cp.ClickGraphPoint(shortestPointIndex, false);
        }
        else if(!existShortestPoint)
        {
            isClingAtShortestPoint = false;
        }
    }

    public void InitPosition()
    {
        transform.position = originPos;
    }
}
