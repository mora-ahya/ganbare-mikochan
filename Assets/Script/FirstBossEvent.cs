using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstBossEvent : MonoBehaviour
{
    [SerializeField] GameObject gasha;
    [SerializeField] GameObject miko;
    [SerializeField] CameraManager cm;
    [SerializeField] BoxCollider2D bc;
    [SerializeField] Image whiteness;
    [SerializeField] Image darkness;
    [SerializeField] GameSystem gs;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("mikochan"))
        {
            gasha.SetActive(true);
            cm.FixedCamera(true);
            bc.enabled = false;
        }
    }

    public void BeatenBoss()
    {
        StartCoroutine("Beaten");
        whiteness.gameObject.SetActive(true);
        whiteness.CrossFadeAlpha(0, 0.2f, false);
    }

    IEnumerator Beaten()
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(0.2f);
        Time.timeScale = 1.0f;
        yield return new WaitForSeconds(4f);
        gs.GameClear();
    }
}
