using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Apex.render
{
    public class PawnRenderNode_Cape : PawnRenderNode
    {
        public PawnRenderNode_Cape(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
             : base(pawn, props, tree)
        {
        }

        public override GraphicMeshSet MeshSetFor(Pawn pawn)
        {
            Pawn_StyleTracker style = pawn.style;
            if (((style != null) ? style.beardDef : null) == null || pawn.style.beardDef.noGraphic)
            {
                return null;
            }
            return HumanlikeMeshPoolUtility.GetHumanlikeBeardSetForPawn(pawn, 1f, 1f);
        }
    }
    public class PawnRenderNodeWorker_Cape : PawnRenderNodeWorker_FlipWhenCrawling
    {
        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            return base.CanDrawNow(node, parms) && (parms.facing != Rot4.North || parms.flipHead);
        }

        public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
        {
            HeadTypeDef headType = parms.pawn.story.headType;
            Vector3 vector = base.OffsetFor(node, parms, out pivot);
            if (parms.facing == Rot4.East)
            {
                vector += Vector3.right * headType.beardOffsetXEast;
            }
            else if (parms.facing == Rot4.West)
            {
                vector += Vector3.left * headType.beardOffsetXEast;
            }
            return vector + (headType.beardOffset + parms.pawn.style.beardDef.GetOffset(headType, parms.facing));
        }
    }
}