using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake() //START()보다 먼저 실행된다. awake>start>update 순으로 진행됨
    {
        if (instance == null)
        {
            instance = this; //자기자신을 인스턴스화 해서 사용?
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public GameObject block; //유니티에서 scean 안에 만든건 다 gameobject임
    public GameObject lastBlock; //멈춘 블록중에 제일 위에 쌓여있는 블록
    public GameObject stackedBlock; //멈춘 블록중에 위에서 두번째 쌓여있는 블록 stackedblock < lastBlock
    public Color previousColor;
    public Color nextColor;
    public float error = 0.2f;

    bool isGameOver = false; //게임오버

    int spawnPosition=0;

    int currentCombo = 0;
    int maxCombo;
    int score = 0;

    bool isSizeUp = false;
    Vector3 sizeUpVector = new Vector3(1.1f, 1, 1.1f);

    // Start is called before the first frame update
    void Start()
    {
        previousColor = RandomColor();
        nextColor = RandomColor();
        ChangeColor(stackedBlock);
        UIManager.instance.SetScore(score); //맨처음엔 0으로 설정해주기 위해서 스타트에도 넣는다.
    }

    //새로운 블록 생성(움직이는 블록!!)
    void SpawnBlock()
    {
        //Debug.Log("click");

        //오브젝트를 생성함. 좌표를 넣으면 뒤에 Quaternion.identity 를 추가해야함
        GameObject newBlock = Instantiate(block, new Vector3(0, spawnPosition, 0), Quaternion.identity);

        ChangeColor(newBlock);

        //유니티 INSPECTOR에서 뷰하고 스크립트 비활성화한걸 newblock에다가 다시 스위치온 시켜주는것
        newBlock.SetActive(true);
        


        //새로생성될 블록의 위치 설정 = cutblock할때 중심을 바꿨어. 크기도 바꼈으니까. 그러니까 새로 생성될 블록의 시작위치도 바껴야지.
        newBlock.transform.position = lastBlock.transform.position + Vector3.up; // up (0,1,0)
        //콤보체크해서 7보다 높아질때마다 사이즈를 10퍼센트씩 커지게 한다.
        if (isSizeUp)
        {
            newBlock.transform.localScale = new Vector3(lastBlock.transform.localScale.x * 1.1f, lastBlock.transform.localScale.y, lastBlock.transform.localScale.z * 1.1f);
            isSizeUp = false;
            currentCombo = 0;
        }
        else
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

            //현재크기의 20%까지는 정답처리 나머지는 에러 !! (error*lastBlock.transform.localScale.x)
            if (deltaX > (error*lastBlock.transform.localScale.x) && deltaX < stackedBlock.transform.localScale.x)
            {
                //잘린 후의 블록의 중심, 위치, 크기 다시계산
                float transCenter = (stackedBlock.transform.position.x + lastBlock.transform.position.x) / 2;
                lastBlock.transform.position = new Vector3(transCenter, lastBlock.transform.position.y, lastBlock.transform.position.z);
                lastBlock.transform.localScale = new Vector3(lastBlock.transform.localScale.x - deltaX, lastBlock.transform.localScale.y, lastBlock.transform.localScale.z);

                //잘리고 떨어지는 블록 부분 생성해서 떨어지도록 하는것
                if (lastBlock.transform.position.x > 0) //x축이 +방향일때
                {
                    GameObject fallingblock = Instantiate(block, new Vector3(transCenter + stackedBlock.transform.localScale.x / 2, lastBlock.transform.position.y, lastBlock.transform.position.z), Quaternion.identity);
                    fallingblock.SetActive(true);
                    fallingblock.transform.localScale = new Vector3(stackedBlock.transform.localScale.x - lastBlock.transform.localScale.x, lastBlock.transform.localScale.y, lastBlock.transform.localScale.z);
                    fallingblock.AddComponent<Rigidbody>();//giridbody 를 달면 물리연산을 해준다. 중력, 물리연산 등등, 충돌등 다됨.
                    ChangeColor(fallingblock);
                }
                else //x축이 -방향일때
                {
                    GameObject fallingblock = Instantiate(block, new Vector3(transCenter - stackedBlock.transform.localScale.x / 2, lastBlock.transform.position.y, lastBlock.transform.position.z), Quaternion.identity);
                    fallingblock.SetActive(true);
                    fallingblock.transform.localScale = new Vector3(stackedBlock.transform.localScale.x - lastBlock.transform.localScale.x, lastBlock.transform.localScale.y, lastBlock.transform.localScale.z);
                    fallingblock.AddComponent<Rigidbody>();//giridbody 를 달면 물리연산을 해준다. 중력, 물리연산 등등, 충돌등 다됨.
                    ChangeColor(fallingblock);
                }
                currentCombo = 0;//콤보 초기화
            }
            else if (deltaX > stackedBlock.transform.localScale.x)
            {
                isGameOver = true;
                Debug.Log("GAME OVER!!");
            }
            else if (deltaX <= (error * lastBlock.transform.localScale.x)) //오차가 0.2 보다 작으면 정답처리 해줌
            {
                if (!isGameOver)
                {
                    lastBlock.transform.position = stackedBlock.transform.position + Vector3.up;
                    CheckCombo();
                }
            }

        }
        else
        {
            float deltaZ = stackedBlock.transform.position.z - lastBlock.transform.position.z;
            deltaZ = Mathf.Abs(deltaZ);

            if (deltaZ > (error * lastBlock.transform.localScale.z) && deltaZ < stackedBlock.transform.localScale.z)
            {
                float transCenter = (stackedBlock.transform.position.z + lastBlock.transform.position.z) / 2;
                lastBlock.transform.position = new Vector3(lastBlock.transform.position.x, lastBlock.transform.position.y, transCenter);
                lastBlock.transform.localScale = new Vector3(lastBlock.transform.localScale.x, lastBlock.transform.localScale.y, lastBlock.transform.localScale.z - deltaZ);

                //잘리고 떨어지는 블록부분 생성해서 떨어지도록 하는것
                if (lastBlock.transform.position.z > 0) //z축이 +방향일때
                {
                    GameObject fallingblock = Instantiate(block, new Vector3(lastBlock.transform.position.x, lastBlock.transform.position.y, transCenter + stackedBlock.transform.localScale.z / 2), Quaternion.identity);
                    fallingblock.SetActive(true);
                    fallingblock.transform.localScale = new Vector3(lastBlock.transform.localScale.x, lastBlock.transform.localScale.y, stackedBlock.transform.localScale.z - lastBlock.transform.localScale.z);
                    fallingblock.AddComponent<Rigidbody>();//giridbody 를 달면 물리연산을 해준다. 중력, 물리연산 등등, 충돌등 다됨.
                    ChangeColor(fallingblock);
                }
                else //z축이 -방향일때
                {
                    GameObject fallingblock = Instantiate(block, new Vector3(lastBlock.transform.position.x, lastBlock.transform.position.y, transCenter - stackedBlock.transform.localScale.z / 2), Quaternion.identity);
                    fallingblock.SetActive(true);
                    fallingblock.transform.localScale = new Vector3(lastBlock.transform.localScale.x, lastBlock.transform.localScale.y, stackedBlock.transform.localScale.z - lastBlock.transform.localScale.z);
                    fallingblock.AddComponent<Rigidbody>();//giridbody 를 달면 물리연산을 해준다. 중력, 물리연산 등등, 충돌등 다됨.
                    ChangeColor(fallingblock);
                }
                currentCombo = 0;//콤보 초기화
            }
            else if (deltaZ > stackedBlock.transform.localScale.z)
            {
                isGameOver = true;
                Debug.Log("GAME OVER!!");
            }
            else if (deltaZ <= (error * lastBlock.transform.localScale.z)) //오차가 현재크기보다 20% 작으면 정답처리 해줌
            {
                if (!isGameOver)
                {
                    lastBlock.transform.position = stackedBlock.transform.position + Vector3.up;
                    CheckCombo();
                }
            }
        }
    }
 
    void CheckCombo()
    {
        currentCombo++;
        if(currentCombo>=7)
        {
            isSizeUp = true;
        }
    }

    Color RandomColor()
    {
        float r = Random.Range(100f, 200f) / 255f; // 255로 무조건 나눠줘야함
        float g = Random.Range(100f, 200f) / 255f;
        float b = Random.Range(100f, 200f) / 255f;
        return new Color(r, g, b);
    }

    void ChangeColor(GameObject go)
    {
        //10개마다 색을바꿔주는데(/10f) 서서히 변하게 만들어줄것 previousColor에서 nextColor컬러까지의 사이 개수(score % 11)만큼 변경시켜주고..
        Color applyColor = Color.Lerp(previousColor, nextColor, (score % 11) / 10f);
        //오브젝터 렌더링하는거(시각적으로 표현하는거)
        Renderer render = go.GetComponent<Renderer>();
        render.material.color = applyColor;
        Camera.main.backgroundColor = applyColor - new Color(0.1f, 0.1f, 0.1f);
        if(applyColor.Equals(nextColor)==true)
        {
            previousColor = nextColor;
            nextColor = RandomColor();
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
                score++; //점수증가
                UIManager.instance.SetScore(score);
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
