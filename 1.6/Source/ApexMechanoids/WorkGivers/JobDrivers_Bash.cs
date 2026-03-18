using DelaunatorSharp;
using Gilzoide.ManagedJobs;
using Ionic.Crc;
using Ionic.Zlib;
using JetBrains.Annotations;
using KTrie;
using LudeonTK;
using NVorbis.NAudioSupport;
using RimWorld;
using RimWorld.BaseGen;
using RimWorld.IO;
using RimWorld.Planet;
using RimWorld.QuestGen;
using RimWorld.SketchGen;
using RimWorld.Utility;
using RuntimeAudioClipLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Grammar;
using Verse.Noise;
using Verse.Profile;
using Verse.Sound;
using Verse.Steam;
using static HarmonyLib.Code;

namespace ApexMechanoids
{
	public class JobDriver_Bash : JobDriver
	{
		public LocalTargetInfo Target => job.GetTarget(TargetIndex.A);

		private Vector3 exactPos;

		private Vector3 direction;

		public float moveSpeed = 1f;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref exactPos, "exactPos");
			Scribe_Values.Look(ref direction, "direction");
			Scribe_Values.Look(ref moveSpeed, "moveSpeed");
		}

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		public override void Notify_Starting()
		{
			zeroPos = true;
			try
			{
				exactPos = pawn.TrueCenter();
			}
			finally
			{
				zeroPos = false;
			}
			direction = Target.CenterVector3 - exactPos;
			Log.Message(direction.ToString());
			direction = direction.normalized;
			Log.Message(direction.ToString());
			direction = direction.Yto0() * moveSpeed;
			base.Notify_Starting();
			Log.Message(direction.ToString());
		}

		public override IEnumerable<Toil> MakeNewToils()
		{
			Toil toil1 = ToilMaker.MakeToil("MoveBash");
			toil1.tickAction = delegate
			{
				toil1.actor.rotationTracker.FaceCell(Target.Cell);
				IntVec3 pos1 = exactPos.ToIntVec3();
				exactPos += direction;
				IntVec3 pos2 = exactPos.ToIntVec3();
				if (pos1 != pos2)
				{
					TryEnterNextPathCell(pawn, pos2);
				}
			};
			toil1.handlingFacing = true;
			toil1.defaultCompleteMode = ToilCompleteMode.Never;
			yield return toil1;
			Toil toil2 = ToilMaker.MakeToil("ActionBash");
			toil2.initAction = delegate
			{
				pawn.Drawer.tweener.ResetTweenedPosToRoot();
				DoBashAction();
			};
			toil2.handlingFacing = true;
			toil2.defaultCompleteMode = ToilCompleteMode.Instant;
			yield return toil2;
		}

		protected virtual void DoBashAction()
		{
			Find.CameraDriver.shaker.SetMinShake(0.1f);
		}

		protected virtual void TryEnterNextPathCell(Pawn pawn, IntVec3 nextCell)
		{
			if(Mathf.DeltaAngle((Target.CenterVector3 - exactPos).AngleFlat(), direction.AngleFlat()) > 60f || Target.Cell.DistanceTo(pawn.Position) <= 1.1f)
			{
				pawn.Position = TargetA.Cell;
				pawn.jobs.curDriver.ReadyForNextToil();
			}
			else
			{
				pawn.Position = nextCell;
			}
			pawn.filth.Notify_EnteredNewCell();
		}

		private bool zeroPos = false;

		public override Vector3 ForcedBodyOffset
		{
			get
			{
				if (zeroPos)
				{
					return Vector3.zero;
				}
				zeroPos = true;
				Vector3 vec;
				try
				{
					vec = pawn.DrawPos;
				}
				finally
				{
					zeroPos = false;
				}
				return exactPos - vec;
			}
		}
	}

	public class JobDriver_BashStun : JobDriver_Bash
	{
		protected override void DoBashAction()
		{
			base.DoBashAction();
			DamageInfo dinfo = new DamageInfo(DamageDefOf.Stun, job.maxNumMeleeAttacks, instigator: pawn);
			foreach (IntVec3 cell in CellRect.FromCell(pawn.Position).ExpandedBy(1))
			{
				foreach(Thing t in cell.GetThingList(pawn.Map).ToList())
				{
					if(t != pawn)
					{
						t.TakeDamage(dinfo);
					}
				}
			}
		}
	}

	public class JobDriver_BashDamage : JobDriver_Bash
	{
		protected override void DoBashAction()
		{
			base.DoBashAction();
			DamageInfo dinfo = new DamageInfo(DamageDefOf.Blunt, job.maxNumMeleeAttacks, DamageDefOf.Blunt.defaultArmorPenetration, instigator: pawn);
			foreach (IntVec3 cell in CellRect.FromCell(pawn.Position).ExpandedBy(1))
			{
				foreach (Thing t in cell.GetThingList(pawn.Map).ToList())
				{
					if (t == pawn || (t.Faction != null && !t.HostileTo(pawn)))
					{
						continue;
					}
					t.TakeDamage(dinfo);
				}
			}
		}
	}
}