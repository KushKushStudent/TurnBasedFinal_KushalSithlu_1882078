using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{
    public float speed=10f;
    Vector2 movement;
    public Text HPtext;
    public Rigidbody2D rb;
    public GameObject Enemy1Door, Enemy2Door, Enemy3Door;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("MaxHP") == -1 || PlayerPrefs.GetInt("MaxHP") == 0)
        { PlayerPrefs.SetInt("PlayerLvl",2);
            PlayerPrefs.SetInt("MaxHP", 30);
            PlayerPrefs.SetInt("CurrentHP",30);
            HPtext.text = "HP:30/30";


        }
        else 
        {
            
            HPtext.text = "HP: "+PlayerPrefs.GetInt("CurrentHP")+"/" + PlayerPrefs.GetInt("MaxHP");
        }
        if (PlayerPrefs.GetString("Enemy1") == "Defeated")
        {
            Enemy1Door.SetActive(false);
            
        }

        if (PlayerPrefs.GetString("Enemy2") == "Defeated")
        {
            Enemy2Door.SetActive(false);

        }

        if (PlayerPrefs.GetString("Enemy3") == "Defeated")
        {
            Enemy3Door.SetActive(false);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) 
        {
            Application.Quit();
        }
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
        if (movement.x < 0)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);


        }
        else if(movement.x>0) 
        { 
            transform.eulerAngles = new Vector3(0, 0, 0); 
        }
       else if (movement.y>0 && movement.x < 0) 
        
        {
            transform.eulerAngles = new Vector3(0, -180, 90);

        }else if (movement.y>0 && movement.x > 0) 
        
        {
            transform.eulerAngles = new Vector3(0, 0, 90);

        }else if (movement.y < 0 && movement.x < 0)

        {
            transform.eulerAngles = new Vector3(0, -180, -90);

        }
       else if (movement.y < 0 && movement.x > 0)

        {
            transform.eulerAngles = new Vector3(0, 0, -90);

        }


    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name=="BattleStart1") 
        {
            SceneManager.LoadScene(1);
        }
        if (collision.tag == "HealBottle")
        {
            PlayerPrefs.SetInt("CurrentHP",PlayerPrefs.GetInt("CurrentHP")+8);
            PlayerPrefs.SetInt("CurrentHP",Mathf.Clamp(PlayerPrefs.GetInt("CurrentHP"),0,PlayerPrefs.GetInt("MaxHP")));
            HPtext.text = "HP: " + PlayerPrefs.GetInt("CurrentHP") + "/" + PlayerPrefs.GetInt("MaxHP");
            collision.gameObject.SetActive(false);
        }
        if (collision.name == "BattleStart2")
        {
            SceneManager.LoadScene(2);
        }
        if (collision.name == "BattleStart3")
        {
            SceneManager.LoadScene(3);
        }
    }
    
}
