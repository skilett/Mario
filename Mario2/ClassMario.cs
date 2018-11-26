using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Mario2
{
    public class Game
    {
        Control controlForm;
        public Physics Phisic;
        public Map level;
        public Unit player;
        List<Monster> listMonsters = new List<Monster>();
        public Interface life;
        public Game(Control Controls)
        {
            Phisic = new Physics(this);
            controlForm = Controls;
            level = new Map(controlForm, new Point(10, 10), new Point(1000, 500));
            player = newPlayer(380, 120);
            newLife();
            life.Value(player.live);
            newMonster(580, 200);
            newMonster(700, 200);
            newMonster(180, 200);
            newMonster(220, 250);
            newMonster(110, 250);
            newMonster(110, 300);
            newMonster(110, 350);
            newMonster(410, 250);
            newMonster(510, 250);
            Update();
        }
        private Unit newPlayer(int x, int y)
        {
            player = new Unit(controlForm, new Animate(@"C:\Users\belov.vasily\Documents\Visual Studio 2010\Projects\Mario2\spriteMario.png"), Phisic, new Point(x, y));
            player.anim.Add("stay");
            player.anim.state("stay").Add(new Rectangle(7, 176, 37, 37));

            player.anim.Add("right");
            player.anim.state("right").Add(new Rectangle(85, 176, 37, 37));
            player.anim.state("right").Add(new Rectangle(85, 215, 37, 37));
            player.anim.state("right").Add(new Rectangle(7, 215, 37, 37));
            player.anim.state("right").Add(new Rectangle(46, 254, 37, 37));
            player.anim.state("right").Add(new Rectangle(7, 254, 37, 37));
            player.anim.state("right").Add(new Rectangle(46, 254, 37, 37));

            player.anim.Add("left");
            player.anim.state("left").Add(new Rectangle(85, 176, 37, 37));
            player.anim.state("left").Add(new Rectangle(7, 215, 37, 37));
            player.anim.state("left").Add(new Rectangle(46, 215, 37, 37));
            player.anim.state("left").Add(new Rectangle(85, 215, 37, 37));

            return player;
        }
        private void newMonster(int x, int y)
        {
            listMonsters.Add(new Monster(controlForm, new Animate(@"C:\Users\belov.vasily\Documents\Visual Studio 2010\Projects\Mario2\spriteMario.png"), Phisic, new Point(x, y)));

            int index = listMonsters.Count - 1;
            listMonsters[index].anim.Add("stay");
            listMonsters[index].anim.state("stay").Add(new Rectangle(158, 7, 40, 44));

            listMonsters[index].anim.Add("left");
            listMonsters[index].anim.state("left").Add(new Rectangle(158, 7, 40, 44));
            listMonsters[index].anim.state("left").Add(new Rectangle(205, 7, 40, 44));
            listMonsters[index].anim.state("left").Add(new Rectangle(250, 7, 40, 44));

            listMonsters[index].anim.Add("right");
            listMonsters[index].anim.state("right").Add(new Rectangle(158, 7, 40, 44));
            listMonsters[index].anim.state("right").Add(new Rectangle(205, 7, 40, 44));
            listMonsters[index].anim.state("right").Add(new Rectangle(250, 7, 40, 44));

            listMonsters[index].autoMove = true;
            listMonsters[index].maxVx = 2;
            listMonsters[index].vx = -2;
            listMonsters[index].live = 1;
        }
        private void newLife()
        { 
            life = new Interface(controlForm, new Point(50, 0), new Point(32, 32));
            life.Add(new Rectangle(192, 0, 64, 64));
            life.Add(new Rectangle(192, 0, 64, 64));
            life.Add(new Rectangle(128, 0, 64, 64));
            life.Add(new Rectangle(64, 0, 64, 64));
            life.Add(new Rectangle(0, 0, 64, 64));          
        }
        public void Tick()
        {
            player.immortal--;
            if (player.live > 0) Phisic.Tick(player);
            for (int i = 0; i < listMonsters.Count; i++)
            {
                if (listMonsters[i].live > 0) Phisic.Tick(listMonsters[i]);
            }
            //удаляем мертвых монстров
            for (int i = 0; i < listMonsters.Count; i++)
            {
                if (listMonsters[i].live == 0)
                    listMonsters.RemoveAt(i);

            }
            
        }
        public void Update()
        {
            controlForm.Update();
            player.Update();
            for (int i = 0; i < listMonsters.Count; i++)
            {
                if (listMonsters[i].live > 0) listMonsters[i].Update();
            }
            level.Update();
            life.Update();
        }
        public bool Collision(Unit units, int x, int y) //ограничиваем перемещение по X
        {
            if (x > 0) units.state = "right";
            else if (x < 0) units.state = "left";
            else units.state = "stay";   

            bool change = false;
            if (x > 0)
            {
                int xWall = level.distWall(units, 'R');
                if (xWall < units.X + x)
                {
                    x = xWall - units.X;
                    units.vx = 0;
                }
                change = true;
            }
            else if (x < 0)
            {
                int xWall = level.distWall(units, 'L');
                if (xWall > units.X + x)
                {
                    x = xWall - units.X;
                    units.vx = 0;
                }
                change = true;
            }
            //перемещаем объект по x
            units.MovePoz(x, 0);

            if (y > 0)
            {
                int xWall = level.distWall(units, 'D');
                if (xWall < units.Y + y)
                {
                    y = xWall - units.Y;
                    units.vy = 0;
                    units.ground = true;
                    units.jump = 0;
                }
                change = true;
            }
            else if (y < 0)
            {
                int xWall = level.distWall(units, 'U');
                if (xWall > units.Y + y)
                {
                    y = xWall - units.Y;
                    units.vy = 0;
                }
                change = true;
            }

            //перемещаем объект по x
            units.MovePoz(0, y);
            if ((x == 0) && (y == 0) && (units.state != "stay"))
            {                
                units.Invalidate();
                units.state = "stay";
            }
            CollisionMonster();     
            //проверяем задели ли интрактивные элементы
            level.collision(units);
                   
            return change;
        }
        void CollisionMonster()
        {
            for (int i = 0; i < listMonsters.Count; i++)
            {
                if (listMonsters[i].live > 0) CollisionPic(listMonsters[i]);
            }
        }
        bool CollisionPic(Monster monsters)
        {
            if (CollisionPoint(player.X, player.Y, monsters) 
              || CollisionPoint(player.X, player.Y + player.Height, monsters)
              || CollisionPoint(player.X + player.Width, player.Y, monsters) 
              || CollisionPoint(player.X + player.Width, player.Y + player.Height, monsters)) //если игрок пересекся с монстром
            {
                if ((player.Y <= monsters.Y) && (player.vy > 0)) //если игрок падает на монстра
                {//мочим монстра                    
                    monsters.live--;
                    player.Score += 10;
                    controlForm.Invalidate(new Rectangle(monsters.X, monsters.Y, monsters.Width, monsters.Height));
                }
                else
                {//коцаем игрока если он не в защите
                    if (player.immortal < 1)
                    {
                        player.live--;
                        life.Value(player.live); //выводим актуальное здоровье
                        player.immortal = 20; //неуязвимость
                    }
                }
                return true;
            }
            else
            {
                //снимаем защиту
                return false;
            }
        }
        bool CollisionPoint(int x, int y, Monster monsters)
        {
            if ((x >= monsters.X) && (x < monsters.X + monsters.Width) && (y >= monsters.Y) && (y < monsters.Y + monsters.Height))
                return true;
            return false;
        }
    }

    public abstract class BaseClass
    {
        //свойства
        public Control controlForm;
        protected GraphicsUnit sizeScrean = GraphicsUnit.Pixel;       
        protected Graphics graphic; 
        protected int sizeSpline = 48;
        public int X = 0;
        public int Y = 0;
        public int Height = 1; //высота
        public int Width = 1; //ширина
        //методы
        public BaseClass() 
        {    
        }
        public BaseClass(Control controls) : this()
        {
            controlForm = controls;            
        }
        public abstract void Update();
        public virtual void Move(int x, int y)
        {            
        }
    }
    // *************************************************************player********************************************************************
    public class Unit : BaseClass
    {
        //свойства
        public bool autoMove = false;
        public int Score = 0;
        public int immortal = 0; //сколько тиков не уязвимости
        public int maxVx = 30, maxVy = 40;//максимальная скорость объекта
        public int impulsX = 12, impulsY = 40;
        public int jump = 0; //кол-во прыжков        
        string _state = "stay";     //состояние (стоит бежит падает)
        public int live = 4;
        public Animate anim;
        public Physics phisic;  //-------------------------------переместить в Game
        public bool ground = false; //стоит на земле
        protected int _vx = 0;
        public int vy = 0;
        public Unit(Control controls, Animate animates, Physics phisic, Point poz) : base(controls)
        {
            //controlForm.Controls.Add(pic);
            //graphic = pic.CreateGraphics();
            graphic = controlForm.CreateGraphics();
            Height = sizeSpline;
            Width = sizeSpline;
            anim = animates;
            this.phisic = phisic;
            X = poz.X;
            Y = poz.Y;
        }    
        public virtual int vx //скорость объекта 
        {
            set
            {
                if (Math.Abs(value) > maxVx)
                    _vx = maxVx * Math.Sign(value);
                else
                    _vx = value;
            }
            get { return _vx; }
        }
        public Rectangle coordRect()
        {
            return new Rectangle(X, Y, Width, Height);
        }
        public string state
        {
            get { return _state; }
            set
            {
                if (value == "right")
                { anim.Direction = true; }
                else if (value == "left")
                { anim.Direction = false; };
                if (value == "stay")
                { anim.state(value).step = 0; }
                _state = value;
            }
        }
        Rectangle skinRect()
        {
            Rectangle r = anim.state(state).Rect();
            return r;
        }
        public override void Move(int x, int y)
        {
            if (y < 0) //прыжок
            {
                ground = false;
                jump++;
            }
            //проверка на двойной прыжок
            if (jump > 2)
                y = 0;
            phisic.Move(this, x * impulsX, y * impulsY);
        }
        public void MovePoz(int x, int y) //cдвигает все координаты на указанную величину
        {
            if (Y + y > 550)  //временное ограничение по падению
            {
                y = 0;
                jump = 0;
                ground = true;
            }

            if ((x != 0) || (y != 0))
            //передвигаем объект
            {
                Invalidate();
                X = X + x;
                Y = Y + y;
            }   
        }
        public override void Update()
        {          
            if (live > 0)
            {
                Point[] poz = new Point[] {
                    new Point(X, Y),
                    new Point(X + Width, Y),
                    new Point(X, Y + Height)
                };
                graphic.DrawImage(anim.Texture, poz, skinRect(), sizeScrean);
            }
            //else controlForm.Invalidate(new Rectangle(X, Y, Width, Height));
        }
        public void Invalidate()
        {
            controlForm.Invalidate(new Rectangle(X, Y, Width, Height));
            //controlForm.Update();
        }
    }
    //*************************************************************monster**************************************************************
    public class Monster : Unit
    {        
        public char Direction = 'R';
        public int Price = 10;
        public Monster(Control controls, Animate animates, Physics phisic, Point poz) : base(controls, animates, phisic, poz)
        {
        }
        public override int vx
        {
            set 
            {
                if (value == 0)
                    if (Direction == 'R')
                    {
                        Direction = 'L';
                        _vx = -maxVx;
                    }
                    else
                    {
                        Direction = 'R';
                        _vx = maxVx;
                    }
                else _vx = value;
            }
            get { return _vx; }
        }
    }
    // *************************************************************map********************************************************************
    public class Map : BaseClass
    {
        //свойства
        public string[] TileMap;
        //int Height = 0, Width = 0;
        public Image Texture; //ссылка на файл с которого читаем картинку    
        public Image BackGround;
        //методы
        public Map(Control controls, Point poz, Point size)
            : base(controls)
        {
            graphic = controls.CreateGraphics();
            string[] TM2 = {
                        "                                                                  B",
                        "    M                                                             B",
                        "B  MMM           B                                                B",
                        "B  MMMM          B                                                B",
                        "B MMMMMM    MMMM B                                                B",
                        "B          MMMMMMB         MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM     B",
                        "BBB     MMMMMBBBBB                                                B",
                        "B    MMMMMMMMMM BB                                                B",
                        "B                                       MMMMMMMMMMMMM             B",
                        "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB"
                        };
            TileMap = TM2;
            Height = TileMap.Length; //высота
            Width = TileMap[0].Length; //ширина
            Texture = new Bitmap(Image.FromFile(@"C:\Users\belov.vasily\Documents\Visual Studio 2010\Projects\Mario2\SpriteMario.png"));
            BackGround = new Bitmap(Image.FromFile(@"C:\Users\belov.vasily\Documents\Visual Studio 2010\Projects\Mario2\Desert.jpg"));
            controlForm.BackgroundImage = BackGround;  
        }
        public override void Move(int x, int y)
        {
            controlForm.Invalidate();
            X += x;
            Y += y;
        }
        public override void Update()
        {
            Point[] RectWall = new Point[3];
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    switch (TileMap[i][j])
                    {
                        case 'B': 
                            RectWall = new Point[] {
                            new Point(X + sizeSpline * j, Y + sizeSpline * i),
                            new Point(X + sizeSpline * j + sizeSpline, Y + sizeSpline * i),
                            new Point(X + sizeSpline * j, Y + sizeSpline * i + sizeSpline)
                            };
                            graphic.DrawImage(Texture, RectWall, new Rectangle(643, 171, 28, 29), sizeScrean);  /// прописать отрисовку в битмапе а потом уже на форме
                        break;
                        case 'M':
                            RectWall = new Point[] {
                            new Point(X + sizeSpline * j, Y + sizeSpline * i),
                            new Point(X + sizeSpline * j + sizeSpline, Y + sizeSpline * i),
                            new Point(X + sizeSpline * j, Y + sizeSpline * i + sizeSpline)
                            };
                            graphic.DrawImage(Texture, RectWall, new Rectangle(358, 12, 21, 30), sizeScrean);  /// прописать отрисовку в битмапе а потом уже на форме
                        break;
                    }
                }                
        }
        public int distWall(Unit units, Char direction)
        {

            int pozX = getPozX(units.X);
            int pozY = getPozY(units.Y);
            if (pozX > Width - 1) pozX = Width - 1; //защита карты от переполнения
            else if (pozX < 0) pozX = 0;
            if (pozY > Height - 1) pozY = Height - 1;
            else if (pozY < 0) pozY = 0;
            int result = 0;
            switch (direction)
            {
                case 'L':
                    result = 0;
                    for (int i = pozX - 1; i >= 0; i--)
                    {
                        if (TileMap[pozY][i] == 'B')
                        {
                            result = i + 1;
                            break;
                        }
                    }
                    pozY = getPozY(units.Y + units.Height - 1); //проверяем 2ю точку
                    for (int i = pozX - 1; i >= result; i--)
                    {
                        if (TileMap[pozY][i] == 'B')
                        {
                            result = i + 1;
                            break;
                        }
                    }
                    break;
                case 'R':
                    pozX = getPozX(units.X + units.Width - 1);
                    result = Width;
                    for (int i = pozX + 1; i < Width; i++)
                    {
                        if (TileMap[pozY][i] == 'B') 
                        {
                            result = i - 1;
                            break;
                        }
                    }
                    pozY = getPozY(units.Y + units.Height - 1); //проверяем 2ю точку
                    for (int i = pozX + 1; i < result; i++)
                    {
                        if (TileMap[pozY][i] == 'B')
                        {
                            result = i - 1;
                            break;
                        }
                    }
                    break;
                case 'U': 
                    result = 0;
                    for (int j = pozY - 1; j >= 0; j--)
                    {
                        if (TileMap[j][pozX] == 'B')
                        {
                            result = j + 1;
                            break;
                        }
                    }
                    pozX = getPozX(units.X + units.Width - 1); //проверяем по 2й точке
                    for (int j = pozY - 1; j >= result; j--)
                    {
                        if (TileMap[j][pozX] == 'B')
                        {
                            result = j + 1;
                            break;
                        }
                    }
                    break;
                case 'D': //+
                    pozY = getPozY(units.Y + units.Height-1);
                    result = Height;
                    for (int j = pozY + 1; j < Height; j++)
                    {
                        if (TileMap[j][pozX] == 'B')
                        {
                            result = j - 1;
                            break;
                        }
                    }
                    pozX = (int)((units.X + units.Width - 1) / sizeSpline); //проверяем по 2й точке
                    for (int j = pozY + 1; j < result; j++)
                    {
                        if (TileMap[j][pozX] == 'B')
                        {
                            result = j - 1;
                            break;
                        }
                    }
                    break;
            }
            return result * sizeSpline; 
        }
        public void collision(Unit units)
        {
            //проверяем все 4 точки юнита на взоимодействие с объектами на карте
            int pozXX = 0;
            int pozYY = 0;
            if (collisionPoint(units, units.X, units.Y, out pozXX, out pozYY) != ' ')
            {
                units.Score++;
                controlForm.Invalidate(new Rectangle(pozXX, pozYY, sizeSpline, sizeSpline));
            }
            if (collisionPoint(units, units.X + units.Width, units.Y, out pozXX, out pozYY) != ' ')
            {
                units.Score++;
                controlForm.Invalidate(new Rectangle(pozXX, pozYY, sizeSpline, sizeSpline));
            }
            if (collisionPoint(units, units.X, units.Y + units.Height, out pozXX, out pozYY) != ' ')
            {
                units.Score++;
                controlForm.Invalidate(new Rectangle(pozXX, pozYY, sizeSpline, sizeSpline));
            }
            if (collisionPoint(units, units.X + units.Width, units.Y + units.Height, out pozXX, out pozYY) != ' ')
            {
                units.Score++;
                controlForm.Invalidate(new Rectangle(pozXX, pozYY, sizeSpline, sizeSpline));
            }
        } //результаты взаимодействия с объектами на карте
        private char collisionPoint(Unit units, int x, int y, out int pozX, out int pozY)
        {
            int indexX = getPozX(x);
            int indexY = getPozY(y);
            char result = ' ';
            if (TileMap[indexY][indexX] == 'M')
            {
                result = TileMap[indexY][indexX];
                TileMap[indexY] = ReplaceCharInString(TileMap[indexY], indexX, 'C');

            }
            pozX = indexX * sizeSpline;
            pozY = indexY * sizeSpline;
            return result;
        }//взаимодействие с картой
        private String ReplaceCharInString(String source, int index, Char newSymb)
        {
            char[] chars = source.ToCharArray();
            chars[index] = newSymb;
            return new String(chars);
        }
        private int getPozX(int x)
        {
            int poz = (int)(x / sizeSpline);
            if (poz < 0) return 0;
            if (poz > Width - 1) return Width - 1;
            return poz;
        }
        private int getPozY(int y)
        {
            int poz = (int)(y / sizeSpline);
            if (poz < 0) return 0;
            if (poz > Height - 1) return Height - 1;
            return poz;
        }
        
    }
    //**********************************************************interface**********************************************
    public class Interface : BaseClass
    {
        int Index = 0; //значение
        public Image Texture; //ссылка на файл с которого читаем картинку 
        List<Rectangle> Rect = new List<Rectangle>();
        public Interface(Control controls, Point poz, Point size)
            : base(controls)
        {
            X = poz.X;
            Y = poz.Y;
            Width = size.X;
            Height = size.Y;
            graphic = controls.CreateGraphics();
            Texture = new Bitmap(Image.FromFile(@"C:\Users\belov.vasily\Documents\Visual Studio 2010\Projects\Mario2\interface.png"));
        }
        public void Add(Rectangle R)
        {
            Rect.Add(R);
        }
        public void Value(int i)
        {
            if ((i < Rect.Count) && (i > -1))
            {
                Index = i;
                controlForm.Invalidate(new Rectangle(X, Y, Width, Height));
            }
        }
        public override void Update()
        {            
            if (Rect.Count > 0)
            {
                Point[] poz = new Point[] {
                    new Point(X, Y),
                    new Point(X + Width, Y),
                    new Point(X, Y + Height)
                };
                graphic.DrawImage(Texture, poz, Rect[Index], sizeScrean);
            }            
        }
        
    }
    //********************************************************Анимация******************************************************************
    public class Animate
    {
        public Image Texture; //ссылка на файл с которого читаем картинку
        cState lastState = new cState();
        public bool Direction = true;
        List<cState> uState = new List<cState>(); //состояние юнита

        public Animate(string FilePath)
        {
            Texture = (Bitmap)Image.FromFile(@FilePath);
        }

        public cState state(string nameState)
        {
            for (int i = 0; i < uState.Count; i++)
            {
                if (nameState == uState[i].Name)
                {
                    if (lastState != uState[i])
                    {
                        uState[i].step = 0;
                        lastState = uState[i];
                    }
                    return uState[i];
                }
            }
            return null;
        }

        public void Add(string nameT)
        {
            uState.Add(new cState() { Name = nameT });
        }

    }
    public class cState
    {
        public string Name;
        public int step = 0; //текущий шаг
        List<Rectangle> RectStep = new List<Rectangle>();  //массив областей текстур

        public cState()
        {
            List<Rectangle> RectStep = new List<Rectangle>();
        }

        public void Add(Rectangle Rect)
        {
            RectStep.Add(Rect);
        }

        public Rectangle Rect()
        {
            int oldStep = step;
            if (step >= RectStep.Count)
            {
                step = 1;
                return RectStep[0];
            }
            step++;
            return RectStep[oldStep];
        }
    }
    //*****************************************************физика***************************************************************************
    public class Physics
    {
        int ay = -5; //гравитация
        int friction = 2; //трение
        int time = 1; //коэф замедления времени
        private Game games;
        public Physics(Game games)
        {
            this.games = games;
        }
        public void Move(Unit units, int powerX, int powerY)  //передаем импульс объекту
        {
            units.vx += powerX;
            units.vy += powerY;              
        }

        public void Tick(Unit units) //просчет перемещения объекта за один такт
        {
            //анализ скорости, замедление
            //y
            int y = units.vy - ay;

            if (Math.Abs(y) > friction * time)
                units.vy = Math.Sign(y) * (Math.Abs(y) - friction * time);
            else
                units.vy = 0;

            if (!units.autoMove)
            {                
                //x
                if (Math.Abs(units.vx) > friction * time)
                    units.vx = Math.Sign(units.vx) * (Math.Abs(units.vx) - friction * time);
                else
                    units.vx = 0;
            }
            //теперь вычисляем перемещение
            games.Collision(units, units.vx * time, units.vy * time);
        }
    }

}
