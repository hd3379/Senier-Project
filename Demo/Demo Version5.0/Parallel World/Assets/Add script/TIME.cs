using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TIME : MonoBehaviour
{
    public Text missiontext;
    private float GameTime = 1;
    private float min = 30;
  
    void Start()
    {
        
    }

 
    void Update()
    {
        GameTime -= Time.deltaTime;


        //if((int)GameTime == 60)
        //{
        //   GameTime = 0;
        //    min +=1;
        //    GameTime += Time.deltaTime;

        //}

        //if ((int)GameTime < 10)
        //{
        //    missiontext.text = min + " : " + "0" + (int)GameTime;
        //}
        //else
        //{
        //    missiontext.text = min + " : " + (int)GameTime;
        //}

        if ((int)GameTime == 0)
        {
            GameTime = 60;
            min -= 1;
            GameTime -= Time.deltaTime;

        }

        if ((int)GameTime < 10)
        {
            missiontext.text = min + " : " + "0" + (int)GameTime;
        }
        else
        {
            missiontext.text = min + " : " + (int)GameTime;
        }
    }
}
