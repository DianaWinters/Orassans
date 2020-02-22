using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using Verse.AI.Group;
using static CustomFactionBase.CustomBaseUtility;

namespace CustomFactionBase
{
    internal static class SymbolResolverAdder
    {
        internal static List<ResolverStruct> resolvers = new List<ResolverStruct>();

        public static void _Resolve(this SymbolResolver _this, ResolveParams rp)
        {
            
        }
    }
}