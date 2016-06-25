﻿// SpeechManager
// Responsibilities:
//  * Construct and start the KeywordRecognizer
//  * Construct a map from keyword to Action
//  * Invoke the actions when a keyword is recognized

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }
    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    void BuildGameSpecificKeywordMap()
    {
        keywords.Add("Make Pop", () =>
        {
            Managers.HoloPops.MakeHoloPop();
        });

        keywords.Add("Destroy Pop", () =>
        {
            Managers.HoloPops.DestroyHoloPop();
        });
    }

    public void Startup(NetworkService service)
    {
        // Build the Keywords for this App
        BuildGameSpecificKeywordMap();

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
        status = ManagerStatus.Started;
    }
    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
}