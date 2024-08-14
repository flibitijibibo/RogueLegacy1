using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS2DEngine
{
    public interface IClickableObj
    {
        //bool LeftJustPressed(Camera2D camera);
        //bool LeftPressed(Camera2D camera);
        //bool LeftJustReleased(Camera2D camera);

        //bool RightJustPressed(Camera2D camera);
        //bool RightPressed(Camera2D camera);
        //bool RightJustReleased(Camera2D camera);

        //bool MiddleJustPressed(Camera2D camera);
        //bool MiddlePressed(Camera2D camera);
        //bool MiddleJustReleased(Camera2D camera);

        bool LeftMouseOverJustPressed(Camera2D camera);
        bool LeftMouseOverPressed(Camera2D camera);
        //bool LeftJustReleased(Camera2D camera);

        bool RightMouseOverJustPressed(Camera2D camera);
        bool RightMouseOverPressed(Camera2D camera);
        //bool RightJustReleased(Camera2D camera);

        bool MiddleMouseOverJustPressed(Camera2D camera);
        bool MiddleMouseOverPressed(Camera2D camera);
        //bool MiddleJustReleased(Camera2D camera);
    }
}
