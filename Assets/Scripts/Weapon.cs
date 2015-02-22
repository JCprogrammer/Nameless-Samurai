using UnityEngine;
using System.Collections;


public class Weapon : MonoBehaviour {

    public WeaponType type;
    public string name;
    public float damageImpact;
    public WeaponEffect effect;

    public GameObject weaponObject;
    
    SGAnimation weaponAnimation;
    Collider2D weaponCollider;
    bool isUsing;

    public bool Using
    {
        get { return isUsing; }
    }

    public void Use()
    {
        isUsing = true;       
    }

    public void Dismiss()
    {
        isUsing = false;
    }
}



public enum WeaponType
{
    OneHandSword,
    TwoHandSword,
    Dagger,
    Shuriken,
    Polearm,
    Helbert,
    Axe,
    Mace
}

public enum WeaponEffect
{
    Lightning,
    Fire,
    Frost,
    Shadow,
    None,
}