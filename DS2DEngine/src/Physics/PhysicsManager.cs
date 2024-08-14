using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DS2DEngine
{
    public class PhysicsManager: IDisposable
    {
        private LinkedList<IPhysicsObj> _poList;
        public int TerminalVelocity { get; set; }

        private Vector2 _gravity = Vector2.Zero; // In metres per second.

        private Color m_damageHitboxColor = new Color(255, 0, 0, 100);
        private Color m_attackHitboxColor = new Color(0, 0, 155, 100);
        private Color m_collisionHitboxColor = new Color(0, 53, 0, 100);
        private Camera2D m_camera;

        private bool m_isDisposed = false;

        public PhysicsManager()
        {
            _poList = new LinkedList<IPhysicsObj>();
            TerminalVelocity = 1000;
        }

        public void Initialize(Camera2D camera)
        {
            m_camera = camera;
        }

        public void AddObject(IPhysicsObj obj)
        {
            if (obj.Node == null)
            {
                obj.Node = _poList.AddLast(obj);
                obj.PhysicsMngr = this;
            }
            else
            {
                _poList.AddLast(obj.Node);
                obj.PhysicsMngr = this;
            }

            // Optimization - cache the physics values for each collision box
            // Calling UpdateCachedValues in the constructor of CollisionBoxes alone
            // did not work.  Without this cache update, the character would
            // fall through the world on the first frame and end up below the ground.
            foreach (CollisionBox box in obj.CollisionBoxes)
            {
                box.UpdateCachedValues();
            }
        }

        public void RemoveObject(IPhysicsObj obj)
        {
            if (obj.Node != null && obj.PhysicsMngr != null)
            {
               // try
                {
                    _poList.Remove(obj.Node);
                    obj.PhysicsMngr = null;
                }
               // catch { Console.WriteLine("Could not remove " + obj + " from Physics Manager as it was not found."); }
            }
        }

        public void RemoveAllObjects()
        {
            int counter = _poList.Count;
            while (counter > 0)
            {
                RemoveObject(_poList.Last.Value);
                counter--;
            }
        }

        public void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (IPhysicsObj weightedObj in _poList)
            {
                if (weightedObj.Visible == true)// && CollisionMath.Intersects(weightedObj.Bounds, m_camera.LogicBounds))
                {
                    if (weightedObj.DisableAllWeight == false)
                    {
                        if (weightedObj.IsWeighted == true)
                        {
                            Vector2 gravity = new Vector2(_gravity.X * elapsedTime, _gravity.Y * elapsedTime);

                            if (weightedObj.AccelerationXEnabled == true && weightedObj.DisableGravity == false)
                                weightedObj.AccelerationX += gravity.X;
                            if (weightedObj.AccelerationYEnabled == true && weightedObj.DisableGravity == false)
                                weightedObj.AccelerationY += gravity.Y;

                            if (weightedObj.AccelerationY > TerminalVelocity)
                                weightedObj.AccelerationY = TerminalVelocity;
                        }

                        weightedObj.UpdatePhysics(gameTime);
                    }
                }
                // Update code, physics, and collision code goes here. Only update collision boxes if the flag isn't disabled. (Needed in case you manually add collision boxes, in which case this method
                // would constantly override those hitboxes.
                if (weightedObj.DisableHitboxUpdating == false)
                    weightedObj.UpdateCollisionBoxes();

                weightedObj.UseCachedValues = true;
                foreach (CollisionBox box in weightedObj.CollisionBoxes)
                    box.UpdateCachedValues();
            }
          
            //Converted to linked list, and had IPhysicsObjs hold a reference to their linkedlist node, so that their addition and removal from the Physics Manager is quick and painless.
            LinkedListNode<IPhysicsObj> firstNode = _poList.First;
            LinkedListNode<IPhysicsObj> secondNode = null;
            if (firstNode != null) secondNode = firstNode.Next;

            while (firstNode != null)
            {
                IPhysicsObj firstObj = firstNode.Value;
                int firstBoxHBCount = firstObj.CollisionBoxes.Count;

                if (firstObj.IsCollidable == true && firstObj.Visible == true)// && CollisionMath.Intersects(firstObj.Bounds, m_camera.LogicBounds))
                {
                    while (secondNode != null)
                    {
                        IPhysicsObj secondObj = secondNode.Value;
                        int secondBoxHBCount = secondObj.CollisionBoxes.Count;
                        secondNode = secondNode.Next;

                        // If both objects are the same type (i.e. enemies) & SameType flag is false for BOTH, move to next iteration.
                        if (firstObj.CollisionTypeTag == secondObj.CollisionTypeTag && (firstObj.SameTypesCollide == false && secondObj.SameTypesCollide == false))
                            continue;

                         if (secondObj.IsCollidable == true && secondObj.Visible == true)// && CollisionMath.Intersects(secondObj.Bounds, m_camera.LogicBounds))
                         {
                             for (int l = 0; l < firstBoxHBCount; l++)
                             {
                                 CollisionBox firstBox = firstObj.CollisionBoxes[l];
                                 
                                 Rectangle firstBoxAbsRect = firstBox.AbsRect; // Optimization so we don't keep pinging it, despite being cached.
                                 float firstBoxAbsRotation = firstBox.AbsRotation; // Optimization so we don't keep pinging it, despite being cached.

                                 for (int m = 0; m < secondBoxHBCount; m++)
                                 {
                                     CollisionBox secondBox = secondObj.CollisionBoxes[m];

                                     Rectangle secondBoxAbsRect = secondBox.AbsRect;
                                     float secondBoxAbsRotation = secondBox.AbsRotation;

                                     if (firstBox.Type != Consts.NULL_HITBOX && secondBox.Type != Consts.NULL_HITBOX && firstBox.Parent.Visible == true && secondBox.Parent.Visible == true)
                                     {
                                         bool runHitDetection = false;
                                         int collisionResponseType = 0;
                                         // Perform terrain hit detection.
                                         if (firstBox.Type == Consts.TERRAIN_HITBOX && secondBox.Type == Consts.TERRAIN_HITBOX )//&& (firstObj.IsWeighted == false || secondObj.IsWeighted == false))
                                         {
                                             runHitDetection = true;
                                             collisionResponseType = Consts.COLLISIONRESPONSE_TERRAIN;
                                         }
                                         // Perform weapon/body body/weapon hit detection.
                                         else if ((firstBox.Type == Consts.WEAPON_HITBOX && secondBox.Type == Consts.BODY_HITBOX) || (firstBox.Type == Consts.BODY_HITBOX && secondBox.Type == Consts.WEAPON_HITBOX))
                                         {
                                             runHitDetection = true;
                                             if (firstBox.Type == Consts.WEAPON_HITBOX)
                                                 collisionResponseType = Consts.COLLISIONRESPONSE_SECONDBOXHIT;
                                             else
                                                 collisionResponseType = Consts.COLLISIONRESPONSE_FIRSTBOXHIT;
                                         }

                                         if (runHitDetection == true)
                                         {
                                             if (CollisionMath.RotatedRectIntersects(firstBoxAbsRect, firstBoxAbsRotation, Vector2.Zero,
                                                                                     secondBoxAbsRect, secondBoxAbsRotation, Vector2.Zero) == true)
                                             {
                                                 if (collisionResponseType == Consts.COLLISIONRESPONSE_FIRSTBOXHIT)
                                                 {
                                                     firstObj.CollisionResponse(firstBox, secondBox, Consts.COLLISIONRESPONSE_FIRSTBOXHIT);
                                                     secondObj.CollisionResponse(secondBox, firstBox, Consts.COLLISIONRESPONSE_SECONDBOXHIT);
                                                 }
                                                 else if (collisionResponseType == Consts.COLLISIONRESPONSE_SECONDBOXHIT)
                                                 {
                                                     firstObj.CollisionResponse(firstBox, secondBox, Consts.COLLISIONRESPONSE_SECONDBOXHIT);
                                                     secondObj.CollisionResponse(secondBox, firstBox, Consts.COLLISIONRESPONSE_FIRSTBOXHIT);
                                                 }
                                                 else
                                                 {
                                                     firstObj.CollisionResponse(firstBox, secondBox, collisionResponseType);
                                                     secondObj.CollisionResponse(secondBox, firstBox, collisionResponseType);
                                                 }

                                                 firstBox.UpdateCachedValues();
                                                 secondBox.UpdateCachedValues();
                                             }
                                         }
                                     }
                                 }
                             }
                         }
                    }
                }

                firstNode = firstNode.Next;
                if (firstNode != null)
                    secondNode = firstNode.Next;
            }
            
            foreach (IPhysicsObj weightedObj in _poList)
            {
                weightedObj.UseCachedValues = false;
            }
        }

        public void SetGravity(float xGrav, float yGrav)
        {
            _gravity.X = xGrav;
            _gravity.Y = yGrav;
        }

        public Vector2 GetGravity()
        {
            return _gravity;
        }

        public LinkedList<IPhysicsObj> ObjectList
        {
            get {return _poList;}
        }

        public void DrawAllCollisionBoxes(SpriteBatch spriteBatch, Texture2D texture, int collBoxType)
        {
            foreach (IPhysicsObj po in _poList)
            {
                foreach (CollisionBox cb in po.CollisionBoxes)
                {
                    if (cb.Type == collBoxType && cb.Parent.Visible == true && cb.AbsParent.Visible == true)
                    {
                        switch (cb.Type)
                        {
                            case (Consts.WEAPON_HITBOX):
                                //spriteBatch.Draw(texture, cb.AbsRect, m_attackHitboxColor);
                                spriteBatch.Draw(texture, cb.AbsRect, null, m_attackHitboxColor, MathHelper.ToRadians(cb.AbsRotation), Vector2.Zero, SpriteEffects.None, 1);
                                break;
                            case (Consts.BODY_HITBOX):
                                //spriteBatch.Draw(texture, cb.AbsRect, m_damageHitboxColor);
                                spriteBatch.Draw(texture, cb.AbsRect, null, m_damageHitboxColor, MathHelper.ToRadians(cb.AbsRotation), Vector2.Zero, SpriteEffects.None, 1);
                                break;
                            case (Consts.TERRAIN_HITBOX):
                                //spriteBatch.Draw(texture, cb.AbsRect, m_collisionHitboxColor);
                                spriteBatch.Draw(texture, cb.AbsRect, null, m_collisionHitboxColor, MathHelper.ToRadians(cb.AbsRotation), Vector2.Zero, SpriteEffects.None, 1);
                                break;

                        }
                    }
                }
            }
        }

        public bool IsDisposed
        {
            get { return m_isDisposed;}
        }

        public void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Physics Manager");

                _poList.Clear();
                _poList = null;
                m_isDisposed = true;
            }
        }
    }
}
