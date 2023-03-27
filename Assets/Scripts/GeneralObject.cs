using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralObject : MonoBehaviour
{
    public bool isActive { get { return gameObject.activeInHierarchy; } }
    public void EnableObject()
    {
        gameObject.SetActive(true);
    }
    public void DisableObject()
    {
        gameObject.SetActive(false);
    }
    public void RemoveObject()
    {
        Destroy(gameObject);
    }


}
