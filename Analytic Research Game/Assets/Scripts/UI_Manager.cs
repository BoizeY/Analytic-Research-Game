using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{

    public GameObject ID_inputForm;
    public Text IDInput;
    public Button btn_Next;
    public GameObject tutorial;

    public GameObject consentForm;
    public Button btn_Agree;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public void IDInputHandler(){
        //TODO:Data extaction
        ID_inputForm.SetActive(false);
        consentForm.SetActive(true);
    }

    public void consentFormHandler(){
        consentForm.SetActive(false);
        tutorial.SetActive(true);
    }
}
