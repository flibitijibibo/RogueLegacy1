using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DS2DEngine;

namespace RogueCastle
{
    public class TraitType
    {
        public const byte None = 0;
        public const byte ColorBlind = 1; // Done
        public const byte Gay = 2; // Done
        public const byte NearSighted = 3; // Done
        public const byte FarSighted = 4; // Done
        public const byte Dyslexia = 5; // Done
        public const byte Gigantism = 6; // Done
        public const byte Dwarfism = 7; // Done
        public const byte Baldness = 8; // Done
        public const byte Endomorph = 9; // Done
        public const byte Ectomorph = 10; // Done
        public const byte Alzheimers = 11; // Done
        public const byte Dextrocardia = 12; // Done
        public const byte Tourettes = 13; // Done
        public const byte Hyperactive = 14; // Done
        public const byte OCD = 15; // Done
        public const byte Hypergonadism = 16; // Done
        public const byte Hypogonadism = 17; // Done
        public const byte StereoBlind = 18; // Done
        public const byte IBS = 19; // Done
        public const byte Vertigo = 20; // Done
        public const byte TunnelVision = 21; // Done
        public const byte Ambilevous = 22; // Done
        public const byte PAD = 23; // Done
        public const byte Alektorophobia = 24; // Done
        public const byte Hypochondriac = 25; // Done
        public const byte Dementia = 26; // Done
        public const byte Hypermobility = 27; // Done
        public const byte EideticMemory = 28; // Done
        public const byte Nostalgic = 29; // Done
        public const byte CIP = 30; // Done
        public const byte Savant = 31; // Done
        public const byte TheOne = 32; // Done
        public const byte NoFurniture = 33; // Done
        public const byte PlatformsOpen = 34; // Done
        public const byte Glaucoma = 35; // Done
        public const byte Clonus = 36; // Done
        public const byte Prosopagnosia = 37; // Done

        public const byte Total = 38;

        public const byte Adopted = 100;

        public static byte Rarity(byte traitType)
        {
            switch (traitType)
            {
                case (ColorBlind):
                    return 2;
                case (Gay):
                    return 1;
                case (NearSighted):
                    return 2;
                case (FarSighted):
                    return 3;
                case (Dyslexia):
                    return 3;
                case (Gigantism):
                    return 1;
                case (Dwarfism):
                    return 1;
                case (Baldness):
                    return 1;
                case (Endomorph):
                    return 1;
                case (Ectomorph):
                    return 2;
                case (Alzheimers):
                    return 3;
                case (Dextrocardia):
                    return 2;
                case (Tourettes):
                    return 1;
                case (Hyperactive):
                    return 1;
                case (OCD):
                    return 1;
                case (Hypergonadism):
                    return 1;
                case (Hypogonadism):
                    return 3;
                case (StereoBlind):
                    return 1;
                case (IBS):
                    return 2;
                case (Vertigo):
                    return 3;
                case (TunnelVision):
                    return 2;
                case (Ambilevous):
                    return 2;
                case (PAD):
                    return 2;
                case (Alektorophobia):
                    return 2;
                case (Hypochondriac):
                    return 3;
                case (Dementia):
                    return 3;
                case (Hypermobility):
                    return 2;
                case (EideticMemory):
                    return 2;
                case (Nostalgic):
                    return 3;
                case (CIP):
                    return 3;
                case (Savant):
                    return 2;
                case (TheOne):
                    return 3;
                case (NoFurniture):
                    return 2;
                case (PlatformsOpen):
                    return 2;
                case (Glaucoma):
                    return 2;
                case(Clonus):
                    return 2;
                case(Prosopagnosia):
                    return 2;
            }
            return 0;
        }

        public static string ToStringID(byte traitType)
        {
            switch (traitType)
            {
                case (ColorBlind):
                    return "LOC_ID_TRAIT_TYPE_1";
                case (Gay):
                    return "LOC_ID_TRAIT_TYPE_2";
                case (NearSighted):
                    return "LOC_ID_TRAIT_TYPE_3";
                case (FarSighted):
                    return "LOC_ID_TRAIT_TYPE_4";
                case (Dyslexia):
                    return "LOC_ID_TRAIT_TYPE_5";
                case (Gigantism):
                    return "LOC_ID_TRAIT_TYPE_6";
                case (Dwarfism):
                    return "LOC_ID_TRAIT_TYPE_7";
                case (Baldness):
                    return "LOC_ID_TRAIT_TYPE_8";
                case (Endomorph):
                    return "LOC_ID_TRAIT_TYPE_9";
                case (Ectomorph):
                    return "LOC_ID_TRAIT_TYPE_10";
                case (Alzheimers):
                    return "LOC_ID_TRAIT_TYPE_11";
                case (Dextrocardia):
                    return "LOC_ID_TRAIT_TYPE_12";
                case (Tourettes):
                    return "LOC_ID_TRAIT_TYPE_13";
                case (Hyperactive):
                    return "LOC_ID_TRAIT_TYPE_14";
                case (OCD):
                    return "LOC_ID_TRAIT_TYPE_15";
                case (Hypergonadism):
                    return "LOC_ID_TRAIT_TYPE_16";
                case (Hypogonadism):
                    return "LOC_ID_TRAIT_TYPE_17";
                case (StereoBlind):
                    return "LOC_ID_TRAIT_TYPE_18";
                case (IBS):
                    return "LOC_ID_TRAIT_TYPE_19";
                case (Vertigo):
                    return "LOC_ID_TRAIT_TYPE_20";
                case (TunnelVision):
                    return "LOC_ID_TRAIT_TYPE_21";
                case (Ambilevous):
                    return "LOC_ID_TRAIT_TYPE_22";
                case (PAD):
                    return "LOC_ID_TRAIT_TYPE_23";
                case (Alektorophobia):
                    return "LOC_ID_TRAIT_TYPE_24";
                case (Hypochondriac):
                    return "LOC_ID_TRAIT_TYPE_25";
                case (Dementia):
                    return "LOC_ID_TRAIT_TYPE_26";
                case (Hypermobility):
                    return "LOC_ID_TRAIT_TYPE_27";
                case (EideticMemory):
                    return "LOC_ID_TRAIT_TYPE_28";
                case (Nostalgic):
                    return "LOC_ID_TRAIT_TYPE_29";
                case (CIP):
                    return "LOC_ID_TRAIT_TYPE_30";
                case (Savant):
                    return "LOC_ID_TRAIT_TYPE_31";
                case (TheOne):
                    return "LOC_ID_TRAIT_TYPE_32";
                case(NoFurniture):
                    return "LOC_ID_TRAIT_TYPE_33";
                case (PlatformsOpen):
                    return "LOC_ID_TRAIT_TYPE_34";
                case (Glaucoma):
                    return "LOC_ID_TRAIT_TYPE_35";
                case(Clonus):
                    return "LOC_ID_TRAIT_TYPE_36";
                case(Prosopagnosia):
                    return "LOC_ID_TRAIT_TYPE_37";
            }
            return "NULL";
        }

        public static string DescriptionID(byte traitType, bool isFemale)
        {
            switch (traitType)
            {
                case (ColorBlind):
                    return "LOC_ID_TRAIT_DESC_1";
                case (Gay):
                    if (isFemale == true)
                        return "LOC_ID_TRAIT_DESC_2";
                    else
                        return "LOC_ID_TRAIT_DESC_2b";
                case (NearSighted):
                    return "LOC_ID_TRAIT_DESC_3";
                case (FarSighted):
                    return "LOC_ID_TRAIT_DESC_4";
                case (Dyslexia):
                    return "LOC_ID_TRAIT_DESC_5";
                case (Gigantism):
                    return "LOC_ID_TRAIT_DESC_6";
                case (Dwarfism):
                    return "LOC_ID_TRAIT_DESC_7";
                case (Baldness):
                    return "LOC_ID_TRAIT_DESC_8";
                case (Endomorph):
                    return "LOC_ID_TRAIT_DESC_9";
                case (Ectomorph):
                    return "LOC_ID_TRAIT_DESC_10";
                case (Alzheimers):
                    return "LOC_ID_TRAIT_DESC_11";
                case (Dextrocardia):
                    return "LOC_ID_TRAIT_DESC_12";
                case (Tourettes):
                    return "LOC_ID_TRAIT_DESC_13";
                case (Hyperactive):
                    return "LOC_ID_TRAIT_DESC_14";
                case (OCD):
                    return "LOC_ID_TRAIT_DESC_15";
                case (Hypergonadism):
                    return "LOC_ID_TRAIT_DESC_16";
                case (Hypogonadism):
                    return "LOC_ID_TRAIT_DESC_17";
                case (StereoBlind):
                    return "LOC_ID_TRAIT_DESC_18";
                case (IBS):
                    return "LOC_ID_TRAIT_DESC_19";
                case (Vertigo):
                    return "LOC_ID_TRAIT_DESC_20";
                case (TunnelVision):
                    return "LOC_ID_TRAIT_DESC_21";
                case (Ambilevous):
                    return "LOC_ID_TRAIT_DESC_22";
                case (PAD):
                    return "LOC_ID_TRAIT_DESC_23";
                case (Alektorophobia):
                    return "LOC_ID_TRAIT_DESC_24";
                case (Hypochondriac):
                    return "LOC_ID_TRAIT_DESC_25";
                case (Dementia):
                    return "LOC_ID_TRAIT_DESC_26";
                case (Hypermobility):
                    return "LOC_ID_TRAIT_DESC_27";
                case (EideticMemory):
                    return "LOC_ID_TRAIT_DESC_28";
                case (Nostalgic):
                    return "LOC_ID_TRAIT_DESC_29";
                case (CIP):
                    return "LOC_ID_TRAIT_DESC_30";
                case (Savant):
                    return "LOC_ID_TRAIT_DESC_31";
                case (TheOne):
                    return "LOC_ID_TRAIT_DESC_32";
                case (NoFurniture):
                    return "LOC_ID_TRAIT_DESC_33";
                case (PlatformsOpen):
                    return "LOC_ID_TRAIT_DESC_34";
                case (Glaucoma):
                    return "LOC_ID_TRAIT_DESC_35";
                case(Clonus):
                    return "LOC_ID_TRAIT_DESC_36";
                case(Prosopagnosia):
                    return "LOC_ID_TRAIT_DESC_37";
            }
            return "NULL";
        }

        public static string ProfileCardDescriptionID(byte traitType)
        {
            switch (traitType)
            {
                case (ColorBlind):
                    return "LOC_ID_TRAIT_PROF_1";
                case (Gay):
                    if (Game.PlayerStats.IsFemale == true)
                        return "LOC_ID_TRAIT_PROF_2";
                    else
                        return "LOC_ID_TRAIT_PROF_2b";
                case (NearSighted):
                    return "LOC_ID_TRAIT_PROF_3";
                case (FarSighted):
                    return "LOC_ID_TRAIT_PROF_4";
                case (Dyslexia):
                    return "LOC_ID_TRAIT_PROF_5";
                case (Gigantism):
                    return "LOC_ID_TRAIT_PROF_6";
                case (Dwarfism):
                    return "LOC_ID_TRAIT_PROF_7";
                case (Baldness):
                    return "LOC_ID_TRAIT_PROF_8";
                case (Endomorph):
                    return "LOC_ID_TRAIT_PROF_9";
                case (Ectomorph):
                    return "LOC_ID_TRAIT_PROF_10";
                case (Alzheimers):
                    return "LOC_ID_TRAIT_PROF_11";
                case (Dextrocardia):
                    return "LOC_ID_TRAIT_PROF_12";
                case (Tourettes):
                    return "LOC_ID_TRAIT_PROF_13";
                case (Hyperactive):
                    return "LOC_ID_TRAIT_PROF_14";
                case (OCD):
                    return "LOC_ID_TRAIT_PROF_15";
                case (Hypergonadism):
                    return "LOC_ID_TRAIT_PROF_16";
                case (Hypogonadism):
                    return "LOC_ID_TRAIT_PROF_17";
                case (StereoBlind):
                    return "LOC_ID_TRAIT_PROF_18";
                case (IBS):
                    return "LOC_ID_TRAIT_PROF_19";
                case (Vertigo):
                    return "LOC_ID_TRAIT_PROF_20";
                case (TunnelVision):
                    return "LOC_ID_TRAIT_PROF_21";
                case (Ambilevous):
                    return "LOC_ID_TRAIT_PROF_22";
                case (PAD):
                    return "LOC_ID_TRAIT_PROF_23";
                case (Alektorophobia):
                    return "LOC_ID_TRAIT_PROF_24";
                case (Hypochondriac):
                    return "LOC_ID_TRAIT_PROF_25";
                case (Dementia):
                    return "LOC_ID_TRAIT_PROF_26";
                case (Hypermobility):
                    return "LOC_ID_TRAIT_PROF_27";
                case (EideticMemory):
                    return "LOC_ID_TRAIT_PROF_28";
                case (Nostalgic):
                    return "LOC_ID_TRAIT_PROF_29";
                case (CIP):
                    return "LOC_ID_TRAIT_PROF_30";
                case (Savant):
                    return "LOC_ID_TRAIT_PROF_31";
                case (TheOne):
                    return "LOC_ID_TRAIT_PROF_32";
                case (NoFurniture):
                    return "LOC_ID_TRAIT_PROF_33";
                case (PlatformsOpen):
                    return "LOC_ID_TRAIT_PROF_34";
                case (Glaucoma):
                    return "LOC_ID_TRAIT_PROF_35";
                case (Clonus):
                    return "LOC_ID_TRAIT_DESC_36";
                case (Prosopagnosia):
                    return "LOC_ID_TRAIT_DESC_37";
            }
            return "NULL";
        }

        public static Vector2 CreateRandomTraits()
        {
            Vector2 traitsToReturn = Vector2.Zero;

            int numTraits = 0;
            int maxTraits = 2;//3; //The maximum number of traits a player can have.
            int baseChanceForTrait = 94;//60;//100; //90; //The Base chance of the player getting at least 1 trait.
            int dropRateChanceForTrait = 39;//45;//40; //The percent chance of getting a consecutive trait.
            int minChanceForTrait = 1; //Minimum chance for the player to get a trait.
            int traitChance = CDGMath.RandomInt(0, 100);

            // Start by getting the number of traits this lineage has.
            for (int i = 0; i < maxTraits; i++)
            {
                if (traitChance < baseChanceForTrait)
                    numTraits++;

                baseChanceForTrait -= dropRateChanceForTrait;
                if (baseChanceForTrait < minChanceForTrait)
                    baseChanceForTrait = minChanceForTrait;
            }

            int[] rarityParams = new int[] { 48, 37, 15 }; //Sets odds of being rarity 1,2, or 3. The higher, the rarer.
            byte rarity = 0;
            int totalChance = 0;
            int chance = CDGMath.RandomInt(0, 100);
            List<byte> traitList = new List<byte>();

            if (numTraits > 0)
            {
                // Calculate the rarity for trait 1.
                for (int k = 0; k < 3; k++)
                {
                    totalChance += rarityParams[k];
                    if (chance <= totalChance)
                    {
                        rarity = (byte)(k + 1);
                        break;
                    }
                }

                // Find all traits of the equivalent rarity for trait 1.
                for (byte i = 0; i < TraitType.Total; i++)
                {
                    if (rarity == TraitType.Rarity(i))
                        traitList.Add(i);
                }

                // Set trait 1's trait.
                traitsToReturn.X = traitList[CDGMath.RandomInt(0, traitList.Count - 1)];
            }

            if (numTraits > 1)
            {
                rarity = 0;
                totalChance = 0;
                chance = CDGMath.RandomInt(0, 100);
                traitList.Clear();

                // Calculate the rarity for trait 2.
                for (int k = 0; k < 3; k++)
                {
                    totalChance += rarityParams[k];
                    if (chance <= totalChance)
                    {
                        rarity = (byte)(k + 1);
                        break;
                    }
                }

                // Find all traits of the equivalent rarity for trait 2.
                for (byte i = 0; i < TraitType.Total; i++)
                {
                    if (rarity == TraitType.Rarity(i))
                        traitList.Add(i);
                }

                // Set trait 2's trait.
                do
                {
                    traitsToReturn.Y = traitList[CDGMath.RandomInt(0, traitList.Count - 1)];
                } while (traitsToReturn.Y == traitsToReturn.X || TraitConflict(traitsToReturn) == true);
            }

            return traitsToReturn;
        }

        // Calculates whether there is a conflict between two traits.
        public static bool TraitConflict(Vector2 traits)
        {
            bool conflict = false;

            if ((traits.X == TraitType.Hypergonadism && traits.Y == TraitType.Hypogonadism) || (traits.X == TraitType.Hypogonadism && traits.Y == TraitType.Hypergonadism))
                conflict = true;

            if ((traits.X == TraitType.Endomorph && traits.Y == TraitType.Ectomorph) || (traits.X == TraitType.Ectomorph && traits.Y == TraitType.Endomorph))
                conflict = true;

            if ((traits.X == TraitType.Gigantism && traits.Y == TraitType.Dwarfism) || (traits.X == TraitType.Dwarfism && traits.Y == TraitType.Gigantism))
                conflict = true;

            if ((traits.X == TraitType.NearSighted && traits.Y == TraitType.FarSighted) || (traits.X == TraitType.FarSighted && traits.Y == TraitType.NearSighted))
                conflict = true;

            if ((traits.X == TraitType.ColorBlind && traits.Y == TraitType.Nostalgic) || (traits.X == TraitType.Nostalgic && traits.Y == TraitType.ColorBlind))
                conflict = true;

            return conflict;   
        }
    }
}
