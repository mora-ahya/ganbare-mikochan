using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField] GameObject backview = default;
    [SerializeField] Enemy[] enemies = default;
    [SerializeField] Treasure[] treasureInStage = default;

    [SerializeField] float stageLeft = default;
    [SerializeField] float stageRight = default;

    public void Initialize()
    {
        foreach (Enemy enemy in enemies)
        {
            if (!enemy.transform.parent.gameObject.activeSelf)
                enemy.transform.parent.gameObject.SetActive(true);

            enemy.ResetFlag = true;
            enemy.Set();
            enemy.ResetPos();
            if (enemy.gameObject.activeSelf)
                enemy.gameObject.SetActive(false);
        }
        
        CameraManager.Instance.SetStageSide(stageLeft + transform.position.x , stageRight + transform.position.x);

        if (backview != default)
        {
            CameraManager.Instance.SetBackView(backview);
        }
    }

    public void Act()
    {

        if (GameSystem.Instance.Stop)
            return;

        foreach (Enemy enemy in enemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                enemy.Act();
            }
        }

        foreach (Treasure treasure in treasureInStage)
        {
            treasure.Act();
        }
    }

    public void StopEnemy()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                enemy.Pause();
            }
        }
    }

    public void RestartEnemy()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                enemy.Restart();
            }
        }
    }
}
