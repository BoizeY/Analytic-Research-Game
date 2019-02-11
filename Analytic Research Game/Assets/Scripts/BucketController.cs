using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BucketController : MonoBehaviour
{

    public GameObject bucketWater;
    public Button empty;
    public float waterFillTimer;
    public Text timeRelativeToFill;
    public Text timePassFill;
    public float canEmptyPercentage;

    public GameObject container;

    Image bucketWaterSprite;
    float timer;
    float tillFull;

    // Start is called before the first frame update
    void OnEnable()
    {

        bucketWaterSprite = bucketWater.GetComponent<Image>();
        empty.onClick.AddListener(emptyWater);
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime/ waterFillTimer;
        // time relative till what is at fill
        tillFull = timer* waterFillTimer;
        bucketWaterSprite.fillAmount = timer;

        timeRelativeToFill.text = (tillFull - waterFillTimer).ToString();
    }

    void emptyWater()
    {

        timePassFill.text = (tillFull - waterFillTimer).ToString();
            timer = 0.0f;
            bucketWaterSprite.fillAmount = 0.0f;
    }


    void setGameOff(bool set)
    {
        container.SetActive(set);
    }
}
