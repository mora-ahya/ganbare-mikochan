using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    const string gameover = "GAME OVER";
    const string gameclear = "GAME CLEAR";

    [SerializeField] Text t;
    [SerializeField] Image[] i;
    [SerializeField] Image darkness;

    WaitForSeconds wait = new WaitForSeconds(2.0f);
    // Start is called before the first frame update
    void Start()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1.0f;
        }
        darkness.gameObject.SetActive(true);
        t.text = GameSystem.gameOver ? gameover : gameclear;
        StartCoroutine("Show");
    }

    // Update is called once per frame
    IEnumerator Show()
    {
        darkness.CrossFadeAlpha(0, 3.0f, true);
        yield return wait;
        t.gameObject.SetActive(true);
        yield return wait;
        i[0].gameObject.SetActive(false);
        (GameSystem.gameOver ? i[1] : i[2]).gameObject.SetActive(true);
        yield return wait;
        darkness.CrossFadeAlpha(1, 2.0f, true);
        yield return wait;
        SceneManager.LoadScene("Title");
    }
}
