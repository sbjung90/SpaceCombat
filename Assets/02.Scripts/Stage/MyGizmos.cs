﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGizmos : MonoBehaviour
{
    public enum Type { NORMAL, WAYPOINT }
    private const string wayPointFile = "Enemy";
	public Type type = Type.NORMAL;

    public Color color = Color.yellow;
    public float radius = 0.1f;

    private void OnDrawGizmos()
    {
        if (type == Type.WAYPOINT)
        {
            Gizmos.DrawIcon(transform.position + Vector3.up * 1.0f,
                wayPointFile, true);

        }
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
