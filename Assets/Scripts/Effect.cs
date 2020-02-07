using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    [SerializeField]
    private AnimationClip attackEffectAnim;

    public AnimationClip AttackEffectAnim { get => attackEffectAnim; private set => attackEffectAnim = value; }
    
}
