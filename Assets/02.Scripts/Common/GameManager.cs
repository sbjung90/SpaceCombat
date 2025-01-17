﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DataInfo;

public class GameManager : MonoBehaviour
{
    [Header("Enemy Create Info")]
    // 적 캐릭터가 출현할 위치를 담을 배열
    public Transform[] points;
    // 적 캐릭터 프리팹을 저장할 변수
    public GameObject enemy;
    // 적 캐릭터를 생성할 주기
    public float createTime = 2.0f;
    // 적 캐릭터의 최대 생성 개수
    public int maxEnemy = 10;
    // 게임 종류 여부를 판단할 변수
    public bool isGameOver = false;

    [Header("Object Pool")]
    // 생성할 총알 프리팹
    public GameObject bulletPrefab;
    // 오브젝트를 풀에 생성할 개수
    public int maxPool = 10;
    public List<GameObject> bulletPool = new List<GameObject>();


    public static GameManager instance = null;

    private bool isPaused = false;

    public CanvasGroup inventoryCG;

    //[HideInInspector]
    //public int killCount;

    [Header("GameData")]
    public Text killCountTxt;
    private DataManager dataManager;
    //public GameData gameData;
    public GameDataObject gameData;

    public delegate void ItemChangeDelegate();
    public static event ItemChangeDelegate OnItemChange;

    private GameObject slotList;

    public GameObject[] itemObject;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // instance에 할당된 클래스의 인스턴스가 다를 경우 새로 생서된 클래스를 의미함
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        dataManager = GetComponent<DataManager>();

        dataManager.Initialize();

        slotList = inventoryCG.transform.Find("SlotList").gameObject;

        LoadGameData();

        CreatePooling();
    }

    void LoadGameData()
    {
        //GameData data = dataManager.Load();

        //gameData.hp = data.hp;
        //gameData.damage = data.damage;
        //gameData.speed = data.speed;
        //gameData.killCount = data.killCount;
        //gameData.equipItem = data.equipItem;

        if (gameData.equipItem.Count > 0)
        {
            InventorySetup();
        }

        killCountTxt.text = "KILL " + gameData.killCount.ToString("0000");

        //killCount = PlayerPrefs.GetInt("KILL_COUNT", 0);
        //killCountTxt.text = "KILL " + killCount.ToString("0000");
    }

    void SaveGameData()
    {
        //dataManager.Save(gameData);
        UnityEditor.EditorUtility.SetDirty(gameData);
    }

    void InventorySetup()
    {
        var slots = slotList.GetComponentsInChildren<Transform>();

        for (int i = 0; i < gameData.equipItem.Count; ++i)
        {
            for (int j = 1; j < slots.Length; ++j)
            {
                if (slots[j].childCount > 0) continue;

                int itemIndex = (int)gameData.equipItem[i].itemType;

                itemObject[itemIndex].GetComponent<Transform>().SetParent(slots[j]);

                itemObject[itemIndex].GetComponent<ItemInfo>().itemData = gameData.equipItem[i];

                break;
            }
        }
    }

    public void AddItem(Item item)
    {
        if (gameData.equipItem.Contains(item)) return;

        gameData.equipItem.Add(item);

        switch (item.itemType)
        {
            case Item.ItemType.HP:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.hp += item.value;
                else
                    gameData.hp += gameData.hp * item.value;
                break;
            case Item.ItemType.DAMAGE:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.damage += item.value;
                else
                    gameData.damage += gameData.damage * item.value;
                break;
            case Item.ItemType.SPEED:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.speed += item.value;
                else
                    gameData.speed += gameData.speed * item.value;
                break;
            case Item.ItemType.GRENADE:
                break;
        }

        UnityEditor.EditorUtility.SetDirty(gameData);
        OnItemChange();
    }

    public void RemoveItem(Item item)
    {
        gameData.equipItem.Remove(item);

        switch (item.itemType)
        {
            case Item.ItemType.HP:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.hp -= item.value;
                else
                    gameData.hp = gameData.hp / (1.0f +  item.value);
                break;
            case Item.ItemType.DAMAGE:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.damage -= item.value;
                else
                    gameData.damage = gameData.hp / (1.0f + item.value);
                break;
            case Item.ItemType.SPEED:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.speed -= item.value;
                else
                    gameData.speed = gameData.hp / (1.0f + item.value);
                break;
            case Item.ItemType.GRENADE:
                break;
        }
        UnityEditor.EditorUtility.SetDirty(gameData);
        OnItemChange();
    }

    public void IncKillCount()
    {
        ++gameData.killCount;
        killCountTxt.text = "KILL " + gameData.killCount.ToString("0000");
        //++killCount;
        //killCountTxt.text = "KILL " + killCount.ToString("0000");
        //PlayerPrefs.SetInt("KILL_COUNT", killCount);
    }

    // Start is called before the first frame update
    void Start()
    {
        OnInventroyOpen(false);
        // Hierarchy 윈도우의 SpawnPointGroup 을 찾아 하위에 있는 모든 Transform 컴포넌트를 찾아옴
        points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        if (points.Length > 0)
        {
            StartCoroutine(CreateEnemy());
        }

    }

    IEnumerator CreateEnemy()
    {
        // 게임 종료 시까지 무한 루프
        while (!isGameOver)
        {
            int enemyCount = (int)GameObject.FindGameObjectsWithTag("ENEMY").Length;

            // 적 캐릭터의 최대 생성 개수보다 작을 때만 적 캐릭터 생성
            if (enemyCount < maxEnemy)
            {
                // 적 캐릭터의 생성 주기 시간만큼 대기
                yield return new WaitForSeconds(createTime);

                // 불규칙적인 위치 산출
                int index = Random.Range(1, points.Length);
                // 적 캐릭터의 동적 생성
                Instantiate(enemy, points[index].position, points[index].rotation);
            }
            else
            {
                yield return null;
            }
        }
    }

    public GameObject GetBullet()
    {
        for (int i = 0; i < bulletPool.Count; ++i)
        {
            if (bulletPool[i].activeSelf == false)
            {
                return bulletPool[i];
            }
        }
        return null;
    }

    public void CreatePooling()
    {
        // 총알을 생성해 차이들화할 페어런트 게임오브젝트를 생성
        GameObject objectPools = new GameObject("ObjectPools");

        for (int i = 0; i < maxPool; ++i)
        {
            var obj = Instantiate<GameObject>(bulletPrefab, objectPools.transform);
            obj.name = "Bullet_" + i.ToString("00");
            // 비활성화
            obj.SetActive(false);
            bulletPool.Add(obj);
        }
    }

    public void OnPauseClick()
    {
        isPaused = !isPaused;

        Time.timeScale = (isPaused) ? 0.0f : 1.0f;

        var playerObj = GameObject.FindGameObjectWithTag("PLAYER");

        var scripts = playerObj.GetComponents<MonoBehaviour>();

        foreach (var script in scripts)
        {
            script.enabled = !isPaused;
        }

        var canvasGroup = GameObject.Find("Panel-Weapon").GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = !isPaused;
    }

    public void OnInventroyOpen(bool isOpened)
    {
        inventoryCG.alpha = (isOpened) ? 1.0f : 0.0f;
        inventoryCG.interactable = isOpened;
        inventoryCG.blocksRaycasts = isOpened;
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }
}
