using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.InputSystem;

namespace BNG
{
    public class Record : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }
        AudioClip recordClip;
        void StartRecordMicrophone()
        {
            recordClip = Microphone.Start(null, true, 100, 44100);
        }

        void StopRecordMicrophone()
        {
            int lastTime = Microphone.GetPosition(null);
            if (lastTime == 0) return;
            else
            {
                Microphone.End(Microphone.devices[0]);
                float[] samples = new float[recordClip.samples];
                recordClip.GetData(samples, 0);
                float[] cutSamples = new float[lastTime];
                Array.Copy(samples, cutSamples, cutSamples.Length - 1);
                recordClip = AudioClip.Create("Notice", cutSamples.Length, 1, 44100, false);
                recordClip.SetData(cutSamples, 0);
            }
        }
        public Text changetext; //미션 텍스트 수정변수

        // Update is called once per frame
        void Update()
        {
          /*  //  if (Input.GetKeyDown(KeyCode.K))
            if (InputBridge.Instance.XButton)
            {
                StartRecordMicrophone();
                int word = 0;
                //StopRecordMicrophone();
                ReadSound.staticReadSound.aud = recordClip;
                word = ReadSound.staticReadSound.Classification();
                if (word == 1)
                {
                    changetext.text = "인벤토리";
                }
                else if (word == 2)
                {
                    changetext.text = "시간이동";
                }
            }*/
            //if (Input.GetKeyUp(KeyCode.K))
            //{
            //    int word = 0;
            //    //StopRecordMicrophone();
            //    ReadSound.staticReadSound.aud = recordClip;
            //    word = ReadSound.staticReadSound.Classification();
            //    if (word == 1)
            //    {
            //        changetext.text = "인벤토리";
            //    }
            //    else if (word == 2)
            //    {
            //        changetext.text = "시간이동";
            //    }
            //}
        }
    }

}