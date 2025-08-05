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
          //  spriteRenderer.color = Color.green;
            PlayerMovement.currentBoxes++;
        }

    }
    
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Point"))
        {
           // spriteRenderer.color = Color.red;
            PlayerMovement.currentBoxes++;

        }

    }


}