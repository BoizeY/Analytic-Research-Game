using UnityEngine;
using UnityEngine.SceneManagement;

public class ThankYouScreen : MonoBehaviour
{
    public void OnRestartPressed()
    {
        SceneManager.LoadScene("Setup");
    }
}
