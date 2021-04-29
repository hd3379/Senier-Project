using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;

public class ReadSound : MonoBehaviour
{
    public AudioClip aud;
    float[] audSignalData;
    // Start is called before the first frame update
    void Start()
    {
        audSignalData = new float[aud.samples * aud.channels];
        aud.GetData(audSignalData, 0);
        print(aud.samples); //signal 수
        print(aud.channels); //채널수
        print(aud.length); //오디오 길이(시간)

        //PREEMPHASIS(aud.samples);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //사람 목소리가 고주파보다 저주파 에너지가 더 많은 경향때문에
    //고주파 성분의 에너지를 조금 올려주면 음성인식 성능 개선이 된다고함
    //그걸위한 PRE EMPHASIS 
    void PREEMPHASIS(int samples) 
    {
        float pre_emphasis = 0.97f;
        for(int i = 0; i < samples; ++i)
        {
            audSignalData[i] = audSignalData[i] - pre_emphasis * audSignalData[i-1];
            print(audSignalData[i]);
        }
    }
}
