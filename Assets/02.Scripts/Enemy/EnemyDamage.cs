using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    private float hp = 100.0f;

    private GameObject bloodEffect;

    // Start is called before the first frame update
    void Start()
    {
        bloodEffect = Resources.Load<GameObject>("BulletImpactFleshBigEffect");
    }

    void ShowBloodEffect(Collision coll)
    {
        // 총알이 충돌한 지점 산출
        Vector3 pos = coll.contacts[0].point;
        // 총알의 충돌했을 때의 법선 벡터
        Vector3 normal = coll.contacts[0].normal;
        // 총알의 충돌 시 방향 벡터의 회전값 계산
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, normal);

        // 혈흔 효과 생성
        GameObject bloodObj = Instantiate<GameObject>(bloodEffect, pos, rot);
        Destroy(bloodObj, 1.0f);
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == bulletTag)
        {
            // 혈흔 효과를 생성하는 함수 호출
            ShowBloodEffect(collision);
            BulletCtrl bulletCtrl = collision.gameObject.GetComponent<BulletCtrl>();
            if (bulletCtrl != null)
            {
                hp -= collision.gameObject.GetComponent<BulletCtrl>().damage;
                if (hp <= 0.0f)
                {
                    GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
                }
            }
            Destroy(collision.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
