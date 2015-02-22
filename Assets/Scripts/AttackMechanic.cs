using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AttackMechanic : MonoBehaviour {
    
    //public float damageImpact;
    public Weapon weapon;
    [HideInInspector]
    public bool againstEveryObject;
    [HideInInspector]
    public List<string> targetTags;
}

