using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEntityData", menuName = "Data/Entity Data/Base Data")]
public class D_Entity : ScriptableObject
{
    public float maxHealth = 30;

    public float damageHopSpeed = 10f;

    public float wallCheckDistance = 0.2f;
    public float ledgeCheckDistance = 0.4f;
    public float groundCheckradius = 0.3f;

    public float minAgroDistance = 3f;
    public float maxAgroDistance = 4f;

    public float stunResistance = 3f;
    public float stunrecoveryTime = 2f;

    public GameObject hitParticle;

    public float closeRangeActionDistance = 1f;

    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;
}
