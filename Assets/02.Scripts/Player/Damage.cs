using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    private const string enemyTag = "ENEMY";
	private float initHp = 100.0f;
    private float currHp;

    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    public Image bloodScreen;
    // Hp Bar Image 를 저장하기 위한 변수
    public Image hpBar;
    // 생명 게이지의 처음 색상(녹색)
    private readonly Color initColor = new Vector4(0, 1.0f, 0.0f, 1.0f);
    private Color currColor;


    // Start is called before the first frame update
    void Start()
    {
        currHp = initHp;
        hpBar.color = initColor;
        currColor = initColor;

    }

    void PlayerDie()
    {
        OnPlayerDie();
        //Debug.Log("PlayerDie !");
        ////"ENEMY" 태그로 지정된 모든 적 캐릭터를 추출해 배열에 저장
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        ////배열의 처음부터 순회하면서 적 캐릭터의 OnPlayerDie 함수를 호출
        //for (int i = 0; i < enemies.Length; ++i)
        //{
        //    enemies[i].SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == bulletTag)
        {
            Destroy(other.gameObject);

            StartCoroutine(ShowBloodScreen());

            currHp -= 5.0f;
            Debug.Log("Player HP = " + currHp.ToString());

            DisplayHpbar();


            if (currHp <= 0.0f)
            {
                PlayerDie();
            }
        }
    }

    IEnumerator ShowBloodScreen()
    {
        //BloodScreen 텍스처의 알파값을 불규칙하게 변경
        bloodScreen.color = new Color(1, 0, 0, Random.Range(0.2f, 0.3f));
        yield return new WaitForSeconds(0.1f);
        //BloodScreen 텍스처의 색상을 모두 0으로 변경
        bloodScreen.color = Color.clear;
        Gizmos.color = Color.green;
    }

    void DisplayHpbar()
    {
        //생명 수치가 50% 일 때까지는 녹색에서 노란색으로 변경
        if ((currHp / initHp) > 0.5f)
            currColor.r = (1 - (currHp / initHp)) * 2.0f;
	    else // 생명 수치가 0% 일 때까지는 노란색에서 빨간색으로 변경
		    currColor.g = (currHp / initHp) * 2.0f;

        // hpBar 의 색상 변경
        hpBar.color = currColor;
        // hpBar 의 크기 변경
        hpBar.fillAmount = (currHp / initHp);
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
