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
    [SerializeField] private TextMeshProUGUI _captionText;

    private void Awake()
    {
        ClearCaption();
    }

    public void PlayBreakOutAnnouncement()
    {
        _audioSource.clip = _breakOutAudioClip;
        _audioSource.Play();

        _captionText.text = LocalizedText.BreakOutAnnouncement;

        StopAllCoroutines();
        StartCoroutine(HideTextCoroutine());
    }

    private IEnumerator HideTextCoroutine()
    {
        yield return new WaitWhile(() => { return _audioSource.isPlaying; });

        ClearCaption();
    }

    public void ClearCaption()
    {
        _captionText.text = "";
    }
}
