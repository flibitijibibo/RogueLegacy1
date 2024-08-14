using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public struct FamilyTreeNode
    {
        public string Name;
        public byte Age;
        public byte ChildAge;
        public byte Class;
        public byte HeadPiece;
        public byte ChestPiece;
        public byte ShoulderPiece;
        public int NumEnemiesBeaten;
        public bool BeatenABoss;
        public bool IsFemale;
        public Vector2 Traits;
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
    }
}
