using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventTextManager : MonoBehaviour
{
    private static EventTextManager EventTextManagerInstance;

    public static EventTextManager Instance => EventTextManagerInstance;

    readonly string[] escapeSecences = { "$", ":" };
    readonly string[] triggerString = {"$name", "$wait", "$stay"};

    [SerializeField] Text mainText = default;
    [SerializeField] Text nameText = default;

    List<string> multipleText = null;
    int indexNum = 0;
    int displayNum = 0;
    int counter = 0;
    bool isStayed = false;
    string[] splitted;
    public int displaySpeed = 3;
    WaitForSeconds wait;

    public bool IsStayed
    {
        get
        {
            return isStayed;
        }

        set
        {
            isStayed = value;
        }
    }
    

    void Awake()
    {
        EventTextManagerInstance = this;
        mainText.text = null;
        nameText.text = null;
        gameObject.SetActive(false);
    }

    public void Act()
    {
        if (!gameObject.activeSelf)
            return;

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

    void SingleText()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Release();
        }
    }

    void MultipleText()
    {
        if (isStayed)
            return;

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
                if (counter++ == displaySpeed)
                {
                    mainText.text = multipleText[indexNum].Substring(0, displayNum++);
                    counter = 0;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    displayNum = multipleText[indexNum].Length;
                    mainText.text = multipleText[indexNum];
                }
                return;
            }
        }

        if (!Input.GetMouseButtonDown(0))
            return;

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
            mainText.text = null;
            return;
        }

        if (splitted[0] == triggerString[2])
        {
            isStayed = true;
            return;
        }
    }

    void Lock()
    {
        //GameSystem.Instance.Stop = true;
        GameSystem.Instance.GameStop();
        TrainingSceneManager.Instance.IsOperational = false;
        //Time.timeScale = 0;
        gameObject.SetActive(true);
    }

    void Release()
    {
        //GameSystem.Instance.Stop = false;
        GameSystem.Instance.GameRestart();
        TrainingSceneManager.Instance.IsOperational = true;
        //Time.timeScale = 1;
        nameText.text = null;
        mainText.text = null;
        gameObject.SetActive(false);
    }
}
