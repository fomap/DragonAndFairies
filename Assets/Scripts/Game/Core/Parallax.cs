using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float startPosX, startPosY;
    private float length, height;
    public GameObject cam;
    public float parallaxEffectX = 0.5f;
    public float parallaxEffectY = 0.5f;

    void Start()
    {
        startPosX = transform.position.x;
        startPosY = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        height = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void Update()
    {

        float tempX = cam.transform.position.x * (1 - parallaxEffectX);
        float tempY = cam.transform.position.y * (1 - parallaxEffectY);
        float distanceX = cam.transform.position.x * parallaxEffectX;
        float distanceY = cam.transform.position.y * parallaxEffectY;

            // cam.transform.position.y - startPosY) * parallaxEffect * verticalParallaxRatio;


        transform.position = new Vector3(
            startPosX + distanceX,
            startPosY + distanceY,
            transform.position.z
        );


        if (tempX > startPosX + length)
        {
            startPosX += length;
        }
        else if (tempX < startPosX - length)
        {
            startPosX -= length;
        }
        
        if (tempY > startPosY + height) {
            startPosY += height;
        }
        else if (tempY < startPosY - height) {
            startPosY -= height;
        }
    }
}