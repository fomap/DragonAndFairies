using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCheck : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Point"))
        {
            FeyNewControl.currentBoxes++;
            Debug.Log(FeyNewControl.currentBoxes);
        }

    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Point"))
        {

            FeyNewControl.currentBoxes--;
           
        }

    }


}


