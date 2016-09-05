namespace SparkTech.SDK.Utils
{
    using System.Text.RegularExpressions;

    using EloBuddy;
    using EloBuddy.SDK;

    using SparkTech.SDK.Enumerations;

    using static Database;

    public static class Minion
    {
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
            return minion.DetermineType() == AIMinionType.Ward;
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
            return minion.DetermineType() == AIMinionType.Pet;
        }

        public static AIMinionType DetermineType(this Obj_AI_Minion minion)
        {
            if (minion.IsValid())
            {
                var @base = minion.CharData.BaseSkinName;

                if (NormalMinionList.Exists(@base.Equals))
                {
                    return AIMinionType.Normal;
                }

                if (SiegeMinionList.Exists(@base.Equals))
                {
                    return AIMinionType.Siege;
                }

                if (SuperMinionList.Exists(@base.Equals))
                {
                    return AIMinionType.Super;
                }

                @base = @base.ToLower();

                if (@base.Contains("ward") || @base.Contains("trinket"))
                {
                    return AIMinionType.Ward;
                }

                if (PetList.Exists(@base.Equals) || CloneList.Exists(@base.Equals))
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

                if (LegendaryNameRegex.Exists(regex => Regex.IsMatch(name, regex)))
                {
                    return AIMinionType.JungleLegendary;
                }
            }

            return AIMinionType.Unknown;
        }
    }
}