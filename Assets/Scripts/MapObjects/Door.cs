using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    NavMeshObstacle obstacle;

    private AudioSource audio;

    private void Awake()
    {
        AddObstacle();
    }

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void Open()
    {
        _animator.SetBool("isOpen", true);        
        RemoveObstacle();
    }

    public void Close()
    {
        _animator.SetBool("isOpen", false);       
        AddObstacle();
    }

    //扉の障害物判定を解除する
    void RemoveObstacle()
    {
        Destroy(gameObject.GetComponent<NavMeshObstacle>());
    }

    //扉を障害物判定する
    void AddObstacle()
    {
        obstacle = gameObject.AddComponent<NavMeshObstacle>();
        SetNavMeshObstacle();
    }

    //障害物のサイズと避ける設定
    void SetNavMeshObstacle()
    {
        obstacle.carving = true;
        obstacle.center = new(0, 1.5f, 0);
        obstacle.size = new(0.6f, 3f, 5.5f);
    }

    //扉のサウンドを流す
    void PlayAudio()
    {
        audio.Play();
    }

    //扉のサウンドを止める
    void StopAudio()
    {
        audio.Stop();
    }
}
