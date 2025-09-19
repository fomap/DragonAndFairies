using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ZoomOut : MonoBehaviour
{


    
    public CinemachineVirtualCamera  camera;
    public int targetZoom = 20;
    private float zoomSpeed = 5f;
    private void OnEnable()
    {
        FairyMovement.CameraZoomOut += ZoomOutCinema;   
    }
    private void OnDisable()
    {
        FairyMovement.CameraZoomOut -= ZoomOutCinema; 
    }
    // Start is called before the first frame update
    void Awake()
    {
         camera.m_Lens.OrthographicSize = 3.5f;
    }

    private void ZoomOutCinema()
    {

        camera.m_Lens.OrthographicSize = Mathf.Lerp(camera.m_Lens.OrthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
    }
}
