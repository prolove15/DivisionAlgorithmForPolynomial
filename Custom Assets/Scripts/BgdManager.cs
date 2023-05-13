using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgdManager : MonoBehaviour
{
    
    //----------------------------------------------- fields
    [SerializeField]
    public Animator curtainAnim_Cp;

    //----------------------------------------------- methods
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CurtainUp()
    {
        if(!curtainAnim_Cp.gameObject.activeInHierarchy)
        {
            curtainAnim_Cp.gameObject.SetActive(true);
        }

        curtainAnim_Cp.SetBool("flag", true);
    }

    public void CurtainDown()
    {
        if(!curtainAnim_Cp.gameObject.activeInHierarchy)
        {
            curtainAnim_Cp.gameObject.SetActive(true);
        }

        curtainAnim_Cp.SetBool("flag", false);
    }
}
