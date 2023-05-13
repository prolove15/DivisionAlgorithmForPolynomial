using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickGraphPoint()
    {
        GameObject.FindWithTag("GameController").GetComponent<Controller>().concept2_Cp
            .OnClickGraphPoint();
    }
}
