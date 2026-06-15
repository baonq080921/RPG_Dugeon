using System.Collections;
using System.Collections.Generic;
using player;
using UnityEngine;
using DG;
using DG.Tweening;

public class DomainExpasionSkill : SkillBase
{
    [SerializeField] private DomainExpasionSkillObject _domainExpasionAera;
    private float _elementalDamge;
    private float _physicalDamage;
    [SerializeField] private float scaleFactor;
    public override void ExecuteSkillEffect()
    {
        _elementalDamge = player.entityStat.GetElementalDamageValue(out ElementType elementType);
        _physicalDamage = player.entityStat.GetPhysicalDamageValue(out bool isCrit);
        base.ExecuteSkillEffect();
        DomainExpasionSkillObject skill = Instantiate(_domainExpasionAera,transform.position,Quaternion.identity);
        skill.OpenDomain(SkillBaseDefinition.Duration);
        skill.SetUpDamageForDomain(SkillBaseDefinition.Damage,_physicalDamage,_elementalDamge, scaleFactor);

    }
}
