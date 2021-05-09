using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;
using System.Linq;
using System.Numerics;
using System;


public class ReadSound : MonoBehaviour
{
    public AudioClip aud;
    float[] audSignalData;
    float[,] frames;
    // Start is called before the first frame update
    void Start()
    {
        audSignalData = new float[aud.samples * aud.channels];
        aud.GetData(audSignalData, 0);
        print("signal 수" + aud.samples); //signal 수
        print("채널수 " + aud.channels); //채널수
        print("오디오 길이 (초)" + aud.length); //오디오 길이(seconds 단위의 시간)
        print("오디오 주파수" + aud.frequency); //오디오 주파수

        //PreEmpasis();
        //Framing(); //Windowing()은 Framing안에서
        Complex[] sampleArray = { 0.2, 0.7, 0.5, 0.3, 0.1, 0, 0, 0 };
        Complex[] input = new Complex[8];
        input = sampleArray;
        Complex[] output = null;
        fft(input, ref output);
        float[] mag_frames = new float[8];


        for (int i = 0; i < output.Length; ++i)
        {
            mag_frames[i] = (float)output[i].Magnitude;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*사람 목소리가 고주파보다 저주파 에너지가 더 많은 경향때문에
    고주파 성분의 에너지를 조금 올려주면 음성인식 성능 개선이 된다고함
    그걸위한 PRE EMPHASIS */
    void PreEmpasis()
    {
        float pre_emphasis = 0.97f;
        for (int i = 0; i < aud.samples; ++i)
        {
            audSignalData[i] = audSignalData[i] - pre_emphasis * audSignalData[i - 1];
        }
    }

    /*분석 대상 시간 구간이 지나치게 길 경우 빠르게 변화하는 신호의 주파수 정보를 정확히 캐치하기 힘들다
    하여 신호가 변화하지 않다고 느낄만큼 짧은 시간단위로 쪼갤건데 이과정이 Framing*/
    void Framing()
    {
        float frame_size = 0.025f; //25ms
        float frame_stride = 0.01f; //10ms 겹치는 구간
        int signal_length = audSignalData.Length;
        int frame_length = Mathf.RoundToInt(aud.frequency * frame_size);
        //length = 한 프레임의길이
        int frame_step = Mathf.RoundToInt(frame_stride * aud.frequency);
        //step = 한 프레임마다 건너뛰는 정도
        int num_frames = (int)(Mathf.CeilToInt((float)Mathf.Abs(
            signal_length - frame_length)) / frame_step);

        int pad_signal_length = num_frames * frame_step + frame_length;

        float[] pad_signal = new float[pad_signal_length];
        for (int i = 0; i < pad_signal_length; ++i)
        {
            if (i < audSignalData.Length)
                pad_signal[i] = audSignalData[i];
            else
                pad_signal[i] = 0;
        }

        frames = new float[num_frames, frame_length];
        for (int i = 0; i < num_frames; ++i)
        {
            for (int j = 0; j < frame_length; ++j)
            {
                frames[i, j] = pad_signal[j + (i * frame_step)];
            }
        }
        Windowing(num_frames, frame_length);
    }

    /*잘게 잘르는 framing을 거친 frames들을 windowing 할건데 개별 프레임에 Hamming Window를
     적용하면 양 끝 부분은 0애 가까운 값이 곱해져 그 값이 작아집니다. 이 과정을 통해 우리가 원하는
    주파수 영역대의 정보는 더 캐치하고 버려야 하는 주파수 영역대 정보도 일부 캐치할수 있게 됩니다.
     */
    void Windowing(int num_frames, int frame_length)
    {
        for (int i = 0; i < num_frames; ++i)
        {
            for (int j = 0; j < frame_length; ++j)
            {
                frames[i, j] *= (float)(0.54 - (0.46 * Mathf.Cos((2 * Mathf.PI * j) / (frame_length - 1))));
            }
        }
    }
    /*FourerTransform = 시간 도메인의 음성 신호를 주파수 도메인으로 바꾸는 과정
     주기함수 또는 신호를 삼각함수로 표현하는 과정?
    임의의 입력 신호를 주기함수들의 합으로 분해하여 표현 하는 것이라 한다.
     */
    //fft 값이 약간 이상할 확률 높음 일단 진행해 보겠다.
    void fft(Complex[] input, ref Complex[] output)
    {
        if (input.Length < 2)
        {
            return;
        }
        int N = input.Length;
        N = 1 << ((int)Mathf.Round((Mathf.Log(N, 2))));
        int rounds = (int)Mathf.Log(N, 2);

        if (output == null || output.Length != N)
        {
            output = new Complex[N];
            output.Initialize();
        }

        permutate(rounds, input, ref output);
        for (int i = 0; i < 5; i++)
        {
            print(i + "= " + output[i]);
        }

        Complex[] temp = new Complex[N];
        Complex[] Swaptmp = new Complex[N];

        temp = output;
        for (int q = rounds - 1; q >= 0; q--)
        {
            Swaptmp = output;
            output = temp;
            temp = Swaptmp;
            computeFFT(q, temp, ref output);
        }


        temp.Initialize();
        for (int i = 0; i < output.Length; ++i)
        {
            print("전" + output[i]);
            output[i] = output[i] / Complex.Pow(Mathf.Sqrt(N), 0.0);
            print("후" + output[i]);
        }
    }

    int Reverse(int n, int k)
    {
        int r, i;
        for (r = 0, i = 0; i < k; ++i)
            r |= ((n >> i) & 1) << (k - i - 1);
        return r;
    }
    void permutate(int q, Complex[] input, ref Complex[] output)
    {
        for (int i = 0; i < (1 << q); ++i)
        {
            output[i] = input[Reverse(i, q)];
        }
    }
    void computeFFT(int q, Complex[] input, ref Complex[] output)
    {
        int countBlock = 1 << q;
        int lenBlock = output.Length / countBlock;

        for (int baseIdx = 0; baseIdx < lenBlock / 2; baseIdx++)
        {

            Complex w = Complex.FromPolarCoordinates(1.0, -(
                Mathf.PI * baseIdx) / (lenBlock / 2));
            for (int block = 0; block < countBlock; block++)
            {
                int idx = baseIdx + block * lenBlock;
                Complex a = (Complex)input[idx];
                Complex b = (Complex)input[idx + lenBlock / 2];
                Complex wB = Complex.Multiply(w, b);

                output[idx] = Complex.Add(a, wB);
                output[idx + lenBlock / 2] = Complex.Subtract(a, wB);
            }
        }
    }

}
