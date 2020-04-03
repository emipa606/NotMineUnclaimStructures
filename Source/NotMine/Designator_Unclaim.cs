using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace NotMine
{
	// Token: 0x02000002 RID: 2
	public class Designator_Unclaim : Designator
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public Designator_Unclaim()
		{
			this.defaultLabel = Translator.Translate("DesignatorUnclaim");
			this.defaultDesc = Translator.Translate("DesignatorUnclaimDesc");
			this.icon = ContentFinder<Texture2D>.Get("UI/Designators/Unclaim", true);
			this.soundDragSustain = SoundDefOf.Designate_DragStandard;
			this.soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
			this.useMouseIcon = true;
			this.soundSucceeded = SoundDefOf.Designate_Claim;
			this.hotKey = KeyBindingDefOf.Misc4;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x000020CC File Offset: 0x000002CC
		public override int DraggableDimensions
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020E0 File Offset: 0x000002E0
		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			bool flag = !c.InBounds(base.Map);
			AcceptanceReport result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = c.Fogged(base.Map);
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !(from t in c.GetThingList(base.Map)
					where this.CanDesignateThing(t).Accepted
					select t).Any<Thing>();
					if (flag3)
					{
						result = Translator.Translate("MessageMustDesignateClaimable");
					}
					else
					{
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x0000216C File Offset: 0x0000036C
		public override void DesignateSingleCell(IntVec3 c)
		{
			List<Thing> thingList = c.GetThingList(base.Map);
			for (int i = 0; i < thingList.Count; i++)
			{
				bool accepted = this.CanDesignateThing(thingList[i]).Accepted;
				if (accepted)
				{
					this.DesignateThing(thingList[i]);
				}
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000021CC File Offset: 0x000003CC
		public override AcceptanceReport CanDesignateThing(Thing t)
		{
			Building building = t as Building;
			return building != null && building.Faction == Faction.OfPlayer;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002200 File Offset: 0x00000400
		public override void DesignateThing(Thing t)
		{
			Faction newFaction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.Insect);
			t.SetFaction(newFaction, null);
			CellRect.CellRectIterator iterator = t.OccupiedRect().GetIterator();
			while (!iterator.Done())
			{
				MoteMaker.ThrowMetaPuffs(new TargetInfo(iterator.Current, base.Map, false));
				iterator.MoveNext();
			}
		}
	}
}
