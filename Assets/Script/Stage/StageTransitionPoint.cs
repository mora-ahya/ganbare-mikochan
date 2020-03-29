using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTransitionPoint : MonoBehaviour
{
    [SerializeField] Vector2 destination = default;
    [SerializeField] GameObject stageDestination = default; //prefabをアタッチ

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Mikochan.Instance.tag))
        {
            Mikochan.Instance.transform.position = destination;
            StageManager.Instance.Transition(stageDestination);
        }
    }
}
