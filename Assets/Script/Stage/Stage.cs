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
}
