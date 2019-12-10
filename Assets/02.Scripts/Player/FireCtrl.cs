using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public struct PlayerSfx
{
    public AudioClip[] fire;
    public AudioClip[] reload;
}

public class FireCtrl : MonoBehaviour
{
    public enum WeaponType
    {
        RIFLE,
        SHOTGUN
    }

    public WeaponType currWeapon = WeaponType.RIFLE;
    public PlayerSfx playerSfx;

    private AudioSource audioSource;

    public GameObject bullet;
    public Transform firePos;

    public ParticleSystem cartridge;
    private ParticleSystem muzzleFlash;

    private Shake shake;

    // 탄창 Image UI
    public Image magazineImg;
    // 남은 총알 수 Text UI
    public Text magazineText;

    // 최대 총알 수
    public int maxBullet = 10;
    // 남은 총알 수
    public int remainingBullet = 10;

    //재장전 시간
    public float reloadTime = 2.0f;
    //재장전 여부를 판단할 변수
    private bool isReloading = false;

    // 변경할 무기 이미지
    public Sprite[] weaponIcons;
    // 교체할 무기 이미지 UI
    public Image weaponImage;


    private void Start()
    {
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (!isReloading && Input.GetMouseButtonDown(0))
        {
            --remainingBullet;
            Fire();

            if (remainingBullet == 0)
            {
                StartCoroutine(Reloading());
            }
        }

    }

    void Fire()
    {
        StartCoroutine(shake.ShakeCamera());
        //Instantiate(bullet, firePos.position, firePos.rotation);
        var bulletObj = GameManager.instance.GetBullet();
        if (bulletObj != null)
        {
            bulletObj.transform.position = firePos.position;
            bulletObj.transform.rotation = firePos.rotation;
            bulletObj.SetActive(true);
        }

        cartridge.Play();
        muzzleFlash.Play();
        FireSfx();
        //재장전 이미지의 fillAmount 속성값 지정
        magazineImg.fillAmount = (float)remainingBullet / (float)maxBullet;
        // 남은 총알 수 갱신
        UpdateBulletText();


    }

    void FireSfx()
    {
        var audioClip = playerSfx.fire[(int)currWeapon];
        audioSource.PlayOneShot(audioClip, 1.0f);
    }

    void UpdateBulletText()
    {
        magazineText.text = string.Format("< color =#ff0000>{0}</color>/{1}", remainingBullet, maxBullet);
    }

    IEnumerator Reloading()
    {
        isReloading = true;
        audioSource.PlayOneShot(playerSfx.reload[(int)currWeapon], 1.0f);

        // 재장전 오디오의 길이 + 0.3초 동안 대기
        yield return new WaitForSeconds(playerSfx.reload[(int)currWeapon].length + 0.3f);

        // 각종 변수값의 초기화
        isReloading = false;
        magazineImg.fillAmount = 1.0f;
        remainingBullet = maxBullet;

        UpdateBulletText();
    }

    public void OnChangeWeapon()
    {
        currWeapon = (WeaponType)((int)++currWeapon % 2);
        weaponImage.sprite = weaponIcons[(int)currWeapon];
    }
}
