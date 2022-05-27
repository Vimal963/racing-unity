using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRadious : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacles"))
        {
            Debug.Log("SHOW OBSTICASLES");
            collision.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacles"))
        {
            Debug.Log("OFF OBSTICASLES");
            collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }

    }
}
