using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Record : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    AudioClip recordClip;
    void StartRecordMicrophone() { 
        recordClip = Microphone.Start(null, true, 100, 44100);
    }

    void StopRecordMicrophone() {
        int lastTime = Microphone.GetPosition(null); 
        if (lastTime == 0) return;
        else {
            Microphone.End(Microphone.devices[0]);
            float[] samples = new float[recordClip.samples]; 
            recordClip.GetData(samples, 0);
            float[] cutSamples = new float[lastTime];
            Array.Copy(samples, cutSamples, cutSamples.Length - 1);
            recordClip = AudioClip.Create("Notice", cutSamples.Length, 1, 44100, false);
            recordClip.SetData(cutSamples, 0); 
        } 
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartRecordMicrophone();
        }
        if(Input.GetKeyUp(KeyCode.A))
        {
            StopRecordMicrophone();
            ReadSound.staticReadSound.aud = recordClip;
            ReadSound.staticReadSound.Classification();
        }
    }
}
