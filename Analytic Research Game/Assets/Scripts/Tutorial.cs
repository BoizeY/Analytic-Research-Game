using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public Sprite[] gallary;
    public Image frame;
    public GameObject start;

    private int gallaryPage;

    // Start is called before the first frame update
    void Start()
    {
        gallaryPage = 0;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void nextImage()
    {
        gallaryPage = (gallaryPage >= (gallary.Length-1)) ? 0 : (++gallaryPage);

        if (gallaryPage == gallary.Length-1)
            start.SetActive(true);

        frame.sprite = gallary[gallaryPage];


    }
    public void prvImage()
    {
        gallaryPage = gallaryPage <= 0 ? (gallary.Length-1) : (--gallaryPage);
        frame.sprite = gallary[gallaryPage];
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}
