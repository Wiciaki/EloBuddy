namespace SparkTech.SDK.Utils
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using EloBuddy;

    using SparkTech.SDK.Enumerations;

    public static class Minion
    {
        private static readonly List<string> CloneList = new List<string> { "leblanc", "shaco", "monkeyking" };

        private static readonly List<string> NormalMinionList = new List<string>
                                                                    {
                                                                        "SRU_ChaosMinionMelee", "SRU_ChaosMinionRanged",
                                                                        "SRU_OrderMinionMelee", "SRU_OrderMinionRanged",
                                                                        "HA_ChaosMinionMelee", "HA_ChaosMinionRanged",
                                                                        "HA_OrderMinionMelee", "HA_OrderMinionRanged"
                                                                    };

        private static readonly List<string> PetList = new List<string>
                                                           {
                                                               "annietibbers", "elisespiderling", "heimertyellow",
                                                               "heimertblue", "malzaharvoidling", "shacobox",
                                                               "yorickspectralghoul", "yorickdecayedghoul",
                                                               "yorickravenousghoul", "zyrathornplant",
                                                               "zyragraspingplant"
                                                           };

        private static readonly List<string> SiegeMinionList = new List<string>
                                                                   {
                                                                       "SRU_ChaosMinionSiege", "SRU_OrderMinionSiege",
                                                                       "HA_ChaosMinionSiege", "HA_OrderMinionSiege"
                                                                   };

        private static readonly List<string> SuperMinionList = new List<string>
                                                                   {
                                                                       "SRU_ChaosMinionSuper", "SRU_OrderMinionSuper",
                                                                       "HA_ChaosMinionSuper", "HA_OrderMinionSuper"
                                                                   };

        private static readonly List<string> SmallNameRegex = new List<string> { "SRU_[a-zA-Z](.*?)Mini", "Sru_Crab" };

        private static readonly List<string> LargeNameRegex = new List<string>
            {
                "SRU_Murkwolf[0-9.]{1,}", "SRU_Gromp", "SRU_Blue[0-9.]{1,}",
                "SRU_Razorbeak[0-9.]{1,}", "SRU_Red[0-9.]{1,}",
                "SRU_Krug[0-9]{1,}"
            };

        private static readonly List<string> LegendaryNameRegex = new List<string> { "SRU_Dragon", "SRU_Baron", "SRU_RiftHerald" };

        public static AIMinionType DetermineType(this Obj_AI_Minion minion)
        {
            var baseSkinName = minion.CharData.BaseSkinName;

            if (NormalMinionList.Exists(baseSkinName.Equals))
            {
                return AIMinionType.Normal;
            }

            if (SiegeMinionList.Exists(baseSkinName.Equals))
            {
                return AIMinionType.Siege;
            }

            if (SuperMinionList.Exists(baseSkinName.Equals))
            {
                return AIMinionType.Super;
            }

            baseSkinName = baseSkinName.ToLower();

            if (baseSkinName.Contains("ward") || baseSkinName.Contains("trinket"))
            {
                return AIMinionType.Ward;
            }

            if (PetList.Exists(baseSkinName.Equals))
            {
                return AIMinionType.Pet;
            }

            var name = minion.Name;

            if (SmallNameRegex.Exists(regex => Regex.IsMatch(name, regex)))
            {
                return AIMinionType.JungleSmall;
            }

            if (LargeNameRegex.Exists(regex => Regex.IsMatch(name, regex)))
            {
                return AIMinionType.JungleLarge;
            }

            return LegendaryNameRegex.Exists(regex => Regex.IsMatch(name, regex)) ? AIMinionType.JungleLegendary : AIMinionType.Unknown;
        }

        public static bool IsJungleBuff(this Obj_AI_Minion minion)
        {
            var baseSkinName = minion.CharData.BaseSkinName;
            return baseSkinName == "SRU_Blue" || baseSkinName == "SRU_Red";
        }

        public static bool IsMinion(this Obj_AI_Minion minion)
        {
            return minion.DetermineType().IsMinion();
        }

        public static bool IsMinion(this AIMinionType type)
        {
            return type == AIMinionType.Normal || type == AIMinionType.Siege || type == AIMinionType.Super;
        }

        public static bool IsWard(this Obj_AI_Minion minion)
        {
            return minion.DetermineType().IsWard();
        }

        public static bool IsWard(this AIMinionType type)
        {
            return type == AIMinionType.Ward;
        }

        public static bool IsJungle(this Obj_AI_Minion minion)
        {
            return minion.DetermineType().IsJungle();
        }

        public static bool IsJungle(this AIMinionType type)
        {
            return type == AIMinionType.JungleSmall || type == AIMinionType.JungleLarge || type == AIMinionType.JungleLegendary;
        }

        public static bool IsPet(this Obj_AI_Minion minion)
        {
            return minion.DetermineType().IsPet();
        }

        public static bool IsPet(this AIMinionType type)
        {
            return type == AIMinionType.Pet;
        }
    }
}