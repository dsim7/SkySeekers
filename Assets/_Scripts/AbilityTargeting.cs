﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AbilityTargeting : ScriptableObject
{
    [SerializeField]
    int requiredTargets;
    [SerializeField]
    protected bool canSelectAllies, canSelectEnemies, canSelectMelee, canSelectSupport,
        cannotSelectSelf, cannotSelectLastMelee, cannotRepeatSelection;

    public virtual bool IsAutoTarget => true;
    public int NumTargetsRequired => requiredTargets;
    public bool CanSelectAllies => canSelectAllies;
    public bool CanSelectEnemies => canSelectEnemies;
    public bool CanSelectMelee => canSelectMelee;
    public bool CanSelectSupport => canSelectSupport;
    public bool CannotSelectSelf => cannotSelectSelf;
    public bool CannotSelectLastMelee => cannotSelectLastMelee;
    public bool CannotRepeatSelection => cannotRepeatSelection;

    public virtual void AutoTarget(AbilityInstance abilityInstance) { }

    public virtual bool SelectTargetCheck(Character selected, AbilityInstance abilityInstance)
    {
        Character caster = abilityInstance.Caster;
        return (canSelectAllies || selected.Team != caster.Team) &&
            (canSelectEnemies || selected.Team == caster.Team) &&
            (canSelectMelee || selected.PositionHandler.Position != TeamPosition.Melee) &&
            (canSelectSupport || selected.PositionHandler.Position != TeamPosition.Support) &&
            (!cannotSelectSelf || selected == caster) &&
            (!cannotSelectLastMelee || selected.PositionHandler.CanReposition) &&
            (!cannotRepeatSelection || !abilityInstance.Targets.Contains(selected));
    }

    public List<Character> GetCandidates(Character caster)
    {
        List<Character> result = new List<Character>();

        Team casterTeam = caster.Team;
        if (CanSelectAllies)
        {
            result.AddRange(casterTeam.Characters);
        }
        if (CanSelectEnemies)
        {
            result.AddRange(casterTeam.EnemyTeam.Characters);
        }
        if (!CanSelectMelee)
        {
            result.RemoveAll(c => c.PositionHandler.Position == TeamPosition.Melee);
        }
        if (!CanSelectSupport)
        {
            result.RemoveAll(c => c.PositionHandler.Position == TeamPosition.Support);
        }
        if (CannotSelectLastMelee)
        {
            result.RemoveAll(c => c.PositionHandler.IsLastMelee);
        }
        if (CannotSelectSelf)
        {
            result.Remove(caster);
        }

        return result;
    }
}
