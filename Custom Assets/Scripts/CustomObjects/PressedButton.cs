using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
 
public class PressedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
 
    [SerializeField]
    int id;

    [SerializeField]
    float step = 1f;

    GraphManager graphManager_Cp
    {
        get { return FindObjectOfType<GraphManager>(); }
    }

    Concept3 concept3_Cp
    {
        get
        {
            return GameObject.FindWithTag("GameController").GetComponent<Controller>().concept3_Cp;
        }
    }

    bool buttonPressed;
    
    void Update()
    {
        // if(buttonPressed){
        //     graphManager_Cp.OnPlusCoefficients(id, step);
        // }
        if(buttonPressed)
        {
            concept3_Cp.OnPlusMinusCoefficients(id, step);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonPressed = true;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        buttonPressed = false;
    }
}