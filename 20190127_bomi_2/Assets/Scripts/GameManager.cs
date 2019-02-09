using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject block; //유니티에서 scean 안에 만든건 다 gameobject임
    public GameObject lastBlock; //멈춘 블록중에 제일 위에 쌓여있는 블록
    public GameObject stackedBlock; //멈춘 블록중에 위에서 두번째 쌓여있는 블록 stackedblock < lastBlock
    public float error = 0.2f;

    bool isGameOver = false; //게임오버

    int spawnPosition=0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //새로운 블록 생성(움직이는 블록!!)
    void SpawnBlock()
    {
        //Debug.Log("click");

        //오브젝트를 생성함. 좌표를 넣으면 뒤에 Quaternion.identity 를 추가해야함
        GameObject newBlock = Instantiate(block, new Vector3(0, spawnPosition, 0), Quaternion.identity);

        //유니티 INSPECTOR에서 뷰하고 스크립트 비활성화한걸 newblock에다가 다시 스위치온 시켜주는것
        newBlock.SetActive(true);
        


        //새로생성될 블록의 위치 설정 = cutblock할때 중심을 바꿨어. 크기도 바꼈으니까. 그러니까 새로 생성될 블록의 시작위치도 바껴야지.
        newBlock.transform.position = lastBlock.transform.position + Vector3.up; // up (0,1,0)
        newBlock.transform.localScale = lastBlock.transform.localScale;



        //x랑 z랑 반복되게 생기게 하는거
        if (spawnPosition % 2 == 0)
            newBlock.GetComponent<BlockController>().currentState = MOVESTATE.MOVEX;//스크립트들이 다 컴포넌트
        else
            newBlock.GetComponent<BlockController>().currentState = MOVESTATE.MOVEZ;//스크립트들이 다 컴포넌트

        //생성한 블록은 마지막 블록에 넣음
        lastBlock = newBlock;
        spawnPosition++;
    }

    //블럭 정지
    void StopBlock()
    {
        try
        {
            lastBlock.GetComponent<BlockController>().currentState = MOVESTATE.STOP;
            CutBlock();
            stackedBlock = lastBlock;
        }
        catch
        {

        }
    }

    //삐져나온만큼 자르기, 잘린 불록 떨어트리기
    void CutBlock()
    {
        if (spawnPosition % 2 == 1)
        {
            //삐져나간 길이 계산
            float deltaX = stackedBlock.transform.position.x - lastBlock.transform.position.x;
            deltaX = Mathf.Abs(deltaX);

            if (deltaX > error && deltaX < stackedBlock.transform.localScale.x)
            {
                //잘린 후의 블록의 중심, 위치, 크기 다시계산
                float transCenter = (stackedBlock.transform.position.x + lastBlock.transform.position.x) / 2;
                lastBlock.transform.position = new Vector3(transCenter, lastBlock.transform.position.y, lastBlock.transform.position.z);
                lastBlock.transform.localScale = new Vector3(lastBlock.transform.localScale.x - deltaX, lastBlock.transform.localScale.y, lastBlock.transform.localScale.z);

                //잘리고 떨어지는 블록 부분 생성해서 떨어지도록 하는것
                if (lastBlock.transform.position.x > 0) //x축이 +방향일때
                {
                    GameObject fallingblock = Instantiate(block, new Vector3(transCenter + 1.5f, lastBlock.transform.position.y, lastBlock.transform.position.z), Quaternion.identity);
                    fallingblock.SetActive(true);
                    fallingblock.transform.localScale = new Vector3(3 - lastBlock.transform.localScale.x, lastBlock.transform.localScale.y, lastBlock.transform.localScale.z);
                    fallingblock.AddComponent<Rigidbody>();//giridbody 를 달면 물리연산을 해준다. 중력, 물리연산 등등, 충돌등 다됨.
                }
                else //x축이 -방향일때
                {
                    GameObject fallingblock = Instantiate(block, new Vector3(transCenter - 1.5f, lastBlock.transform.position.y, lastBlock.transform.position.z), Quaternion.identity);
                    fallingblock.SetActive(true);
                    fallingblock.transform.localScale = new Vector3(3 - lastBlock.transform.localScale.x, lastBlock.transform.localScale.y, lastBlock.transform.localScale.z);
                    fallingblock.AddComponent<Rigidbody>();//giridbody 를 달면 물리연산을 해준다. 중력, 물리연산 등등, 충돌등 다됨.
                }
            }
            else if (deltaX > stackedBlock.transform.localScale.x)
            {
                isGameOver = true;
                Debug.Log("GAME OVER!!");
            }
            else if (deltaX <= error) //오차가 0.2 보다 작으면 정답처리 해줌
            {
                lastBlock.transform.position = stackedBlock.transform.position + Vector3.up;
            }

        }
        else
        {
            float deltaZ = stackedBlock.transform.position.z - lastBlock.transform.position.z;
            deltaZ = Mathf.Abs(deltaZ);

            if (deltaZ > error && deltaZ < stackedBlock.transform.localScale.z)
            {
                float transCenter = (stackedBlock.transform.position.z + lastBlock.transform.position.z) / 2;
                lastBlock.transform.position = new Vector3(lastBlock.transform.position.x, lastBlock.transform.position.y, transCenter);
                lastBlock.transform.localScale = new Vector3(lastBlock.transform.localScale.x, lastBlock.transform.localScale.y, lastBlock.transform.localScale.z - deltaZ);

                //잘리고 떨어지는 블록부분 생성해서 떨어지도록 하는것
                if (lastBlock.transform.position.z > 0) //z축이 +방향일때
                {
                    GameObject fallingblock = Instantiate(block, new Vector3(lastBlock.transform.position.x, lastBlock.transform.position.y, transCenter + 1.5f), Quaternion.identity);
                    fallingblock.SetActive(true);
                    fallingblock.transform.localScale = new Vector3(lastBlock.transform.localScale.x, lastBlock.transform.localScale.y, 3 - lastBlock.transform.localScale.z);
                    fallingblock.AddComponent<Rigidbody>();//giridbody 를 달면 물리연산을 해준다. 중력, 물리연산 등등, 충돌등 다됨.
                }
                else //z축이 -방향일때
                {
                    GameObject fallingblock = Instantiate(block, new Vector3(lastBlock.transform.position.x, lastBlock.transform.position.y, transCenter - 1.5f), Quaternion.identity);
                    fallingblock.SetActive(true);
                    fallingblock.transform.localScale = new Vector3(lastBlock.transform.localScale.x, lastBlock.transform.localScale.y, 3 - lastBlock.transform.localScale.z);
                    fallingblock.AddComponent<Rigidbody>();//giridbody 를 달면 물리연산을 해준다. 중력, 물리연산 등등, 충돌등 다됨.
                }
            }
            else if (deltaZ > stackedBlock.transform.localScale.z)
            {
                isGameOver = true;
                Debug.Log("GAME OVER!!");
            }
            else if (deltaZ <= error) //오차가 0.2 보다 작으면 정답처리 해줌
            {
                lastBlock.transform.position = stackedBlock.transform.position + Vector3.up;
            }
        }
    }
 






    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //마우스 첫번째 클릭을 받음(배열)
        {
            StopBlock(); // 스톱할때 컷도함!! (cutblock())
            if (!isGameOver)//게임오버하면 더이상 블록을 쌓지 않고 카메라 올리지 않는다.
            {
                SpawnBlock();
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,
                                             8 + spawnPosition,
                                             Camera.main.transform.position.z);
            }
            else
            {
                Debug.Log("GAME OVER!!");
            }
        }
    }
}
