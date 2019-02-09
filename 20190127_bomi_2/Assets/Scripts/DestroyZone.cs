using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision) //물체가 충돌할 때!를 정한다. 이벤트함수야. 유니티엔진이 지원해줌.collision이 충돌한 상대!
    {
        Destroy(collision.gameObject);
    }
}
