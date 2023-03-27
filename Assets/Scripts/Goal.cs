using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Goal : GeneralObject
{
    private readonly float DISTANCE = 20f;

    public bool isGoal = false;

    public void Init(Vector3 pos_player)
    {
        Vector3 pos = Vector3.zero;
        float angle = Random.Range(0f, 360f);
        pos.x = DISTANCE * Mathf.Cos(angle) + pos_player.x;
        pos.y = DISTANCE * Mathf.Sin(angle) + pos_player.y;

        transform.position = pos;

        EnableObject();
    }
}
