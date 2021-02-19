using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainFramework : MonoBehaviour
{

    private GameObject m_goMainPlayer;

    // Start is called before the first frame update
    void Start()
    {
        //Prefabs 폴더에 있는 Player를 지정한 위치로 불러온다
        m_goMainPlayer = GameObject.Instantiate(Resources.Load("Prefabs/Player"), new Vector3(-18, 2, -26), Quaternion.identity) as GameObject;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
