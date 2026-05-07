using UnityEngine;
using UnityEngine.InputSystem;
public class StoreTrigger : MonoBehaviour
{
    Rigidbody2D _rb;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }





    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Store"))
        {

        }
    }
    private void OnSelectionAndEscape (InputValue value)
    {

    }


}
