using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BobbleShooter
{
    // สร้าง class ของ bubble ที่ประกอบไปด้วย Attribute ที่จำเป็นของ Bubble

    public class Bubble
    {
        //ประกาศ Attribute

        public Vector2 Position;
        public Vector2 Direction;
        public Rectangle bubbleRect;
        public Color Color { get; private set; }
        public bool moving = false;
        public Vector2 pos_bubbleBox { get; set; }

        public List<Bubble> Neighbors { get; set; }

        public bool isDestroy = false;

        public List<Bubble> findEquals()
        {
            List<Bubble> _list = new List<Bubble>();
            _list.Add(this);
            this.isDestroy = true;

            // ทำ recursive หา bubble ที่เหมือนกัน
            findEquals(_list, this);

            return _list;
        }

        public void findEquals(List<Bubble> list, Bubble bubble)
        {
            // ประกาศ List ของ Neighbors ก่อนถึงจะใส่ได้
            Neighbors = new List<Bubble>();
            bubble.findNeighbors();

            if (bubble.Neighbors == null)
            {
                return;
            }
            foreach (Bubble bub in bubble.Neighbors)
            {
                // if it's same color, not adding it to list
                if (Color == bub.Color && !list.Contains(bub))
                {
                    bub.isDestroy = true;
                    list.Add(bub);
                    findEquals(list, bub);
                }
            }
        }

        public List<Bubble> findConnectedBubbles()
        {
            List<Bubble> _list = new List<Bubble>();
            _list.Add(this);

            //หา bubble ที่เชื่อมต่อกัน
            findConnectedBubbles(_list, this);
            return _list;
        }

        public void findConnectedBubbles(List<Bubble> _list, Bubble bubble)
        {
            bubble.findNeighbors();

            foreach (Bubble bub in bubble.Neighbors)
            {
                //ไม่มีก่อนหน้านี้
                if (!bub.isDestroy && !_list.Contains(bub))
                {
                    _list.Add(bub);

                    //
                    if (bub.pos_bubbleBox.Y == bubbleShooter.Rooflower)
                        break;
                    findConnectedBubbles(_list, bub);
                }
            }
        }

        //สร้าง Constructer ของ Bubble
        public Bubble(Vector2 position, Color color)
        {
            Position = position;
            this.Direction = Vector2.Zero;
            Color = color;
            bubbleRect = new Rectangle((int)Position.X, (int)Position.Y, 60, 60);
        }

        public void Mover()
        {
            if (moving)
            {
                this.Position += Direction;
                bubbleRect = new Rectangle((int)Position.X, (int)Position.Y, 60, 60);
            }
        }

        public void findNearest()
        {
            int fy = (int)(Position.Y - bubbleShooter.limitUp + (bubbleShooter.bubbleSize / 2)) / bubbleShooter.bubbleSize;
            int fx = (int)((Position.X - bubbleShooter.limitLeft + (bubbleShooter.bubbleSize / 2) - (((fy + bubbleShooter.Rooflower) % 2) * (bubbleShooter.bubbleSize / 2))) / bubbleShooter.bubbleSize);

            pos_bubbleBox = new Vector2(fx, fy);
        }

        public void findNeighbors()
        {
            //if (Neighbors != null)
            //{
            //    Neighbors.Clear();
            //}
            Neighbors.Clear();

            foreach (Bubble bub in bubbleShooter.bubbleStuck)
            {

                if (checkNextto(bub))
                {
                     Neighbors.Add(bub);
                }
            }

        }
        public bool checkNextto(Bubble bub) //Bubble bub0, 
        {
            //bool check = false;
            if (bub.pos_bubbleBox == new Vector2(pos_bubbleBox.X - ((pos_bubbleBox.Y + 1 + bubbleShooter.Rooflower) % 2), pos_bubbleBox.Y - 1))
            { // (X – (Y + 1) % 2, Y – 1)
                //has top left bubble
                //check = true;
                return true;
            }
            if (bub.pos_bubbleBox == new Vector2(pos_bubbleBox.X - ((pos_bubbleBox.Y + 1 + bubbleShooter.Rooflower) % 2) + 1, pos_bubbleBox.Y - 1))
            // (X – ((Y + 1) % 2) + 1, Y – 1)
            {
                // has top right bubble
                //check = true;
                return true;
            }
            if (bub.pos_bubbleBox == new Vector2(pos_bubbleBox.X - 1, pos_bubbleBox.Y))
            {
                // (X-1,Y)
                // has left bubble
                //check = true;
                return true;
            }
            if (bub.pos_bubbleBox == new Vector2(pos_bubbleBox.X + 1, pos_bubbleBox.Y))
            {
                // (X + 1, Y)
                // has right bubble
                //check = true;
                return true;
            }
            if (bub.pos_bubbleBox == new Vector2(pos_bubbleBox.X - ((pos_bubbleBox.Y + 1 + bubbleShooter.Rooflower) % 2), pos_bubbleBox.Y + 1))
            {
                // (X – (Y + 1) % 2, Y + 1)
                // has down left
                //check = true;
                return true;
            }
            if (bub.pos_bubbleBox == new Vector2(pos_bubbleBox.X + 1 - ((pos_bubbleBox.Y + 1+ bubbleShooter.Rooflower) % 2), pos_bubbleBox.Y + 1))
            {
                // (X – ((Y + 1) % 2) + 1, Y + 1)
                // has down right
                //check = true;
                return true;
            }

            return false; ; // no neighbors bubble
        }
    }
}