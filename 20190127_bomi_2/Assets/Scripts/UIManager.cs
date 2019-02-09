using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; //textmeshpro 선언하기 위해서

public class UIManager : MonoBehaviour
{
    public static UIManager instance; //UI매니저를 정적변수로 만들어서
    private void Awake() //START()보다 먼저 실행된다. awake>start>update 순으로 진행됨
    {
        if (instance==null)
        {
            instance = this; 
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public TextMeshPro scoreText; //public 으로 선언하면 유니티 인스펙터에 표시됨!!! 까먹지마

    public void SetScore(int value)
    {
        scoreText.text = value.ToString();
    }
}
