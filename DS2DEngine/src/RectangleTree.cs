using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DS2DEngine
{
    public class RectangleTree<T>
    {
        public Dictionary<T, RectangleTreeItem> AllItems;
        protected Func<T, Rectangle> PositionDelegate;
        protected RectangleTreeNode Root;

        public class RectangleTreeItem
        {
            public RectangleTreeNode Parent;
            public Rectangle Position;
            public T Value;
        }

        public class RectangleTreeNode
        {
            public RectangleTreeNode Parent;
            public RectangleTreeNode[] Children;
            public List<RectangleTreeItem> Items;
            public Rectangle Position;
            public int ItemCount;

            public RectangleTreeNode(Rectangle dimensions, RectangleTreeNode parent)
            {
                Items = new List<RectangleTreeItem>();
                Position = dimensions;
                Parent = parent;
            }
        }

        public RectangleTree(Func<T, Rectangle> positionDelegate, Rectangle dimensions) {
            PositionDelegate = positionDelegate;
            AllItems = new Dictionary<T, RectangleTreeItem>();
            Root = new RectangleTreeNode(dimensions, null);
        }

        public void Add(T item) {
            RectangleTreeItem i = new RectangleTreeItem();
            i.Value = item;
            i.Position = PositionDelegate(item);
            AllItems.Add(item, i);
            Add(Root, i);
        }

        private bool Add(RectangleTreeNode node, RectangleTreeItem item)
        {
            Rectangle nodePosition = node.Position;
            //check if the item is entirely contained in this node
            if (nodePosition.Contains(item.Position)) 
            {
                if (node.Children == null)
                {
                    //Bucket has not been split yet
                    if (node.Items.Count < 5)
                    {
                        InnerAdd(node, item);
                        return true;
                    }

                    //Bucket needs to be split
                    CreateChildren(node);

                    //Move all nodes down
                    for (int i = 0; i < node.Items.Count; i++)
                    {
                        if (MoveDown(node.Items[i]))
                        {
                            i--;
                        }
                    }
                }
                //bucket has been split
                //try to add the item to each child
                foreach (RectangleTreeNode child in node.Children)
                {
                    if (Add(child, item))
                    {
                        node.ItemCount++;
                        return true;
                    }
                }

                //couldn't add to any children, add to this node
                InnerAdd(node, item);
                return true;
            }

            return false;
        }

        private void CreateChildren(RectangleTreeNode node)
        {
            Rectangle nodePosition = node.Position;
            int w = nodePosition.Width / 2, h = nodePosition.Height / 2;
            node.Children = new RectangleTreeNode[4] {
                new RectangleTreeNode(new Rectangle(nodePosition.X, nodePosition.Y, w, h), node),
                new RectangleTreeNode(new Rectangle(nodePosition.X + w, nodePosition.Y, w,h), node),
                new RectangleTreeNode(new Rectangle(nodePosition.X, nodePosition.Y + h, w, h), node),
                new RectangleTreeNode(new Rectangle(nodePosition.X + w, nodePosition.Y + h, w, h), node)
            };
        }

        private bool MoveDown(RectangleTreeItem item)
        {
            foreach (RectangleTreeNode child in item.Parent.Children)
            {
                if (Add(child, item))
                {
                    return true;
                }
            }

            return false;
        }

        private void InnerAdd(RectangleTreeNode node, RectangleTreeItem item)
        {
            if (item.Parent != null)
            {
                item.Parent.Items.Remove(item);
            }
            node.Items.Add(item);
            node.ItemCount++;
            item.Parent = node;
        }

        public void Remove(T item) {
            Remove(AllItems[item].Parent, AllItems[item]);
            AllItems.Remove(item);
        }

        private void Remove(RectangleTreeNode node, RectangleTreeItem item)
        {
            node.Items.Remove(item);
            item.Parent = null;

            while (node != null)
            {
                node.ItemCount--;
                if (node.ItemCount < 6)
                {
                    CollapseChildren(node);
                }
                node = node.Parent;
            }
        }

        private void CollapseChildren(RectangleTreeNode node)
        {
            foreach (RectangleTreeNode child in node.Children)
            {
                while (child.Items.Count > 0)
                {
                    MoveUp(child.Items[0]);
                }
            }

            node.Children = null;
        }

        private void MoveUp(RectangleTreeItem item)
        {
            item.Parent.Items.Remove(item);
            item.Parent.ItemCount--;
            if (item.Parent.Children != null && item.Parent.ItemCount < 6)
            {
                CollapseChildren(item.Parent);
            }
            item.Parent = item.Parent.Parent;


            if (item.Parent == null)
            {
                AllItems.Remove(item.Value);
            }
            else
            {
                item.Parent.Items.Add(item);
            }
        }

        public void UpdatePosition(T item){
            RectangleTreeItem ri = AllItems[item];
            Rectangle newPosition = PositionDelegate(item);

            if (newPosition == ri.Position)
            {
                return;
            }

            ri.Position = newPosition;
            if (ri.Parent.Position.Contains(newPosition))
            { //Step Into
                if (ri.Parent.Children != null)
                    MoveDown(ri);
            }
            else
            {
                do
                { //Step Out Of
                    MoveUp(ri);
                } while (ri.Parent != null && !ri.Parent.Position.Contains(newPosition));

                if (ri.Parent != null)
                {
                    Add(ri.Parent, ri);
                }
            }
        }

        public T[] GetItems(Rectangle area) {
            return GetItems(Root, area);
        }

        private T[] GetItems(RectangleTreeNode node, Rectangle area)
        {
            List<T> items = new List<T>();
            foreach (RectangleTreeItem item in node.Items)
            {
                if (item.Position.Intersects(area) || item.Position.Contains(area) || area.Contains(item.Position))
                {
                    items.Add(item.Value);
                }
            }

            if (node.Children != null)
            {
                foreach (RectangleTreeNode child in node.Children)
                {
                    if (area.Contains(child.Position))
                    {
                        items.AddRange(GetAllItems(node));
                    }
                    else if (child.Position.Contains(area))
                    {
                        items.AddRange(GetItems(child, area));
                        break;
                    }
                    else if (child.Position.Intersects(area))
                    {
                        items.AddRange(GetItems(child, area));
                    }
                }
            }

            return items.ToArray();
        }

        private T[] GetAllItems(RectangleTreeNode node)
        {
            T[] items = new T[node.ItemCount];
            int i = 0;
            foreach (RectangleTreeItem item in node.Items)
            {
                items[i++] = item.Value;
            }

            if (node.Children != null)
            {
                int q;
                foreach (RectangleTreeNode child in node.Children)
                {
                    T[] tmp = GetAllItems(child);
                    for (q = i; q < tmp.Length + i; q++)
                    {
                        items[q] = tmp[q - i];
                    }
                    i = q;
                }
            }

            return items;
        }

        public T[] GetItems(Point point) {
            return GetItems(Root, point);
        }

        private T[] GetItems(RectangleTreeNode node, Point point)
        {
            List<T> items = new List<T>();
            foreach (RectangleTreeItem item in node.Items)
            {
                if (item.Position.Contains(point))
                {
                    items.Add(item.Value);
                }
            }

            if (node.Children != null)
            {
                foreach (RectangleTreeNode child in node.Children)
                {
                    if(child.Position.Contains(point)) {
                        items.AddRange(GetItems(child, point));
                        break;
                    }
                }
            }
            return items.ToArray();
        }
    }

    public class RectangleDrawer<T> : RectangleTree<T>
    {
        Texture2D pixel;

        public RectangleDrawer(GraphicsDevice graphicsDevice, Func<T, Rectangle> positionDelegate, Rectangle dimensions)
            : base(positionDelegate, dimensions)
        {
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Draw(Root, spriteBatch);
        }

        private void Draw(RectangleTreeNode Root, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(pixel, new Rectangle(Root.Position.X, Root.Position.Y, 1, Root.Position.Height), Color.White);
            spriteBatch.Draw(pixel, new Rectangle(Root.Position.X, Root.Position.Y, Root.Position.Width, 1), Color.White);
            spriteBatch.Draw(pixel, new Rectangle(Root.Position.X, Root.Position.Bottom, Root.Position.Width, 1), Color.White);
            spriteBatch.Draw(pixel, new Rectangle(Root.Position.Right, Root.Position.Y, 1, Root.Position.Height), Color.White);

            if (Root.Children != null)
            {
                foreach (RectangleTreeNode child in Root.Children)
                {
                    Draw(child, spriteBatch);
                }
            }
        }
    }
}