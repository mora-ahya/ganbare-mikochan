using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField] private Enemy[] enemies;

    public void Reset()
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
    }
}
