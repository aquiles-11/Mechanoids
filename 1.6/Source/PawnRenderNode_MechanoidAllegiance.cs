using Verse;

namespace ApexMech
{
    public class PawnRenderNode_MechanoidAllegiance : PawnRenderNode
    {
        public new PawnRenderNodeProperties_MechanoidAllegiance Props => (PawnRenderNodeProperties_MechanoidAllegiance)props;
        public PawnRenderNode_MechanoidAllegiance(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            return GraphicDatabase.Get<Graphic_Multi>(Props.texPath, ShaderDatabase.CutoutWithOverlay, Props.maskPath);
        }
    }
}
