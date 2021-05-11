using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;
using System.Linq;
using System.Numerics;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public class ReadSound : MonoBehaviour
{
    public AudioClip aud;
    float[] audSignalData;
    float[,] frames;
    float[,] fft_frames;
    int num_frames;
    int frame_step;
    int frame_length;
    int NFFT;
    // Start is called before the first frame update
    void Start()
    {
        MFCC();
    }

    void MFCC()
    {
        audSignalData = new float[aud.samples * aud.channels];
        aud.GetData(audSignalData, 0);
        print("signal 수" + aud.samples); //signal 수
        print("채널수 " + aud.channels); //채널수
        print("오디오 길이 (초)" + aud.length); //오디오 길이(seconds 단위의 시간)
        print("오디오 주파수" + aud.frequency); //오디오 주파수

        PreEmpasis();
        Framing();
        Windowing();

        if (frame_length < 2)
        {
            return;
        }
        int N = frame_length;
        N = 1 << ((int)Mathf.Round((Mathf.Log(N, 2))));
        NFFT = N;

        Complex[] output = new Complex[N];
        fft_frames = new float[num_frames, N];
        output.Initialize();

        Complex[] input = new Complex[N];
        float[] mag_frames = new float[N];
        float[] pow_frames = new float[N];

        for (int i = 0; i < num_frames; ++i)
        {
            for (int j = 0; j < N; ++j)
            {
                if(j >= frame_length)
                {
                    input[j] = 0;
                    continue;
                }
                input[j] = frames[i, j];
            }
            fft(input, ref output);
            for (int j = 0; j < N; ++j)
            {
                mag_frames[j] = (float)output[j].Magnitude; //위상정보는 없애고 진폭 정보만 남김
                pow_frames[j] = (float)(Mathf.Pow(mag_frames[j], 2) / mag_frames.Length);//power 스펙트럼으로 바꿔줌

                fft_frames[i,j] = pow_frames[j];
            }
            //print(fft_frames[i, 0]);
        }

        int num_ceps = 12;
        MelScaleFilter(aud.frequency);
    }

    static float[] ToComplex(float[] real)
    {
        int n = real.Length;
        var comp = new float[n * 2];
        for(int i = 0; i< n; i++)
        {
            comp[2 * i] = real[i];
        }
        return comp;
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
        for (int i = 1; i < aud.samples; ++i)
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
        frame_length = Mathf.RoundToInt(aud.frequency * frame_size);
        //length = 한 프레임의길이
        frame_step = Mathf.RoundToInt(frame_stride * aud.frequency);
        //step = 한 프레임마다 건너뛰는 정도
        num_frames = (int)(Mathf.CeilToInt((float)Mathf.Abs(
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
    }

    /*잘게 잘르는 framing을 거친 frames들을 windowing 할건데 개별 프레임에 Hamming Window를
     적용하면 양 끝 부분은 0애 가까운 값이 곱해져 그 값이 작아집니다. 이 과정을 통해 우리가 원하는
    주파수 영역대의 정보는 더 캐치하고 버려야 하는 주파수 영역대 정보도 일부 캐치할수 있게 됩니다.
     */
    void Windowing()
    {
        for (int i = 0; i < num_frames; ++i)
        {
            for (int j = 0; j < frame_length; ++j)
            {
                frames[i, j] *= (float)(0.54 - (0.46 * Mathf.Cos((2 * Mathf.PI * j) / (frame_length - 1))));
            }
        }
    }

    void dft (Complex[] input, ref Complex[] output)
    {
        if (input.Length < 2)
        {
            return;
        }
        int N = input.Length;
        N = 1 << ((int)Mathf.Round((Mathf.Log(N, 2))) + 1);
        int rounds = (int)Mathf.Log(N, 2);
        //NFFT = frame_length;

        if (output == null || output.Length != N)
        {
            output = new Complex[frame_length];
            output.Initialize();
        }

        for (int i = 0; i < frame_length; i++)
        {
            Complex tmp = (Complex)(0.0);
            for (int j = 0; j < frame_length; j++)
            {
                Complex wj = Complex.FromPolarCoordinates(1.0, -(2.0 * Mathf.PI * i * j) / N);
                tmp = Complex.Add(tmp, Complex.Multiply(wj, input[j]));
            }
            output[i] = tmp;
        }
        
        for (int i = 0; i < frame_length; ++i)
        {
            output[i] = output[i] / Complex.Pow(Mathf.Sqrt(N), 0.0);
        }
    }
    /*FourerTransform = 시간 도메인의 음성 신호를 주파수 도메인으로 바꾸는 과정
     주기함수 또는 신호를 삼각함수로 표현하는 과정?
    임의의 입력 신호를 주기함수들의 합으로 분해하여 표현 하는 것이라 한다.
     */
    //fft 값이 약간 이상할 확률 높음 일단 진행해 보겠다.
    void fft(Complex[] input, ref Complex[] output)
    {

        int rounds = (int)Mathf.Log(NFFT, 2);
        permutate(rounds, input, ref output);

        Complex[] temp = new Complex[NFFT];
        Complex[] Swaptmp = new Complex[NFFT];

        temp = output;
        for (int q = rounds - 1; q >= 0; q--)
        {
            Swaptmp = output;
            output = temp;
            temp = Swaptmp;
            computeFFT(q, temp, ref output);
        }


        temp.Initialize();
        for (int i = 0; i < NFFT; ++i)
        {
            output[i] = output[i] / Complex.Pow(Mathf.Sqrt(NFFT), 0.0);
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

    void MelScaleFilter(float fre)
    {
        int nfilt = 40;
        float low_freq_mel = 0;
        float high_freq_mel = (2595 * Mathf.Log10(1 + (fre) / 700)); //Convert Hz to me
        float[] mel_points = new float[nfilt + 2];
        float mel_point = low_freq_mel;
        for (int i = 0; i < nfilt + 2; ++i) {
            mel_points[i] = mel_point;
            mel_point += ((high_freq_mel - low_freq_mel) / (nfilt + 2));
        } // Equally spaced in Mel scale

        float[] hz_points = new float[nfilt + 2];
        for (int i = 0; i < nfilt + 2; ++i)
        {
            hz_points[i] = (700 * (Mathf.Pow(10, (mel_points[i] / (float)2595)) - 1));
        }// Convert Mel to Hz

        float[] bin = new float[nfilt + 2];
        for (int i = 0; i < nfilt + 2; ++i)
        {
            bin[i] = Mathf.Floor((NFFT + 1) * hz_points[i] / fre);
        }

        float[,] fbank = new float[nfilt, (int)NFFT];
        int f_m_minus, f_m, f_m_plus;
        for (int m = 1; m < nfilt + 1; ++m)
        {
            f_m_minus = (int)bin[m - 1];
            f_m = (int)bin[m];
            f_m_plus = (int)bin[m + 1];
            for (int k = f_m_minus; k < f_m; ++k)
            {
                fbank[m - 1, k] = (k - bin[m - 1]) / (bin[m] - bin[m - 1]);
            }
            for (int k = f_m; k < f_m_plus; ++k)
            {
                fbank[m - 1, k] = (bin[m + 1] - k) / (bin[m + 1] - bin[m]); 
            }
        }

        fft_frames = Multm1andm2T(fft_frames, fbank);
    }

    public static float[,] Multm1andm2T(float[,] m1, float[,] m2)
    {
        float[,] result = new float[m1.GetLength(0), m2.GetLength(0)];

        print(result.GetLength(0));
        print(result.GetLength(1));
        if (m1.GetLength(1) == m2.GetLength(1))
        {
            print("hi");
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < m1.GetLength(1); k++)
                        result[i, j] = result[i, j] + m1[i, k] * m2[j, k];
                }
            }
        }
        return result;
    }

    public double[,] GenerateDCTmatrix(int order)
    {
        int i, j;
        int N;
        N = order;
        double alpha;
        double denominator;
        double[,] DCTCoeff = new double[N, N];
        for (j = 0; j <= N - 1; j++)
        {
            DCTCoeff[0, j] = Math.Sqrt(1 / (double)N);
        }
        alpha = Math.Sqrt(2 / (double)N);
        denominator = (double)2 * N;
        for (j = 0; j <= N - 1; j++)
            for (i = 1; i <= N - 1; i++)
            {
                DCTCoeff[i, j] = alpha * Math.Cos(((2 * j + 1) *
            i * 3.14159) / denominator);
            }

        return (DCTCoeff);
    }
}
