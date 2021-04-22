using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class VoiceRecogn : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    void Start()
    {
        //initialize stuff
        keywords.Add("jump", () =>
         {
             GoCalled();
         });
        keywords.Add("Hello", () =>
        {
            HelloCalled();
        });
        keywords.Add("SiGan", () =>
        {
            RiceCalled();
        });
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecogizerOnPhraseRecognized;
        keywordRecognizer.Start();
    }

    void KeywordRecogizerOnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;

        if(keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
    
    void GoCalled()
    {
        print("You just said GO");
    }

    void RiceCalled()
    {
        print("You just said 시간");
    }

    void HelloCalled()
    {
        print("You just said Hello");
    }
}
