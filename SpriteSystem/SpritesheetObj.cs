using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SpriteSystem
{
    public class SpritesheetObj
    {
        private string _spritesheetObjName;
        private string _spritesheetName;
        private List<ImageData> _imageDataList;
        private List<FrameData> _frameDataList;
        private bool m_isDisposed = false;

        public SpritesheetObj(string objName, string spritesheetName)
        {
            _spritesheetObjName = objName;
            _spritesheetName = spritesheetName;
            _imageDataList = new List<ImageData>();
            _frameDataList = new List<FrameData>();
       
        }

        public void AddImageData(int index, int posX, int posY, int anchorX, int anchorY, int width, int height)
        {
            ImageData newImageData = new ImageData();
            newImageData.Index = index;
            newImageData.SSPos.X = posX;
            newImageData.SSPos.Y = posY;
            newImageData.Anchor.X = anchorX;
            newImageData.Anchor.Y = anchorY;
            newImageData.Width = width;
            newImageData.Height = height;
            _imageDataList.Add(newImageData);
        }

        public void SortImageData()
        {
            _imageDataList.Sort(ImageDataComparator);
        }

        private int ImageDataComparator(ImageData id1, ImageData id2)
        {
            if (id1.Index > id2.Index)
                return 1;
            else if (id1.Index < id2.Index)
                return -1;
            else return 0;
        }

        public void AddFrameData(int frame, int index, int duration, string label)
        {
            if (duration < 1)
                throw new Exception("Frame data cannot have a duration of 0 frames");
            
            FrameData newFrameData = new FrameData();
            newFrameData.hitboxList = new List<Hitbox>();
            newFrameData.Frame = frame;
            newFrameData.Index = index;
            newFrameData.Duration = duration;
            newFrameData.Label = label;
            _frameDataList.Add(newFrameData);
        }

        //Adds a hitbox to the hitboxlist in the last FrameData object created by calling AddFrameData().
        public void AddHitboxToLastFrameData(int x, int y, int width, int height, float rotation, int type)
        {
            Hitbox newHitbox = new Hitbox();
            newHitbox.X = x;
            newHitbox.Y = y;
            newHitbox.Width = width;
            newHitbox.Height = height;
            newHitbox.Rotation = rotation;
            newHitbox.Type = type;

            _frameDataList[_frameDataList.Count - 1].hitboxList.Add(newHitbox);
        }

        public FrameData FrmData(int index)
        { return _frameDataList[index]; }

        public int FrameCount()
        { return _frameDataList.Count; }

        public ImageData ImgData(int index)
        { return _imageDataList[index]; }

        public Vector2 SSPos(int index)
        { return _imageDataList[index].SSPos; }

        public Vector2 Anchor(int index)
        { return _imageDataList[index].Anchor; }

        public int Width(int index)
        { return _imageDataList[index].Width; }

        public int Height(int index)
        { return _imageDataList[index].Height; }

        public List<ImageData> ImgDataList()
        { return _imageDataList; }

        public List<FrameData> FrmDataList()
        { return _frameDataList; }

        public string SpritesheetObjName
        {
            get { return _spritesheetObjName; }
        }

        public string SpritesheetName
        {
            get { return _spritesheetName; }
        }

        public bool IsDisposed
        {
            get { return m_isDisposed; }
        }

        public void Dispose()
        {
            if (m_isDisposed == false)
            {
                m_isDisposed = true;
                _imageDataList.Clear();
                _imageDataList = null;
                foreach (FrameData fd in _frameDataList)
                {
                    fd.Dispose();
                }
                _frameDataList.Clear();
                _frameDataList = null;

            }
        }
    }

    public struct ImageData
    {
        public Vector2 SSPos; // Spritesheet Position
        public Vector2 Anchor;
        public int Index;
        public int Width;
        public int Height;
    }

    public class FrameData
    {
        public int Frame;
        public int Index;
        public int Duration;
        public List<Hitbox> hitboxList;
        public string Label;

        public void Dispose()
        {
            hitboxList.Clear();
            hitboxList = null;
        }
    }

    public struct CharacterData
    {
        public string Child;
        public int ChildX;
        public int ChildY;
    }

    public struct Hitbox
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public float Rotation;
        public int Type;
    }
}
