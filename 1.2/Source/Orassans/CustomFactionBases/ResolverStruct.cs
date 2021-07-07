using RimWorld.BaseGen;
using System;
using Verse;

namespace CustomFactionBase
{
    public struct ResolverStruct
    {
        Predicate<ResolveParams> enabler;
        string symbol;
        float chance;

        public string RuleDefName => this.symbol;

        private bool Chance => Rand.Chance(this.chance / 100f);

        public bool Active(ResolveParams rp) => this.Chance && this.enabler(rp);

        public ResolverStruct(Predicate<ResolveParams> enabler, string symbol, float chance)
        {
            this.enabler = enabler;
            this.symbol = symbol;
            this.chance = chance;
        }
    }
}
