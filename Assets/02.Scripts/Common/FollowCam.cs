using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target;    //추적할 대상
    public float moveDamping = 15.0f; //이동 속도 계수
    public float rotateDamping = 10.0f; //회전 속도 계수
    public float distance = 5.0f; //추적 대상과의 거리
    public float height = 4.0f; //추적 대상과의 높이
    public float targetOffset = 2.0f; //추적 좌표의 오프셋

    [Header("Wall Obstacle Setting")]
    public float heightAboveWall = 7.0f;
    public float colliderRadius = 1.8f;
    public float overDamping = 5.0f;
    private float originHeight;

    [Header("Etc Obstacle Setting")]
    public float heightAboveObstacle = 12.0f;

    public float castOffset = 1.0f;

    //CameraRig 의 Transform 컴포넌트
    private Transform tr;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        originHeight = height;
    }

    private void LateUpdate()
    {
        var camPos = target.position - (target.forward * distance)
                        + (target.up * height);

        //이동할 때의 속도 계수를 적용
        tr.position = Vector3.Slerp(tr.position, camPos, Time.deltaTime * moveDamping);

        //회전할 때의 속도 계수를 적용
        tr.rotation = Quaternion.Slerp(tr.rotation, target.rotation,
                                Time.deltaTime * rotateDamping);

        tr.LookAt(target.position + (target.up * targetOffset));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //추적 및 시야를 맞출 위치를 표시
        Gizmos.DrawSphere(target.position + (target.up * targetOffset), 0.1f);

        //메인 카메라와 추적 지점 간의 선율 표시
        Gizmos.DrawLine(target.position + (target.up * targetOffset),
                            transform.position);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, colliderRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(target.position + (target.up * castOffset), transform.position);


    }


    // Update is called once per frame
    void Update()
    {
        if (Physics.CheckSphere(tr.position, colliderRadius))
        {
            height = Mathf.Lerp(height, heightAboveWall, Time.deltaTime * overDamping);
        }
        else
        {
            height = Mathf.Lerp(height, originHeight, Time.deltaTime * overDamping);
        }

        Vector3 castTarget = target.position + (target.up * castOffset);
        Vector3 castDir = (castTarget - tr.position).normalized;

        if (Physics.Raycast(tr.position, castDir, out RaycastHit hit, Mathf.Infinity))
        {
            if (!hit.collider.CompareTag("PLAYER"))
            {
                height = Mathf.Lerp(height, heightAboveObstacle, Time.deltaTime * overDamping);
            }
            else
            {
                height = Mathf.Lerp(height, originHeight, Time.deltaTime * overDamping);
            }
        }
    }
}
