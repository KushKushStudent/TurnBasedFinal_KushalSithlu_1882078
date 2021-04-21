using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float distance;
    public float speed = 1;
    private bool rightMove = true;

    public Transform groundDetection;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distance);
            if (groundInfo.collider==false)
        {
            if (rightMove == true)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                rightMove = false;
            
            }
            else 
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                rightMove = true;
             
            }
             
        }   
            

    }
    public IEnumerator endwait() 
    {
        yield return new WaitForSeconds(2f);
    }


  
}
