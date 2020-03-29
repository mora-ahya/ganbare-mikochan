using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowlManager : MonoBehaviour
{
    static HowlManager howlManagerInstance;
    public static HowlManager Instance => howlManagerInstance;

    private Vector3 v = Vector3.zero;
    // Start is called before the first frame update
    void Awake()
    {
        howlManagerInstance = this;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale += v;
        if (transform.localScale.x >= 10)
        {
            gameObject.SetActive(false);
        }
    }

    public void Howl(Vector3 pos, float velocity)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        transform.position = pos;
        transform.localScale = Vector3.zero;
        v.x = v.y = velocity;

    }
}
