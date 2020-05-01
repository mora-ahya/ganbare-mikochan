using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstBossEvent : MonoBehaviour
{
    [SerializeField] GameObject invisibleWall = default;
    [SerializeField] GameObject gasha = default;
    [SerializeField] BoxCollider2D bc = default;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("mikochan"))
        {
            gasha.SetActive(true);
            invisibleWall.SetActive(true);
            TrainingSceneManager.Instance.IsOperational = false;
            CameraManager.Instance.FixedCamera(true);
            bc.enabled = false;
        }
        
    }

    public void BeatenBoss()
    {
        StartCoroutine("Beaten");
        GameSystem.Instance.Whiteness.gameObject.SetActive(true);
        GameSystem.Instance.Whiteness.CrossFadeAlpha(0f, 0.2f, false);
    }

    IEnumerator Beaten()
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(0.2f);
        Time.timeScale = 1.0f;
        yield return GameSystem.Instance.FourSecond;
        GameSystem.Instance.GameClear();
    }
}
