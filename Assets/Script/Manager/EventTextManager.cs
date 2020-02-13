using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventTextManager : MonoBehaviour
{
    private static EventTextManager etmInstance;

    public static EventTextManager Instance => etmInstance;

    [SerializeField] Text t;

    private List<string> multipleText = null;
    private int indexNum = 1;
    private WaitForSeconds wait;

    void Awake()
    {
        etmInstance = this;
    }

    void Start()
    {
        t.text = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (multipleText != null)
        {
            MultipleText();
        }
        else
        {
            SingleText();
        }
    }

    public void Set(string str)
    {
        t.text = str;
        Lock();
    }

    public void Set(List<string> mStr)
    {
        multipleText = mStr;
        t.text = multipleText[0];
        Lock();
    }

    private void SingleText()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Release();
        }
    }

    private void MultipleText()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (multipleText.Count <= indexNum)
            {
                Release();
                multipleText = null;
                indexNum = 1;
            }
            else
            {
                t.text = multipleText[indexNum++];
            }
        }
    }

    private void Lock()
    {
        gameObject.SetActive(true);
        GameSystem.stop = true;
        Time.timeScale = 0;
    }

    private void Release()
    {
        GameSystem.stop = false;
        Time.timeScale = 1.0f;
        t.text = null;
        gameObject.SetActive(false);
    }
    /*
    public void Set(string c, bool f, WaitForSeconds w)
    {
        t.text = c;
        wait = w;
        finish = f;
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        StartCoroutine("Wait");
    }
    
    IEnumerator Wait()
    {
        yield return wait;
        finish = true;
    }
    */
}
