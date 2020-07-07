﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInitSet;

public class CameraManager : MonoBehaviour
{
    static CameraManager cameraManagerInstance;
    public static CameraManager Instance => cameraManagerInstance;

    readonly float cameraHalfWidth = 8f;

    [SerializeField] float stageRight = 190;
    [SerializeField] float stageLeft = -7;
    float minDifY = -2.0f;

    [SerializeField] GameObject postCamera = default;
    [SerializeField] MyPostEffects mpe = default;

    Vector3 tmp = new Vector3(0, 0, -10);
    Vector2 dif = new Vector2(1.0f, 2.0f);
    Vector2 bvDis = new Vector2(13f, 6.3f);
    GameObject backView; // 13f 6.3f
    bool fixation = false;
    bool shake;
    bool lookUnder;
    float d = 1;
    float value = 0;
    float fixedPos = 0;

    public MyPostEffects MainPostEffect => mpe;

    public bool GetShake => shake;

    public bool GetLook => lookUnder;

    void Awake()
    {
        cameraManagerInstance = this;
    }

    void ProcessShake()
    {
        if (shake)
        {
            tmp.y -= value;
            value = Random.Range(1, 11) * d / 100;
            tmp.y += value;
            d *= -1;
        }
        else if (value != 0)
        {
            tmp.y -= value;
            value = 0;
        }
    }

    void ProcessLook()
    {
        if (lookUnder)
        {
            if (dif.y > minDifY)
            {
                dif.y -= 0.05f;
            }
        } else if (dif.y < -minDifY)
        {
            dif.y += 0.1f;
            if (dif.y > -minDifY)
            {
                dif.y = -minDifY;
            }
        }
    }

    void ProcessView()
    {
        tmp.z = 0;
        tmp.x += bvDis.x * (1.0f - tmp.x / stageRight);
        tmp.y = backView.transform.position.y;
        backView.transform.position = tmp;
    }

    public void Set(float x, float y)
    {
        tmp.x = fixation ? fixedPos : x;
        tmp.y = y;
        tmp.z = -10;
        ProcessShake();
        ProcessLook();
        tmp.y += dif.y;
        if (tmp.x < stageLeft)
            tmp.x = stageLeft;
        
        if (tmp.x > stageRight)
            tmp.x = stageRight;

        transform.position = tmp;
        if (backView != null)
            ProcessView();

        postCamera.transform.position = transform.position;
    }

    public void SetStageSide(float left, float right)
    {
        stageLeft = left + cameraHalfWidth;
        stageRight = right - cameraHalfWidth;
    }

    public void Shake(bool value)
    {
        shake = value;
    }

    public void LookUnder(bool value)
    {
        lookUnder = value;
    }

    public void FixedCamera(bool value)
    {
        fixation = value;
        if (value)
        {
            fixedPos = transform.position.x;
        }
    }

    public void SetBackView(GameObject bv)
    {
        backView = bv;
    }
}
