using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridSoundEffect : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    public void PlayPlaceOnGridsSound()
    {
        _audioSource.volume = Random.Range(0.1f, 0.2f);
        _audioSource.Play();
    }
}
