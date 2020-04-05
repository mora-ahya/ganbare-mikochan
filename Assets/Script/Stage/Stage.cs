using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField] GameObject backview = default;
    [SerializeField] Enemy[] enemies = default;

    public void Initialize()
    {
        foreach (Enemy enemy in enemies)
        {
            enemy.ResetFlag = true;
            enemy.Set();
            enemy.ResetPos();
            if (enemy.gameObject.activeSelf)
            {
                enemy.gameObject.SetActive(false);
            }
        }

        if (backview != default)
        {
            CameraManager.Instance.SetBackView(backview);
        }
    }

    public void Act()
    {
        if (!TrainingSceneManager.Instance.gameObject.activeSelf)
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy.gameObject.activeSelf)
                {
                    enemy.Act();
                }
            }
        }
    }
}
