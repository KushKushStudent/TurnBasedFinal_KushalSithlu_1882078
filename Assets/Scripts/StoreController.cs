using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreController : MonoBehaviour
{
    public Text HealCostText;
    public Text HitRateCostText;
    public Text DamageBoostCostText;
    public int HealCost=4;
    public int DBCost=3;
        public int HRCost=2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RefreshStore() 
    {
        HealCostText.text = HealCost + " UP";
        HitRateCostText.text = HRCost + " UP";
        DamageBoostCostText.text = DBCost + " UP";
    
    }
    public void increaseHealCost() 
    {
        HealCost++;
        Mathf.Clamp(HealCost, 0, 10);
    }  
    public void increaseHRCost() 
    {
     
            HRCost++;
        Mathf.Clamp(HRCost, 0, 10);
    }  
    public void increaseDBCost() 
    {
        DBCost++;
        Mathf.Clamp(DBCost, 0, 10);
    }
}
