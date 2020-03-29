using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPostEffects : MonoBehaviour
{
    public static readonly int WAVE_EFFECT = 0;
    public static readonly int GAUSSIANBLUR_EFFECT = 1;
    public static readonly int SIMPLECOLOR_EFFECT = 2;
    public static readonly int CIRCLE_GRAYSCALE_EFFECT = 3;

    [SerializeField] PostEffect[] postEffects;

    public bool GetEffectActive(int effectNum)
    {
        return postEffects[effectNum].IsActive;
    }

    public void SetEffectActive(int effectNum, bool b)
    {
        if (b)
        {
            if (!postEffects[effectNum].IsActive)
            {
                postEffects[effectNum].IsActive = b;
                postEffects[effectNum].Clear();
            }
        }
        else
        {
            if (postEffects[effectNum].IsActive)
            {
                postEffects[effectNum].IsActive = b;
            }
        }
        

    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        foreach (PostEffect pe in postEffects)
        {
            if (pe.IsActive)
                pe.Run();
        }
        Graphics.Blit(src, dest);
        //Debug.Log(true);
    }
}
