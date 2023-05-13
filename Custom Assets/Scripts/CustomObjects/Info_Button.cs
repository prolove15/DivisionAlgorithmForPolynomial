using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info_Button : MonoBehaviour
{

    //---------------------------------------------------------------------- fields
    [SerializeField]
    Animator onMouseAnim_Cp;

    //---------------------------------------------------------------------- methods
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // OnMouseEnter
    void OnMouseEnter()
    {
        onMouseAnim_Cp.SetBool("flag", true);
    }

    // OnMouseExit
    void OnMouseExit()
    {
        onMouseAnim_Cp.SetBool("flag", false);
    }
}
