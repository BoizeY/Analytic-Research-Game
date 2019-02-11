using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    int counter;
    // Start is called before the first frame update
    void Start()
    {
        for (counter = 0; counter < 10; counter++)
        {
            Debug.Log("YOLO");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
