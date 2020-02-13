using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GashaHand : Enemy
{
    [SerializeField] CameraManager cm;
    [SerializeField] float waitTime;
    private WaitForSeconds wait;

    void Start()
    {
        wait = new WaitForSeconds(waitTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            if (!cm.GetShake)
            {
                StartCoroutine("Shake");
            }
        }
        //Debug.Log(other.gameObject.tag);
    }

    IEnumerator Shake()
    {
        cm.Shake(true);
        yield return wait;
        cm.Shake(false);
    }
}
