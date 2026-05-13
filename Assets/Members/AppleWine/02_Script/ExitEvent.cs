using System;
using Unity.VisualScripting;
using UnityEngine;

public class ExitEvent : MonoBehaviour
{
    [SerializeField]private GameObject next; 
    private void Awake()
    {
       next.SetActive(false);
    }
    private void Update()
    {
        PassByMonster();
        PassByKey();

    }

    private void PassByKey()
    {
        throw new NotImplementedException();
    }

    private void PassByMonster()
    {
        throw new NotImplementedException();
    }
}
