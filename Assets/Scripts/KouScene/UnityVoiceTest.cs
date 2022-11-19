using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class UnityVoiceTest : MonoBehaviour
{
    //[SerializeField]
    //private string[] _keywords;
    private KeywordRecognizer _keywordRecognizer;
    private Dictionary<string ,Action> actions = new Dictionary<string, Action>();

    // Start is called before the first frame update
    void Start()
    {
        //_keywordRecognizer = new KeywordRecognizer(_keywords);
        actions.Add("‚Ü‚¦", Forward);
        actions.Add("‚¤‚¦", Up);
        actions.Add("‚µ‚½", Down);
        actions.Add("‚¤‚µ‚ë", Back);

        _keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        _keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        _keywordRecognizer.Start();
    }

    private void OnDisable()
    {
        _keywordRecognizer.Stop();
        _keywordRecognizer.Dispose();
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    private void Forward()
    {
        transform.Translate(1, 0, 0);
    }
    private void Back()
    {
        transform.Translate(-1, 0, 0);
    }
    private void Up()
    {
        transform.Translate(0, 1, 0);
    }
    private void Down()
    {
        transform.Translate(0, -1, 0);
    }
}