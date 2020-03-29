using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static SoundManager soundManagerInstance;
    public static SoundManager Instance => soundManagerInstance;

    public static readonly int EXPLOSION = 0;
    public static readonly int SHOT_SOUND = 1;

    public static readonly int TITLE_BGM = 0;
    public static readonly int GAME_BGM = 1;
    public static readonly int END1_BGM = 2;
    public static readonly int END2_BGM = 3;

    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource seSource;
    [SerializeField] AudioClip[] ses;
    [SerializeField] AudioClip[] bgms;

    // Start is called before the first frame update
    void Awake()
    {
        soundManagerInstance = this;
    }

    public void PlaySE(int seNum)
    {
        seSource.PlayOneShot(ses[seNum]);
    }

    public void SetBGM(int bgmNum)
    {
        bgmSource.clip = bgms[bgmNum];
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void SetSEVolum(float volume)
    {
        seSource.volume = volume;
    }

    public void SetBGMVolum(float volume)
    {
        bgmSource.volume = volume;
    }


}
