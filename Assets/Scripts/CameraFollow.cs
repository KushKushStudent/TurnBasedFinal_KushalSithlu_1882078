using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;public GameObject cam;
    private void Update()
    {

       
    }
    private void LateUpdate()
    {
        Follow();
       
    }
    void Follow() 
    {
        Vector3 targetPos = player.position + offset;
        Vector3 smoothPosition=Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.fixedDeltaTime);
       targetPos.z = -10;
        transform.position = targetPos;
       
    }

    // Update is called once per frame
   
}
