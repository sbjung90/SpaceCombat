using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHpBar : MonoBehaviour
{
    // Canvas 를 렌더링하는 카메라
    private Camera uiCamera;
    // UI 용 최상위 캔버스
    private Canvas canvas;
    // 부모 RectTransform 컴포넌트
    private RectTransform rectParent;
    // 자신 RectTransform 컴포넌트
    private RectTransform rectHp;

    //hpbar 이미지의 위치를 조절할 오프셋
    [HideInInspector] public Vector3 offset = Vector3.zero;
    // 추적할 대상의 Transform 컴포넌트
    [HideInInspector] public Transform targetTr;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        uiCamera = canvas.worldCamera;
        rectParent = canvas.GetComponent<RectTransform>();
        rectHp = gameObject.GetComponent<RectTransform>(); 
    }

    private void LateUpdate()
    {
        // 월드 좌표를 스크린의 좌표로 변환
        var ScreenPos = Camera.main.WorldToScreenPoint(targetTr.position + offset);
        // 카메라의 뒷쪽 영역(180 도 회전) 일 때 좌표값 보정
        if (ScreenPos.z < 0.0f)
        {
            ScreenPos *= -1.0f;
        }
        // RectTransform 조표값을 전달받을 변수
        var localPos = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, ScreenPos, uiCamera, out localPos);
        // 생명 게이지 이미지의 위치를 변경
        rectHp.localPosition = localPos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
