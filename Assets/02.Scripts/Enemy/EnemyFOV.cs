using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    public float viewRange = 15.0f;
    [Range(0, 360)]
    public float viewAngle = 120.0f;

    private Transform enemyTr;
    private Transform playerTr;
    private int playerLayer;
    private int obstacleLayer;
    private int layerMask;

    private void Awake()
    {
        enemyTr = GetComponent<Transform>();
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").transform;

        playerLayer = LayerMask.NameToLayer("PLAYER");
        obstacleLayer = LayerMask.NameToLayer("OBSTACLE");

        layerMask = (1 << playerLayer) | (1 << obstacleLayer);
    }

    public Vector3 CiclePoint(float angle)
    {
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public bool IsTracePlayer()
    {
        if (enemyTr == null)
            return false;

        bool isTrace = false;

        Collider[] colls = Physics.OverlapSphere(enemyTr.position, viewRange, 1 << playerLayer);

        if (colls.Length == 1)
        {
            Vector3 dir = (playerTr.position - enemyTr.position).normalized;

            if (Vector3.Angle(enemyTr.forward, dir) < viewAngle * 0.5f)
            {
                isTrace = true;
            }
        }
        return isTrace;
    }

    public bool IsViewPlayer()
    {
        if (enemyTr == null)
            return false;

        bool isView = false;

        RaycastHit hit;

        Vector3 dir = (playerTr.position - enemyTr.position).normalized;

        if (Physics.Raycast(enemyTr.position, dir, out hit, viewRange, layerMask))
        {
            isView = (hit.collider.CompareTag("PLAYER"));
        }

        return isView;
    }
}
