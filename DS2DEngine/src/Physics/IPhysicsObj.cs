using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DS2DEngine
{
    public interface IPhysicsObj
    {
        bool CollidesLeft { get; set; }
        bool CollidesRight { get; set; }
        bool CollidesTop { get; set; }
        bool CollidesBottom { get; set; }
        bool Visible { get; set; }
        bool SameTypesCollide { get; set; }
        LinkedListNode<IPhysicsObj> Node { get; set; }

        bool DisableHitboxUpdating { get; set; }

        int CollisionTypeTag { get; set; } // A generic tag that developers can use to tag the type of collision object you are running into.
        bool IsWeighted { get; set; }
        bool AccelerationXEnabled { get; set; }
        bool AccelerationYEnabled { get; set; }
        bool IsCollidable { get; set; }
        float AccelerationX { get; set; }
        float AccelerationY { get; set; }
        float X { get; set; }
        float Y { get; set; }
        float Rotation { get; set; }
        int Width { get; }
        int Height { get; }

        Rectangle Bounds { get; }
        Rectangle TerrainBounds { get; }

        PhysicsManager PhysicsMngr { get; set; }

        List<CollisionBox> CollisionBoxes { get; }

        void UpdatePhysics(GameTime gameTime);
        void UpdateCollisionBoxes();
        void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType);
        void RemoveFromPhysicsManager();

        bool DisableAllWeight { get; set; }

        bool HasTerrainHitBox { get; }
        bool DisableCollisionBoxRotations { get; set; }
        bool DisableGravity { get; set; }
        bool UseCachedValues { get; set; }
    }
}
