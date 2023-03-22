using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioClip pickCoinSound;
    [SerializeField] AudioClip pickBCSound;
    [SerializeField] AudioClip eatGhostSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip levelCompleteSound;

    private void Start()
    {
        soundSource.volume = PlayerPrefs.GetFloat("Volumn",1f);
    }
    public void PickCoin()
    {
        soundSource.PlayOneShot(pickCoinSound);
    }
    public void PickBigCoin()
    {
        soundSource.PlayOneShot(pickBCSound);
    }

    public void EatGhost()
    {
        soundSource.PlayOneShot(eatGhostSound);
    }

    public void Death()
    {
        soundSource.PlayOneShot(deathSound);
    }
    
    public void LevelComplete()
    {
        soundSource.PlayOneShot(levelCompleteSound);
    }

    public void ChangeVolumn(System.Single value)
    {
        soundSource.volume = value;
        PlayerPrefs.SetFloat("Volumn", value);
    }
}
