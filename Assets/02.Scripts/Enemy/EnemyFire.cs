using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    private AudioSource audioSource;
    private Animator animator;
    private Transform playerTr;
    private Transform enemyTr;

    private readonly int hashFire = Animator.StringToHash("Fire");
    private readonly int hashReload = Animator.StringToHash("Reload");

    // 다음 발사할 시간 계산용 변수
    private float nextFire = 0.0f;

    // 총알 발사 간격
    private readonly float fireRate = 0.1f;

    // 주인공을 향해 회전할 속도 계수
    private readonly float damping = 10.0f;

    private readonly float reloadTime = 2.0f;
    private readonly int maxBullet = 10;
    private int currBullet = 10;
    private bool isReload = false;
    private WaitForSeconds wsReload;


    public bool isFire = false;

    public AudioClip fireSfx;
    public AudioClip reloadSfx;

    //적 캐릭터의 총알 프리랩
    public GameObject bullet;
    // 총알 발사 위치 정보
    public Transform firePos;

    public MeshRenderer muzzleFlashRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();
        enemyTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        wsReload = new WaitForSeconds(reloadTime);
        muzzleFlashRenderer.enabled = false;
    }

    void Fire()
    {
        animator.SetTrigger(hashFire);
        audioSource.PlayOneShot(fireSfx, 1.0f);

        StartCoroutine(ShowMuzzleFlash());

        GameObject bulletObj = Instantiate(bullet, firePos.position, firePos.rotation);
        Destroy(bulletObj, 3.0f);

        isReload = (--currBullet % maxBullet == 0);
        if (isReload)
        {
            StartCoroutine(Reloading());
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!isReload && isFire)
        {
            if (Time.time >= nextFire)
            {
                Fire();
                nextFire = Time.time + fireRate + Random.Range(0.0f, 0.3f);
            }

            Quaternion rot = Quaternion.LookRotation(playerTr.position - enemyTr.position);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }
    }

    IEnumerator Reloading()
    {
        muzzleFlashRenderer.enabled = false;
        animator.SetTrigger(hashReload);
        audioSource.PlayOneShot(reloadSfx, 1.0f);

        // 재장전 시간만큼 대기하는 동안 제어권 양보
        yield return wsReload;

        // 총알 개수 초기화
        currBullet = maxBullet;
        isReload = false;
    }

    IEnumerator ShowMuzzleFlash()
    {
        muzzleFlashRenderer.enabled = true;

        Quaternion rot = Quaternion.Euler(Vector3.forward * Random.Range(0, 360));
        muzzleFlashRenderer.transform.localRotation = rot;
        muzzleFlashRenderer.transform.localScale = Vector3.one * Random.Range(1.0f, 2.0f);

        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        muzzleFlashRenderer.material.SetTextureOffset("_MainTex", offset);

        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
        muzzleFlashRenderer.enabled = false;
    }


}
