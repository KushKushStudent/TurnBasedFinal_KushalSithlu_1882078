using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    public Text nameText;
    public Text levelText;
    public Slider HP;
    public void setHud(Unit unit) 
    {
        HP.minValue = 0;
        nameText.text = unit.unitName;
        levelText.text = "Lvl"+unit.unitLevel;
        HP.maxValue = unit.maxHP;
        HP.value = unit.currentHp;
    
    
    }
    public void setHP(int hp) 
    {
        HP.value = hp;
    }

    // Start is called before the first frame update
}
