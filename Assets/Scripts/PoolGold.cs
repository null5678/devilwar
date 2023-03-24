using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolGold : MonoBehaviour
{
    [SerializeField]
    private Gold _gold;
    [SerializeField]
    private Gold _navi;

    private List<Gold> _listGold = new List<Gold>();
    private Vector3 _defaultScale = Vector3.zero;

    private Gold _naviObj;

    private void Awake()
    {
        _defaultScale = _gold.transform.localScale;
    }

    public void GenerateGold(Vector3 pos)
    {
        if(_listGold.Count != 0)
        {
            foreach (var v in _listGold)
            {
                if (!v.isActive)
                {
                    v.SetScale(_defaultScale);
                    v.Value = 1;
                    v.Drop(pos);
                    return;
                }
            }
        }

        InstantiateGold(pos, _defaultScale);
    }

    public void GenerateGoldBoss(Vector3 pos)
    {
        int num = 5;
        float rad = 1f;
        float angleDiff = 360f / num;

        for(int i = 0; i < num; i++)
        {
            Vector3 p = Vector3.zero;
            float angle = (90 - angleDiff * i) * Mathf.Deg2Rad;
            p.x = pos.x + (rad * Mathf.Cos(angle));
            p.y = pos.y + (rad * Mathf.Sin(angle));

            InstantiateGold(p, new Vector3(_defaultScale.x * 1.5f, _defaultScale.y * 1.5f,1), 100);
        }
    }

    public void GenerateNavi(Vector3 pos)
    {
        _naviObj = Instantiate(_navi, pos, Quaternion.identity, transform);
    }

    public void AllPickup()
    {
        foreach(var v in _listGold)
        {
            v.Pickup();
        }

        _naviObj?.Pickup();
    }

    private void InstantiateGold(Vector3 pos, Vector3 scale, int gold_value = 1)
    {
        var gold = Instantiate(_gold, pos, Quaternion.identity, transform);
        gold.GetComponent<Gold>().Value = gold_value;
        gold.GetComponent<Gold>().SetScale(scale);
        _listGold.Add(gold);
    }
}
