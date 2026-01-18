using System;
using UnityEngine;
using Verse;

namespace ApexMechanoids
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HotSwappableAttribute : Attribute
    {
    }
    [HotSwappable]
    public class PlatformAnimation
    {
        private PlatformAnimationConfig config;
        private int ticks;
        private Graphic graphic;
        
        public PlatformAnimation(PlatformAnimationConfig cfg)
        {
            this.config = cfg;
            if (cfg.graphicData != null)
            {
                graphic = cfg.graphicData.Graphic;
            }
        }

        public void Update(bool repairing)
        {
            if (repairing)
            {
                ticks++;
            }
        }

        public void Draw(Vector3 drawLoc, Rot4 rot)
        {
            if (graphic == null) return;
            
            float angle = (float)ticks / config.ticksToMove * 2f * (float)Math.PI;
            float offsetAmount = Mathf.Sin(angle) * (config.animationEnd - config.animationStart);
            Vector3 offset = new Vector3(0, AltitudeLayer.BuildingOnTop.AltitudeFor(), offsetAmount);
            Vector3 worldOffset = offset.RotatedBy(rot);
            Material material = graphic.MatAt(rot);
            Mesh mesh = graphic.MeshAt(rot);
            Graphics.DrawMesh(mesh, Matrix4x4.TRS(drawLoc + worldOffset, rot.AsQuat, Vector3.one), material, 0);
        }
    }
}
