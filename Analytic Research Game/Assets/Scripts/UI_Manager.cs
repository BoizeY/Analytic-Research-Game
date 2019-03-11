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

    public InputField in_ID;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public void IDInputHandler(){

        // Send the participant ID to the game controller, assuming the input can be parsed correctly
        int ID = -1;
        if (int.TryParse(in_ID.text, out ID)) {

            Game_Controller.SetParticipantID(ID);

            ID_inputForm.SetActive(false);
            consentForm.SetActive(true);
        }
    }

    public void consentFormHandler(){
        consentForm.SetActive(false);
        tutorial.SetActive(true);
    }
}
