using UnityEngine;

  

public class SafeZone : MonoBehaviour

{

    private void OnTriggerStay2D(Collider2D collider)

    {

    if (collider.gameObject.CompareTag("Player"))

    {

        GameManager.instance.isSafeZone = true;

        Debug.Log("안전지대 안입니다.");

    }

    }

    private void OnTriggerExit2D(Collider2D collider)

    {

    if (collider.gameObject.CompareTag("Player"))

    {

        GameManager.instance.isSafeZone = false;

        Debug.Log("안전지대 밖입니다.");

    }

    }

	}