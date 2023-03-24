using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour
{
    public int Value { get; set; } = 1;

    public bool isActive { get { return gameObject.activeInHierarchy; } }

    public void SetScale(Vector3 scale)
    {
        transform.localScale = scale;
    }

    public void Drop(Vector3 pos)
    {
        gameObject.SetActive(true);
        transform.position = pos;
    }

    public void Pickup()
    {
        gameObject.SetActive(false);
    }
}
