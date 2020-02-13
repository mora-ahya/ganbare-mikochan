using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamisama : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("enemy"))
        {
            Enemy tmp = other.gameObject.GetComponent<Enemy>();
            tmp.InRange = true;
            tmp.OutLine();
            //Debug.Log(other.gameObject.GetComponent<Enemy>().transform.position.x);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //Debug.Log(true);
        if (other.gameObject.CompareTag("enemy"))
        {
            Enemy tmp = other.gameObject.GetComponent<Enemy>();
            tmp.InRange = false;
            tmp.ResetM();
        }
    }
}
