using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [SerializeField] private AudioSource _enemyAudio;

    [SerializeField] private AudioClip _attackClip;
    [SerializeField] private AudioClip _gameoverClip;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AudioPlay_Attack()
    {
        _enemyAudio.PlayOneShot(_attackClip,1.0f);
    }

    private void AudioPlay_Gameover()
    {
        _enemyAudio.PlayOneShot(_gameoverClip, 1.0f);
    }
}
