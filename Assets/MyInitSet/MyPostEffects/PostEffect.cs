using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostEffect : MonoBehaviour
{
    [SerializeField]
    protected Shader shader;
    protected Material material;
    protected CommandBuffer cb;
    protected bool isActive;

    public CommandBuffer Cb => cb;

    public bool IsActive
    {
        get
        {
            return isActive;
        }

        set
        {
            isActive = value;
        }
    }

    virtual public void Run()
    {

    }

    virtual public void Clear()
    {

    }

}
