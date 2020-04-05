using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Image darkness = default;
    string changeDestination;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1000, 640, false);
        darkness.gameObject.SetActive(true);
        darkness.CrossFadeAlpha(0, 2.0f, true);
    }

    public void StartButton()
    {
        changeDestination = "Stage";
        StartCoroutine("SceneChange");
    }

    public void TutorialButton()
    {
        changeDestination = "Tutorial";
        StartCoroutine("SceneChange");
    }

    public void EndButton()
    {
        changeDestination = "end";
        StartCoroutine("SceneChange");
    }

    IEnumerator SceneChange()
    {
        darkness.CrossFadeAlpha(1.0f, 0.2f, true);
        yield return new WaitForSeconds(0.2f);
        if (changeDestination == "end")
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene(changeDestination);
        }
    }
}
