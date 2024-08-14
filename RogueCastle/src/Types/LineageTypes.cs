using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public struct PlayerLineageData
    {
        public string Name;
        public byte Spell;
        public byte Class;
        public byte Age;
        public byte ChildAge;
        public bool IsFemale;

        private string m_romanNumeral;
        public string RomanNumeral
        {
            get
            {
                if (m_romanNumeral == null)
                    m_romanNumeral = "";
                return m_romanNumeral;
            }
            set { m_romanNumeral = value; }
        }

        public byte HeadPiece;
        public byte ChestPiece;
        public byte ShoulderPiece;
        public Vector2 Traits;
    }
}
