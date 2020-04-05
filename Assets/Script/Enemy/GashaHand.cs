using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GashaHand : Enemy
{
    [SerializeField] float waitTime = default;
    WaitForSeconds wait;

    void Start()
    {
        wait = new WaitForSeconds(waitTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground") && !CameraManager.Instance.GetShake)
            StartCoroutine("Shake");
        //Debug.Log(other.gameObject.tag);
    }

    IEnumerator Shake()
    {
        CameraManager.Instance.Shake(true);
        yield return wait;
        CameraManager.Instance.Shake(false);
    }
}
