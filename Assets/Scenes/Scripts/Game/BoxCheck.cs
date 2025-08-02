using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCheck : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Point"))
        {
            Debug.Log("Hello there");
            spriteRenderer.color = Color.green;
        }

    }
    
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Point"))
        {
            spriteRenderer.color = Color.red;
        }

    }


}