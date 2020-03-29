using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventTextManager : MonoBehaviour
{
    private static EventTextManager etmInstance;

    public static EventTextManager Instance => etmInstance;

    readonly string[] escapeSecences = { "$", ":" };
    readonly string[] triggerString = {"$name", "$wait"};

    [SerializeField] Text mainText;
    [SerializeField] Text nameText;

    List<string> multipleText = null;
    int indexNum = 0;
    int displayNum = 0;
    string[] splitted;
    public int displaySpeed = 1;
    WaitForSeconds wait;
    //public bool pause;

    void Awake()
    {
        etmInstance = this;
        mainText.text = null;
        nameText.text = null;
        gameObject.SetActive(false);
    }

    void Start()
    {
        //mainText.text = null;
        //gameObject.SetActive(false);
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
        mainText.text = str;
        Lock();
    }

    public void Set(List<string> mStr)
    {
        multipleText = mStr;
        Lock();
    }

    public void ResetObject()
    {
        multipleText = null;
        mainText.text = null;
        indexNum = 0;
        displayNum = 0;
        gameObject.SetActive(false);
    }

    private void SingleText()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Release();
        }
    }

    private void MultipleText()
    {
        if (multipleText.Count > indexNum)
        {
            if (multipleText[indexNum].Length > 0 && multipleText[indexNum].Substring(0, 1) == escapeSecences[0])
            {
                EscapeProcess();
                indexNum++;
                return;
            }
            if (multipleText[indexNum].Length >= displayNum)
            {
                mainText.text = multipleText[indexNum].Substring(0, displayNum++);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    displayNum = multipleText[indexNum].Length;
                    mainText.text = multipleText[indexNum];
                }
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (multipleText.Count <= indexNum + 1)
            {
                Release();
                multipleText = null;
                indexNum = 0;
                displayNum = 0;
            }
            else
            {
                indexNum++;
                displayNum = 0;
            }
        }
    }

    void EscapeProcess()
    {
        splitted = multipleText[indexNum].Split(escapeSecences[1][0]);
        if (splitted[0] == triggerString[0])
        {
            nameText.text = splitted[1];
            return;
        }

        if (splitted[0] == triggerString[1])
        {
            gameObject.SetActive(false);
            return;
        }
    }

    private void Lock()
    {
        GameSystem.stop = true;
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }

    private void Release()
    {
        GameSystem.stop = false;
        Time.timeScale = 1;
        nameText.text = null;
        mainText.text = null;
        gameObject.SetActive(false);
    }
}
