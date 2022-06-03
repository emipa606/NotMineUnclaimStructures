using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace NotMine;

public class Designator_Unclaim : Designator
{
    public Designator_Unclaim()
    {
        defaultLabel = "DesignatorUnclaim".Translate();
        defaultDesc = "DesignatorUnclaimDesc".Translate();
        icon = ContentFinder<Texture2D>.Get("UI/Designators/Unclaim");
        soundDragSustain = SoundDefOf.Designate_DragStandard;
        soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
        useMouseIcon = true;
        soundSucceeded = SoundDefOf.Designate_Claim;
        hotKey = KeyBindingDefOf.Misc4;
    }

    public override int DraggableDimensions => 2;

    public override AcceptanceReport CanDesignateCell(IntVec3 c)
    {
        AcceptanceReport result;
        if (!c.InBounds(Map))
        {
            result = false;
        }
        else
        {
            if (c.Fogged(Map))
            {
                result = false;
            }
            else
            {
                if (!(from t in c.GetThingList(Map)
                        where CanDesignateThing(t).Accepted
                        select t).Any())
                {
                    result = "MessageMustDesignateClaimable".Translate();
                }
                else
                {
                    result = true;
                }
            }
        }

        return result;
    }

    public override void DesignateSingleCell(IntVec3 c)
    {
        var thingList = c.GetThingList(Map);
        foreach (var thing in thingList)
        {
            var accepted = CanDesignateThing(thing).Accepted;
            if (accepted)
            {
                DesignateThing(thing);
            }
        }
    }

    public override AcceptanceReport CanDesignateThing(Thing t)
    {
        return t is Building building && building.Faction == Faction.OfPlayer;
    }

    public override void DesignateThing(Thing t)
    {
        var newFaction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.Insect);
        t.SetFaction(newFaction);
        FleckMaker.ThrowMetaPuffs(t.OccupiedRect(), t.Map);
    }
}