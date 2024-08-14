using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Randomchaos2DGodRays
{
    public class ScreenQuad
    {
        VertexPositionTexture[] corners;
        VertexBuffer vb;
        short[] ib;
        VertexDeclaration vertDec;

        Game Game;

        public ScreenQuad(Game game)
        {
            Game = game;
            corners = new VertexPositionTexture[4];
            corners[0].Position = new Vector3(0, 0, 0);
            corners[0].TextureCoordinate = Vector2.Zero;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public virtual void Initialize()
        {
            vertDec = VertexPositionTexture.VertexDeclaration;

            corners = new VertexPositionTexture[]
                    {
                        new VertexPositionTexture(
                            new Vector3(1,-1,0),
                            new Vector2(1,1)),
                        new VertexPositionTexture(
                            new Vector3(-1,-1,0),
                            new Vector2(0,1)),
                        new VertexPositionTexture(
                            new Vector3(-1,1,0),
                            new Vector2(0,0)),
                        new VertexPositionTexture(
                            new Vector3(1,1,0),
                            new Vector2(1,0))
                    };

            ib = new short[] { 0, 1, 2, 2, 3, 0 };
            vb = new VertexBuffer(Game.GraphicsDevice, typeof(VertexPositionTexture), corners.Length, BufferUsage.None);
            vb.SetData(corners);
        }

        public virtual void Draw()
        {            
            Game.GraphicsDevice.SetVertexBuffer(vb);
            Game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, corners, 0, 4, ib, 0, 2);
        }
    }
}
