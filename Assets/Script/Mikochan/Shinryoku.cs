using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shinryoku : MonoBehaviour
{
    delegate void Movement();

    static Vector2 Tmp;

    [SerializeField] Enemy enemy = default;
    [SerializeField] Rigidbody2D rb = default;
    [SerializeField] SpriteRenderer sr = default;

    float whiteDegree = 1f;
    float velocity = 0f;
    Movement movement;

    public void Init(Material m)
    {
        sr.material = m;
    }

    public void Set()
    {
        whiteDegree = 1f;
        movement = Generate;
        transform.localScale = Vector3.zero;
        velocity = 5f;
    }

    public void Process()
    {
        movement();
    }

    void Generate()
    {
        if (whiteDegree > 0f)
        {
            whiteDegree -= 1f / 60f;
            sr.material.SetFloat("_White_Degree", whiteDegree);
        }
        transform.localScale += (Vector3.one * 2f) / 60f;

        if (transform.localScale.x >= 2f)
            movement = FlyAway;
    }

    void FlyAway()
    {
        Tmp = StatusManager.Instance.ExpImage.transform.position - gameObject.transform.position;
        if (Tmp.magnitude < 0.3f)
        {
            Mikochan.Instance.GetExp(enemy.Exp);
            //enemy.ResetPos();
            enemy.gameObject.SetActive(false);
            //enemy.ResetFlag = true;
            enemy.transform.parent.gameObject.SetActive(false);
            gameObject.SetActive(false);
            return;
        }

        if (velocity < 20f)
        {
            velocity += 0.2f;
        }

        rb.velocity = Tmp.normalized * velocity;
    }
}
