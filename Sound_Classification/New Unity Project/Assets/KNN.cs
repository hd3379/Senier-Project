using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static System.IO.Directory;

public class KNN : MonoBehaviour
{

    float[,] DataSet;
    int[] DataSet_Index;
    int num_ceps = 12; //data.GetLength(1) 의도대로 넘어왔으면 12임
    public static KNN staticKNN = new KNN();

    public int Classification(float[,] Newdata)  //0. 아무소리안냄(소음), 1. 인벤토리, 2. 시간이동
    {
        int num_frames = Newdata.GetLength(0);
        int k = 3; //k의 범위 선택(가까운 상대를 몇명이나 검색할 것인지)

        FileStream fs = new FileStream("SoundDataFile.txt", FileMode.Open);
        BinaryReader br = new BinaryReader(fs);

        int dataSize = br.ReadInt32();
        DataSet = new float[dataSize, num_ceps];
        DataSet_Index = new int[dataSize];
        for (int i = 0; i < dataSize; ++i)
        {
            for (int j = 0; j < num_ceps; ++j)
            {
                DataSet[i, j] = (float)br.ReadDouble();
            }
            DataSet_Index[i] = br.ReadInt32();
        }




        float[] min = new float[k];
        int[] neighbors_index = new int[k];
        int[] point = new int[k];
        int[] ceps_point = new int[k];
        int[] frames_point = new int[k];
        int max = 0;
        int maxindex = 0;
        int ceps_max = 0;
        int ceps_maxindex = 0;
        int frames_max = 0;
        int frames_maxindex = 0;
        //가까이에 있는 이웃들을 검색
        for (int i = 0; i < num_frames; ++i)
        {
            ceps_max = 0;
            for (int j = 0; j < num_ceps; ++j)
            {
                //min값 초기화
                for (int w = 0; w < k; ++w)
                {
                    min[w] = 99999;
                    neighbors_index[w] = 0;
                }
                if (Newdata[i, j] == -120) // 아무 소리 없을때 값이 -120
                {
                    continue;
                }
                //데이터를 싹 돌며 가까운 이웃 검색
                for (int q = 0; q < dataSize; ++q)
                {
                    for (int w = 0; w < k; ++w)
                    {
                        if (min[w] > Mathf.Abs(Newdata[i, j] - DataSet[q, j]))
                        {
                            min[w] = Mathf.Abs(Newdata[i, j] - DataSet[q, j]);
                            neighbors_index[w] = DataSet_Index[q];
                            
                            break;
                        }
                    }
                }

                //투표
                max = 0;
                for (int w = 0; w < k; ++w)
                {
                    if (min[w] == 99999)
                        continue;
                    point[neighbors_index[w]] += (k - w);
                    if (point[neighbors_index[w]] > max)
                    {
                        max = point[neighbors_index[w]];
                        maxindex = neighbors_index[w];
                    }
                }
                if (point[maxindex] == 0)
                    continue;
                //지역당선
                ceps_point[maxindex]++;
                if (ceps_point[maxindex] > ceps_max)
                {
                    ceps_max = ceps_point[maxindex];
                    ceps_maxindex = maxindex;
                }
            }
            //최종당선
            if (point[maxindex] == 0)
                continue;
            frames_max = 0;
            frames_point[ceps_maxindex]++;
            if (frames_point[maxindex] > frames_max)
            {
                frames_max = frames_point[ceps_maxindex];
                frames_maxindex = ceps_maxindex;
            }
        }
        br.Close();
        return frames_maxindex;
    }

    public void ClassificationSetting(float[,] Newdata, int answer)  //0. 아무소리안냄(소음), 1. 인벤토리, 2. 시간이동
    {
        int dataSize = 0;
        if (Exists("SoundDataFile.txt"))
        {
            FileStream fs = new FileStream("SoundDataFile.txt", FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            
            dataSize = br.ReadInt32();
            DataSet = new float[dataSize, num_ceps];
            DataSet_Index = new int[dataSize];

            for (int i = 0; i < dataSize; ++i)
            {
                for (int j = 0; j < num_ceps; ++j)
                {
                    DataSet[i, j] = (float)br.ReadDouble();
                }
                DataSet_Index[i] = br.ReadInt32();
            }
            br.Close();
            fs.Close();
        }
        FileStream newfs = new FileStream("SoundDataFile.txt", FileMode.Create);
        BinaryWriter bw = new BinaryWriter(newfs);

        int num_frames = Newdata.GetLength(0);
        int new_dataSize = dataSize + num_frames;

        bw.Write(new_dataSize);
        for (int i = 0; i < dataSize; ++i)
        {
            for (int j = 0; j < num_ceps; ++j)
            {
                bw.Write((double)DataSet[i, j]);
            }
            bw.Write(DataSet_Index[i]);
        }
        for (int i = 0; i < num_frames; ++i)
        {
            for (int j = 0; j < num_ceps; ++j)
            {
                bw.Write((double)Newdata[i, j]);
            }
            bw.Write(answer);
        }
        print("세이브완료");

        newfs.Close();
        bw.Close();
    }
}
