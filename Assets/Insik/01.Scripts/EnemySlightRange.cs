using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemySlightRange : MonoBehaviour
{
 public Transform _player;
   
 private Transform _Enemy;

 private EnemyInsik EnemyInsik;
 
 private float _moveSpeed = 3f;

    private void Awake()
    {
        EnemyInsik = GetComponent<EnemyInsik>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            EnemyInsik.Chase();
        }
    }

}

