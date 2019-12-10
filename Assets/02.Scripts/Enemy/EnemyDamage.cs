using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    private float hp = 100.0f;

    private GameObject bloodEffect;

    private float initHp = 100.0f;

    // 생명 게이지 프리팹을 저장할 변수
    public GameObject hpBarPrefab;
    // 생명 게이지의 위치를 보정할 오프셋
    public Vector3 hpBarOffset = new Vector3(0, 2.2f, 0);
    // 부모가 될 Canvas 객체
    private Canvas uiCanvas;
    // 생명 수치에 따라 fillAmount 속성을 변경할 Image
    private Image hpBarImage;

    // Start is called before the first frame update
    void Start()
    {
        bloodEffect = Resources.Load<GameObject>("BulletImpactFleshBigEffect");

        uiCanvas = GameObject.Find("UI Canvas").GetComponent<Canvas>();
        // UI Canvas 하위로 생명 게이지를 생성
        GameObject hpBar = Instantiate<GameObject>(hpBarPrefab, uiCanvas.transform);
        // fillAmount 속성을 변경할 Image 를 추출
        hpBarImage = hpBar.GetComponentsInChildren<Image>()[1];

        // 생명 게이지에 따라가야할 대상과 오프셋 값 설정

        var hpBarComp = hpBar.GetComponent<EnemyHpBar>();
        hpBarComp.targetTr = transform;
        hpBarComp.offset = hpBarOffset;

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

                hpBarImage.fillAmount = hp / initHp;
                if (hp <= 0.0f)
                {
                    GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
                    hpBarImage.GetComponentsInChildren<Image>()[1].color = Color.clear;
                }
            }
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
