using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace BetharianPower
{
    /// <summary>
    /// Betharian stone grid storing how strong the stone contents are.
    /// </summary>
    public class BetharianGrid : ICellBoolGiver, IExposable
    {
        public Map map;

        private ushort[] strengthGrid;
        private CellBoolDrawer drawer;

        public BetharianGrid(Map map)
        {
            this.map = map;
            strengthGrid = new ushort[map.cellIndices.NumGridCells];
            drawer = new CellBoolDrawer(this, map.Size.x, map.Size.z, 0.33f);
        }

        public int StrengthAt(IntVec3 c)
        {
            return strengthGrid[map.cellIndices.CellToIndex(c)];
        }

        public void SetStrengthAt(IntVec3 c, ushort strength)
        {
            if (strength > ushort.MaxValue)
            {
                Log.Error("Cannot store strength " + strength + " in BetharianGrid: out of ushort range.");
                strength = ushort.MaxValue;
            }
            if (strength < ushort.MinValue)
            {
                Log.Error("Cannot store strength " + strength + " in BetharianGrid: out of ushort range.");
                strength = ushort.MinValue;
            }
            strengthGrid[map.cellIndices.CellToIndex(c)] = strength;

            drawer.SetDirty();
        }

        public void GridUpdate()
        {
            drawer.CellBoolDrawerUpdate();
        }

        public void MarkForDraw()
        {
            if (map == Find.CurrentMap)
            {
                drawer.MarkForDraw();
            }
        }

        public Color Color => new Color(0.63f, 0.27f, 0.63f, 1f);

        public void ExposeData()
        {
            MapExposeUtility.ExposeUshort(this.map, 
                (IntVec3 c) => strengthGrid[this.map.cellIndices.CellToIndex(c)], 
                delegate (IntVec3 c, ushort val)
                {
                    strengthGrid[this.map.cellIndices.CellToIndex(c)] = val;
                }
                , "strengthGrid");
        }

        public bool GetCellBool(int index)
        {
            return StrengthAt(map.cellIndices.IndexToCell(index)) > 0;
        }

        public Color GetCellExtraColor(int index)
        {
            return Color;
        }
    }
}
