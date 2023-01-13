using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 備考：
// 今のAnnouncementManagerはとても簡単です。
// もしアナウンスが増えたら、enumとか管理してください。
public class AnnouncementManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _breakOutAudioClip;
    [SerializeField] private TextMeshProUGUI _announcementText;

    private void Awake()
    {
        ClearText();
    }

    public void PlayBreakOutAnnouncement()
    {
        _audioSource.clip = _breakOutAudioClip;
        _audioSource.Play();

        _announcementText.text = LocalizedText.BreakOutAnnouncement;

        StopAllCoroutines();
        StartCoroutine(HideTextCoroutine());
    }

    private IEnumerator HideTextCoroutine()
    {
        yield return new WaitWhile(() => { return _audioSource.isPlaying; });

        ClearText();
    }

    public void ClearText()
    {
        _announcementText.text = "";
    }
}
