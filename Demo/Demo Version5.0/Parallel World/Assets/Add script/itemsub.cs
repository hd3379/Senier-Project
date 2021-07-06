using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class itemsub : MonoBehaviour
{

    public Text keytext; //아이템설명 텍스트
    public Text maptext; //아이템설명 텍스트

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void m()
    {
        keytext.gameObject.SetActive(false);

    }

    void m2()
    {
        maptext.gameObject.SetActive(false);

    }

    public void Keysub()
    {
        keytext.text = "누군가가 놓고간 카드키";

        Invoke("m", 9.0f);
    }

    public void Mapsub()
    {
        maptext.text = "아파트 구조가 적힌 지도";

        Invoke("m2", 9.0f);
    }



}
