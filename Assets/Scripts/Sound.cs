using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    [SerializeField] private AudioClip _soundTakeCoin;

    private AudioSource _SoundBloks;
    private void OnEnable()
    {
        CoinController._scoreForCoint += SoundTakeCoin;
    }
    private void OnDisable()
    {
        CoinController._scoreForCoint -= SoundTakeCoin;
    }

    private void Awake()
    {
        _SoundBloks = GetComponent<AudioSource>();

    }
  

    private void SoundTakeCoin()
    {
        _SoundBloks.PlayOneShot(_soundTakeCoin);
    }
}
