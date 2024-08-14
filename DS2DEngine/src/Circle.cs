using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DS2DEngine
{
    public struct Circle
    {
        private float m_x;
        private float m_y;
        private float m_radius;

        public Circle(float x, float y, float radius)
        {
            m_x = x;
            m_y = y;
            m_radius = radius;
        }

        public static Circle Zero
        {
            get { return new Circle(0,0,0); }
        }

        public float X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        public float Y
        {
            get { return m_y; }
            set { m_y = value; }
        }

        public Vector2 Position
        {
            get { return new Vector2(m_x, m_y); }
            set
            {
                m_x = value.X;
                m_y = value.Y;
            }
        }

        public float Radius
        {
            get { return m_radius; }
            set { m_radius = value; }
        }
    }
}
