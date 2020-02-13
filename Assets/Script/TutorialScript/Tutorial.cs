using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] BoxCollider2D bc;
    [SerializeField] CapsuleCollider2D cc;
    private readonly string textName = "Text/tutorial";

    private List<string> tutorialText = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        TextLoader.Instance.LoadText(textName, tutorialText);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Mikochan.Instance.tag))
        {
            EventTextManager.Instance.Set(tutorialText);
            bc.enabled = false;
        }
    }
}
