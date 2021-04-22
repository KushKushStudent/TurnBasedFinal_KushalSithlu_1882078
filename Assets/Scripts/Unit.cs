using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;
    public bool Defending;

    public int MaxRoundDamage;
    public int UltDamage;
    public int maxHP;
    public int currentHp;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public bool TakeDamage(int dmg)
    {
        currentHp -= dmg;
        if (currentHp <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool TakeDefendedDamage(int dmg)
    {
        currentHp -= dmg / 2;
        if (currentHp <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}