using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum TurnPhases {START,PLAYERTURN,ENEMYTURN,WON,LOST }
public class CombatSystem : MonoBehaviour {
    public Button attackBtn, defendBtn, storeBtn, ultBtn;
    public CameraShakeController shaker;
    public StoreController SC;
    public HudController playerHud;
    public HudController enemyHud;
    public GameObject player;
    public GameObject enemy;
    public Text storeResponseTxt;
    public int EnemyNum;
    
    Unit playerUnit;
    Unit enemyUnit;
    public bool playerDefending;
    public bool enemyDefending;
    public Text dbText;
    public Text UltText; public Text turnText;  public Text enemyHPText; public Text playerHPText;
    public int ultPts;
    public Slider UltSlider;
    public int EnemyUltPts;
    public Canvas StoreCanvas;
    public Text storePoints;
    public bool PlayerRateboost;
    public float PlayerAttackPerc;
    public int RateBoostUses = 2;
    public float standardRate = 50;
  
    public int DamageDealt;
    public bool DamageBoostActive;
    public TurnPhases state;

    // Start is called before the first frame update
    void Start()
    {
        SC.RefreshStore();
        DamageBoostActive = false;

        StoreCanvas.enabled = false;

        UltSlider.maxValue = 10;
        UltSlider.minValue = 0;
        UltSlider.value = 0;
        state = TurnPhases.START;
        StartCoroutine(SetupBattle());
      

    }
    private void Update()
    {
        UltText.text = "UP: " + ultPts + "/10";
       Mathf.Clamp(ultPts,0,10);
        UltSlider.value = Mathf.Clamp(ultPts, 0, 10) ;
        turnText.text = "" + state;
        enemyHPText.text = "HP: " + enemyUnit.currentHp + "/" + enemyUnit.maxHP ;
        playerHPText.text = "HP: " + playerUnit.currentHp + "/" + playerUnit.maxHP ;
    }

    IEnumerator SetupBattle()
    {
        playerUnit = player.GetComponent<Unit>();
        enemyUnit = enemy.GetComponent<Unit>();
        if (playerUnit.unitLevel>=PlayerPrefs.GetInt("PlayerLvl"))
        {
            PlayerPrefs.SetString("Enemy" + EnemyNum, "Defeated");
            PlayerPrefs.SetInt("MaxHP", playerUnit.maxHP + 2);
            PlayerPrefs.SetInt("Damage", playerUnit.MaxRoundDamage);
            PlayerPrefs.SetInt("UltDamage", playerUnit.UltDamage);
            PlayerPrefs.SetInt("PlayerLvl", playerUnit.unitLevel);
            PlayerPrefs.SetInt("CurrentHP", playerUnit.currentHp);

        }
        else
        {
            playerUnit.unitLevel = PlayerPrefs.GetInt("PlayerLvl");
            playerUnit.UltDamage = PlayerPrefs.GetInt("UltDamage");
            playerUnit.maxHP = PlayerPrefs.GetInt("MaxHP");
            playerUnit.MaxRoundDamage = PlayerPrefs.GetInt("Damage");
            playerUnit.currentHp = PlayerPrefs.GetInt("CurrentHP");

        }
       
        dbText.text = " A wild " + enemyUnit.name + " approaches!";
        playerHud.setHud(playerUnit);
        enemyHud.setHud(enemyUnit);
        yield return new WaitForSeconds(2f);
        state = TurnPhases.PLAYERTURN;
        PlayerTurn();
    }
    void PlayerTurn()
    {
        dbText.text = "Choose an Action: ";
    }
    public void OnStoreBtn()
    {
        if (state != TurnPhases.PLAYERTURN)
        {
            return;
        }
        else
        {
            storeResponseTxt.enabled = false;
            storePoints.text = "UP: " + ultPts;
            StoreCanvas.enabled = true;


        }


    }
    public void onHealBtn()
    {
        if (ultPts >= SC.HealCost) 
        {
            StartCoroutine(HealthPotion());

        }else
        {
            storeResponseTxt.enabled = true;
            storeResponseTxt.text = "NOT ENOUGH UP!!!";
            return; 
        }
        

    }
    IEnumerator HealthPotion() 
    {
        ultPts -= SC.HealCost;
        SC.increaseHealCost();
        SC.RefreshStore();
       
        dbText.text = "Consuming Healing Potion... +5HP!";
        yield return new WaitForSeconds(3f);
        dbText.text = "Choose an Action.";
        playerUnit.currentHp += 5;
        playerHud.setHud(playerUnit);
        storePoints.text = "UP: " + ultPts;
        // state = TurnPhases.ENEMYTURN;
        //   StartCoroutine(EnemyTurn());


    }
    public void onRateBoostClick()
    {
        if (ultPts >= SC.HRCost)
        {
            ultPts -= SC.HRCost;
            SC.increaseHRCost(); SC.RefreshStore();
            RateBoostUses = 2;
            PlayerAttackPerc += 20;
            
            storeResponseTxt.text = "Purchased Hit Rate Boost. The next 2 attacks have a +20% hit chance. ";
            storeResponseTxt.enabled = true;
            playerHud.setHud(playerUnit);
            storePoints.text = "UP: " + ultPts;
        }
        else
        {
            storeResponseTxt.text = "Not enough UP!";
            return;
        }

    }
    public void onDamageBoostClick()
    {
        if (ultPts >= SC.DBCost)
        {
            ultPts -= SC.DBCost;
            SC.increaseDBCost(); SC.RefreshStore();
            DamageBoostActive = true;
            PlayerAttackPerc += 20;
            storeResponseTxt.text = "Damage boost now active! 1.5x damage! ";
            storeResponseTxt.enabled = true;
            playerHud.setHud(playerUnit);
            storePoints.text = "UP: " + ultPts;
        }
        else
        {
            storeResponseTxt.text = "Not enough UP!";
            return;
        }

    }

    public void ExitBtn() 
    {
        Application.Quit();
    }
    public void onBackBtn() 
    {
        StoreCanvas.enabled = false;
    }
    public void onAttackBtn() 
    {
        if (state!=TurnPhases.PLAYERTURN) 
        {
            return;        
        }
        StartCoroutine(PlayerAttack2());
    }
    public void onDefend() 
    {
        if (state != TurnPhases.PLAYERTURN)
        {
            return;
        } 
        StartCoroutine(PlayerDefend());


    }
    public void onUltimate()
    {
        if (state != TurnPhases.PLAYERTURN)
        {
            return;
        }
        if (ultPts == 10)
        {
            StartCoroutine(PlayerUlt());
        }
        else { return; }
       


    }
    IEnumerator PlayerUlt() 
    {
        bool isDead = enemyUnit.TakeDamage(playerUnit.UltDamage);
        dbText.text = "Ultimate unleashed! Enemy cannot defend!";
        StartCoroutine(shaker.Shake(.5f, 2f));
        ultPts = 0;
        enemyHud.setHP(enemyUnit.currentHp);
        yield return new WaitForSeconds(2f);
        if (isDead)
        {
            state = TurnPhases.WON;
            EndBattle();

        }
        else
        {
            state = TurnPhases.ENEMYTURN;
            

            ultPts = 0;
            dbText.text = "Ending Player turn. Beginnning enemy turn.";
            yield return new WaitForSeconds(2);
            StartCoroutine(EnemyTurn());
        }


    }
    IEnumerator PlayerDefend() 
    {
        playerDefending = true;
        dbText.text = "Entered defense stance.Next turn only damage is halved.";

        yield return new WaitForSeconds(3f);
        state = TurnPhases.ENEMYTURN;
        dbText.text = "Ending Player turn. Beginnning enemy turn.";
        yield return new WaitForSeconds(3);
        StartCoroutine(EnemyTurn());

    }

    IEnumerator PlayerAttack()
    {
        if (playerDefending==true) { playerDefending = false; }
        if (Random.Range(1, 6) >= 3)
        {
            if (enemyDefending == true)
            {
                StartCoroutine(shaker.Shake(.15f, 1f));
                bool isDead = enemyUnit.TakeDefendedDamage(playerUnit.MaxRoundDamage);
                dbText.text = "Enemy defends. Attack partially successful.";
                EnemyUltPts++;
                ultPts++;
                Mathf.Clamp(ultPts, 0, 10);
                enemyHud.setHP(enemyUnit.currentHp);
                yield return new WaitForSeconds(1f);
                if (isDead)
                {
                    state = TurnPhases.WON;
                    EndBattle();

                }
                else
                {
                    state = TurnPhases.ENEMYTURN;
                    dbText.text = "Ending Player turn. Beginnning enemy turn.";
                    yield return new WaitForSeconds(2);
                    StartCoroutine(EnemyTurn());
                }
            } 
            else
            {
                StartCoroutine(shaker.Shake(.3f,1f));
                bool isDead = enemyUnit.TakeDamage(playerUnit.MaxRoundDamage);
                enemyHud.setHP(enemyUnit.currentHp);
                ultPts += 2;
                dbText.text = "Attack is succesful!";
                Mathf.Clamp(ultPts, 0, 10);


                enemyUnit.TakeDamage(playerUnit.MaxRoundDamage);
                yield return new WaitForSeconds(2f);
                if (isDead)
                {
                    state = TurnPhases.WON;
                    EndBattle();
                }
                else
                {
                    state = TurnPhases.ENEMYTURN;
                    dbText.text = "Ending Player turn. Beginnning enemy turn.";
                    yield return new WaitForSeconds(2);
                    StartCoroutine(EnemyTurn());
                }

            }
          
        }
        else
        {
            dbText.text = "Attack misses";
            yield return new WaitForSeconds(2f);
                state = TurnPhases.ENEMYTURN;
            dbText.text = "Ending Player turn. Beginnning enemy turn.";
            yield return new WaitForSeconds(2);
            StartCoroutine(EnemyTurn());
        }

       
       
    }
    IEnumerator PlayerAttack2()
    {
        bool isDead = false;
        yield return new WaitForSeconds(1);
        if (RateBoostUses == 0)
        {
            PlayerAttackPerc = 0;
        }

        int rollVal = Random.Range(0, 100);
        if ((rollVal + PlayerAttackPerc) >= 40) //attack hits
        {
            DamageDealt = playerUnit.MaxRoundDamage;
            if (DamageBoostActive == true)
            {
                DamageDealt = playerUnit.MaxRoundDamage * (int)1.5f;
            }

            //  DamageDealt = (int)3*((playerUnit.unitLevel *2* (Mathf.Clamp(playerUnit.currentHp, playerUnit.MaxRoundDamage,100) / playerUnit.MaxRoundDamage))/(playerUnit.MaxRoundDamage-DamageBoostAmmount)); Damage Formula drafting, requires more time to test. Sticking with simple calc. issues with floats too broad to change now.
            if (enemyUnit.Defending == true)
            {
                DamageDealt = DamageDealt / 2;
                isDead = enemyUnit.TakeDamage((int)(DamageDealt));

                dbText.text = "Enemy defends. Attack partially successful.";
                EnemyUltPts += 2;
                ultPts += 3;
                Mathf.Clamp(ultPts, 0, 10);
                enemyHud.setHP(enemyUnit.currentHp);
                yield return new WaitForSeconds(3f);

            }
            else
            {


                isDead = enemyUnit.TakeDamage((int)(DamageDealt));
                enemyHud.setHP(enemyUnit.currentHp);
                ultPts += 3;
                dbText.text = "Attack is succesful!";
                Mathf.Clamp(ultPts, 0, 10);



                yield return new WaitForSeconds(3f);
            }

            if (isDead)
            {
                state = TurnPhases.WON;
                EndBattle();

            }
            else
            {
                state = TurnPhases.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }

        }


        //attack fails
        else
        {
            dbText.text = "Attack misses";
            yield return new WaitForSeconds(2f);
            state = TurnPhases.ENEMYTURN;
            StartCoroutine(EnemyTurn());

        }
        RateBoostUses--;
        Mathf.Clamp(RateBoostUses, 0, 2);
    }
    IEnumerator EnemyTurn()
    {
        attackBtn.interactable = false; 
        defendBtn.interactable = false; 
        storeBtn.interactable = false; 
        ultBtn.interactable = false;

        if (enemyDefending == true) 
        {
            enemyDefending = false;
        
        }
        int RandomAct = Random.Range(1, 6);
        if (RandomAct <= 3)
        {
            dbText.text = enemyUnit.name + " attacks!";
            yield return new WaitForSeconds(3f);
            if (Random.Range(1, 6) > 2)
            {

                if (playerDefending == true)
                {
                    StartCoroutine(shaker.Shake(.15f, 1f));
                    bool isDead = playerUnit.TakeDefendedDamage(enemyUnit.MaxRoundDamage);
                    playerDefending = false;
                    EnemyUltPts++;
                    ultPts+=2;
                    Mathf.Clamp(EnemyUltPts, 0, 10);
                    dbText.text = "Player defends. Attack partially successful.";
                    playerHud.setHP(playerUnit.currentHp);
                    yield return new WaitForSeconds(4f);
                    if (isDead)
                    {
                        state = TurnPhases.LOST;
                        EndBattle();

                    }
                    else
                    {
                        state = TurnPhases.PLAYERTURN;
                        dbText.text = "Ending Enemy turn. Beginnning Player turn.";
                        yield return new WaitForSeconds(4);
                        PlayerTurn();
                    }
                }
                else
                {
                    StartCoroutine(shaker.Shake(.3f, 1f));
                    bool isDead = playerUnit.TakeDamage(enemyUnit.MaxRoundDamage);
                    dbText.text = "Attack successful!";
                    EnemyUltPts += 3;
                    Mathf.Clamp(EnemyUltPts, 0, 10);

                    playerHud.setHP(playerUnit.currentHp);
                    yield return new WaitForSeconds(4f);
                    if (isDead)
                    {
                        state = TurnPhases.LOST;
                        EndBattle();

                    }
                    else
                    {
                        state = TurnPhases.PLAYERTURN;
                        dbText.text = "Ending Enemy turn. Beginnning Player turn.";
                        yield return new WaitForSeconds(4);
                        PlayerTurn();
                    }
                }

            }
            else
            {

                dbText.text = "Attack has missed!";
                yield return new WaitForSeconds(3f);
                state = TurnPhases.PLAYERTURN;
                dbText.text = "Ending Enemy turn. Beginnning Player turn.";
                yield return new WaitForSeconds(2);
                PlayerTurn();


            }

        }
        
        else if ((RandomAct == 6 && EnemyUltPts == 10)|| (RandomAct == 5 && EnemyUltPts == 10))
        {
            StartCoroutine(shaker.Shake(.5f, 2f));
            EnemyUltPts = 0;
            bool isDead = playerUnit.TakeDamage(enemyUnit.UltDamage);
            dbText.text = "Enemy unleashes Ultimate! You cannot defend.";
           
            Mathf.Clamp(EnemyUltPts, 0, 10);

            playerHud.setHP(playerUnit.currentHp);
            yield return new WaitForSeconds(4f);
            if (isDead)
            {
                state = TurnPhases.LOST;
                EndBattle();

            }
            else
            {
                dbText.text = "Enemy smoked some shizz and is tripping crazy. No action made.";
                    yield return new WaitForSeconds(5);
                state = TurnPhases.PLAYERTURN;
                dbText.text = "Ending Enemy turn. Beginnning Player turn.";
                yield return new WaitForSeconds(4);
                PlayerTurn();
            }
        }
        else if (RandomAct == 4 || RandomAct == 5)
        {
            enemyDefending = true;
            dbText.text = "Entered defense stance.Next turn only damage dealt is halved.";

            yield return new WaitForSeconds(5f);
            state = TurnPhases.PLAYERTURN;
            dbText.text = "Ending Enemy turn. Beginnning Player turn.";
            yield return new WaitForSeconds(4);
            PlayerTurn();


        }
        else { }
        attackBtn.interactable = true;
        defendBtn.interactable = true;
        storeBtn.interactable = true;
        ultBtn.interactable = true ;

    }
     public void EndBattle()
    {
        if (state == TurnPhases.WON)
        {
            PlayerPrefs.SetString("Enemy"+EnemyNum,"Defeated");
            PlayerPrefs.SetInt("MaxHP",PlayerPrefs.GetInt("MaxHP")+2);
            PlayerPrefs.SetInt("Damage", PlayerPrefs.GetInt("Damage") + 1);
            PlayerPrefs.SetInt("UltDamage", PlayerPrefs.GetInt("UltDamage")+1);
            PlayerPrefs.SetInt("PlayerLvl", PlayerPrefs.GetInt("PlayerLvl") + 1);
            PlayerPrefs.SetInt("CurrentHP",playerUnit.currentHp);
            dbText.text = enemyUnit.name + " has fallen in battle! You reign victorious!";

            StartCoroutine(scenewait());
            SceneManager.LoadScene(0);
        } else if (state==TurnPhases.LOST) 
        {
            dbText.text = "You have Fallen in battle.";
        }
    
    
    }
    IEnumerator scenewait() 
    {

        yield return new WaitForSeconds(3f);
    }
}
    // Update is called once per frame
   
