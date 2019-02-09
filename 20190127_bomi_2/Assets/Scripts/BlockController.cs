using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    bool toRight = true;        //0축 기준 이동 방향
    public float moveSpeed = 1; //이동 속도

    public MOVESTATE currentState = MOVESTATE.STOP;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case MOVESTATE.MOVEX:
                MoveX();
                break;
            case MOVESTATE.MOVEZ:
                MoveZ();
                break;
            case MOVESTATE.STOP:
                break;
            default:
                break;
        }
    }

    void MoveX()
    {
        if (toRight)
        {
            this.transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime * moveSpeed);
            if (this.transform.position.x > 5)
            {
                toRight = false;
            }
        }
        else
        {
            this.transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * moveSpeed);
            if (this.transform.position.x < -5)
            {
                toRight = true;
            }
        }
    }
    void MoveZ()
    {
        if (toRight)
        {
            this.transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime * moveSpeed);
            if (this.transform.position.z > 5)
            {
                toRight = false;
            }
        }
        else
        {
            this.transform.Translate(new Vector3(0, 0, -1) * Time.deltaTime * moveSpeed);
            if (this.transform.position.z < -5)
            {
                toRight = true;
            }
        }
    }
}
