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
        public Display disp;
        public Physics Phisic;
        public Map level;
        public Unit player;
        List<Monster> listMonsters = new List<Monster>();
        public Interface life;
        public Game(Control Controls)
        {
            Phisic = new Physics(this);
            disp = new Display(Controls);
            level = new Map(disp, new Point(10, 10), new Point(1000, 500));
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
            player = new Unit(disp, new Animate("spriteMario.png"), Phisic, new Point(x, y));
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
            listMonsters.Add(new Monster(disp, new Animate("spriteMario.png"), Phisic, new Point(x, y)));

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
            listMonsters[index].vx = -4;
            listMonsters[index].live = 1;
        }
        private void newLife()
        { 
            life = new Interface(disp, new Point(50, 0), new Point(32, 32));
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
            //передвигаем карту если игрок убежал
            if (player.X > disp.controlForm.Width - disp.sizeSpline - disp.offSetX)
            {
                disp.offSetX -= disp.controlForm.Width - disp.sizeSpline;
            }
            else if ((player.X + disp.offSetX < 0)&&(player.X > 0))
            {
                disp.offSetX += disp.controlForm.Width - disp.sizeSpline;
            }
        }
        public void Update()
        {
            //disp.Update();
            //Invalidate();//сначала зачищаем все объекты
            //отрисовываем все остальные объекты
            player.Update();
            for (int i = 0; i < listMonsters.Count; i++)
            {
                if (listMonsters[i].live > 0) listMonsters[i].Update();
            }
            level.Update();
            life.Update();
            Invalidate();//сначала зачищаем все объекты

        }
        public bool Collision(Unit units, double x, double y) //ограничиваем перемещение по X
        {
            if (x > 0) units.state = "right";
            else if (x < 0) units.state = "left";
            else units.state = "stay";   

            bool change = false;
            if (x > 0)
            {
                double xWall = level.distWall(units, 'R');
                if (xWall < units.X + x)
                {
                    x = xWall - units.X;
                    units.vx = 0;
                }
                change = true;
            }
            else if (x < 0)
            {
                double xWall = level.distWall(units, 'L');
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
                double xWall = level.distWall(units, 'D');
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
                double xWall = level.distWall(units, 'U');
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
               // units.Invalidate();
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
                   // disp.Invalidate(Convert.ToInt32(monsters.X), Convert.ToInt32(monsters.Y), monsters.Width, monsters.Height);
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
        bool CollisionPoint(double x, double y, Monster monsters)
        {
            if ((x >= monsters.X) && (x < monsters.X + monsters.Width) && (y >= monsters.Y) && (y < monsters.Y + monsters.Height))
                return true;
            return false;
        }
        public void Invalidate()
        {
            player.Invalidate();
            for (int i = 0; i < listMonsters.Count; i++)
            {
                listMonsters[i].Invalidate();
            }
        }
    }

    public abstract class BaseClass
    {
        //свойства
        protected Display disp;
        public double X = 0;
        public double Y = 0;
        public double lastX = 0;
        public double lastY = 0;
        public int Height = 1; //высота
        public int Width = 1; //ширина
        //методы
        public BaseClass() 
        {    
        }
        public BaseClass(Display disp) : this()
        {
            this.disp = disp;            
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
        public int impulsX = 24, impulsY = 70;
        public int jump = 0; //кол-во прыжков        
        string _state = "stay";     //состояние (стоит бежит падает)
        public int live = 4;
        public Animate anim;
        public Physics phisic;  //-------------------------------переместить в Game
        public bool ground = false; //стоит на земле
        protected double _vx = 0;
        public double vy = 0;
        public Unit(Display disp, Animate animates, Physics phisic, Point poz) : base(disp)
        {
            Height = disp.sizeSpline;
            Width = disp.sizeSpline;
            anim = animates;
            this.phisic = phisic;
            X = poz.X;
            Y = poz.Y;
        }    
        public virtual double vx //скорость объекта 
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
            return new Rectangle(Convert.ToInt32(X), Convert.ToInt32(Y), Width, Height);
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
        public bool changeView = true; //необходимость перерисовки объекта
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
        public void MovePoz(double x, double y) //cдвигает все координаты на указанную величину
        {
            if (Y + y > 550)  //временное ограничение по падению
            {
                y = 0;
                jump = 0;
                ground = true;
                changeView = true;
            }

            if ((x != 0) || (y != 0))
            //передвигаем объект
            {
                //   Invalidate();
                changeView = true;
                X = X + x;
                Y = Y + y;
            }   
        }
        public override void Update()
        {
            //Invalidate();
            if (live > 0)//((live > 0)&&(changeView))
            {
                disp.Draw(anim.Texture, Convert.ToInt32(X), Convert.ToInt32(Y), Width, Height, skinRect());
                lastX = X;
                lastY = Y;
                //Invalidate();
                changeView = false;
            }
        }
        public void Invalidate()
        {
            //if (changeView)
                disp.Invalidate((int) lastX, (int)lastY, Width, Height);
        }
    }
    //*************************************************************monster**************************************************************
    public class Monster : Unit
    {        
        public char Direction = 'R';
        public int Price = 10;
        public Monster(Display disp, Animate animates, Physics phisic, Point poz) : base(disp, animates, phisic, poz)
        {
        }
        public override double vx
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
        public Map(Display disp, Point poz, Point size)
            : base(disp)
        {
            string[] TM2 = {
                        "B                       M                                         B",
                        "B                      MMM                                        B",
                        "B                                                                 B",
                        "B                     BBBB                                        B",
                        "B                MMM        BB                                    B",
                        "B   MMM         B   B        B                                    B",
                        "B           BBBB BBBBB    BBBB                                    B",
                        "B        B B                                                      B",
                        "B        B                                                        B",
                        "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB"
                        };
            TileMap = TM2;
            Height = TileMap.Length; //высота
            Width = TileMap[0].Length; //ширина
            //string path = Environment.CurrentDirectory;
            Texture = new Bitmap(Image.FromFile("SpriteMario.png"));
            BackGround = new Bitmap(Image.FromFile("Desert.jpg"));
            disp.controlForm.BackgroundImage = BackGround;  
        }
        public override void Move(int x, int y)
        {
            //disp.Invalidate();
            X += x;
            Y += y;
        }
        public override void Update()
        {
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    switch (TileMap[i][j])
                    {
                        case 'B': 
                            disp.Draw(Texture, disp.sizeSpline * j, disp.sizeSpline * i, disp.sizeSpline, disp.sizeSpline, new Rectangle(643, 171, 28, 29));                            
                            break;
                        case 'M':
                            disp.Draw(Texture, disp.sizeSpline * j, disp.sizeSpline * i, disp.sizeSpline, disp.sizeSpline, new Rectangle(358, 12, 21, 30));
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
                    result = Width; //сохраняем предел карты
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
                    pozX = (int)((units.X + units.Width - 1) / disp.sizeSpline); //проверяем по 2й точке
                    for (int j = pozY + 1; j <= result; j++)
                    {
                        if (TileMap[j][pozX] == 'B')
                        {
                            result = j - 1;
                            break;
                        }
                    }
                    break;
            }
            if (result == 0) result = -10;
            return result * disp.sizeSpline; 
        }
        public void collision(Unit units)
        {
            //проверяем все 4 точки юнита на взоимодействие с объектами на карте
            int pozXX = 0;
            int pozYY = 0;
            if (collisionPoint(units, units.X, units.Y, out pozXX, out pozYY) != ' ')
            {
                units.Score++;
                disp.Invalidate(pozXX, pozYY, disp.sizeSpline, disp.sizeSpline);                
            }
            if (collisionPoint(units, units.X + units.Width, units.Y, out pozXX, out pozYY) != ' ')
            {
                units.Score++;
                disp.Invalidate(pozXX, pozYY, disp.sizeSpline, disp.sizeSpline);
            }
            if (collisionPoint(units, units.X, units.Y + units.Height, out pozXX, out pozYY) != ' ')
            {
                units.Score++;
                disp.Invalidate(pozXX, pozYY, disp.sizeSpline, disp.sizeSpline);
            }
            if (collisionPoint(units, units.X + units.Width, units.Y + units.Height, out pozXX, out pozYY) != ' ')
            {
                units.Score++;
                disp.Invalidate(pozXX, pozYY, disp.sizeSpline, disp.sizeSpline);
            }
        } //результаты взаимодействия с объектами на карте
        private char collisionPoint(Unit units, double x, double y, out int pozX, out int pozY)
        {
            int indexX = getPozX(x);
            int indexY = getPozY(y);
            char result = ' ';
            if (TileMap[indexY][indexX] == 'M')
            {
                result = TileMap[indexY][indexX];
                TileMap[indexY] = ReplaceCharInString(TileMap[indexY], indexX, 'C');

            }
            pozX = indexX * disp.sizeSpline;
            pozY = indexY * disp.sizeSpline;
            return result;
        }//взаимодействие с картой
        private String ReplaceCharInString(String source, int index, Char newSymb)
        {
            char[] chars = source.ToCharArray();
            chars[index] = newSymb;
            return new String(chars);
        }
        private int getPozX(double x)
        {
            int poz = (int)(x / disp.sizeSpline);
            if (poz < 0) return 0;
            if (poz > Width - 1) return Width - 1;
            return poz;
        }
        private int getPozY(double y)
        {
            int poz = (int)(y / disp.sizeSpline);
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
        public Interface(Display disp, Point poz, Point size)
            : base(disp)
        {
            X = poz.X;
            Y = poz.Y;
            Width = size.X;
            Height = size.Y;
            //graphic = controls.CreateGraphics();
            Texture = new Bitmap(Image.FromFile("interface.png"));
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
                disp.Invalidate((int) X,  (int) Y, Width, Height);
                //controlForm.Invalidate(new Rectangle(X, Y, Width, Height));
            }
        }
        public override void Update()
        {            
            if (Rect.Count > 0)
                disp.Draw(Texture, Convert.ToInt32(X), Convert.ToInt32(Y), Width, Height, Rect[Index]);
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
        int ay = -10; //гравитация
        int friction = 6; //трение
        double time = 0.5; //коэф замедления времени --влияет на фпс
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
            double y = units.vy - ay;

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
    public class Display
    {
        public Control controlForm;
        Graphics graphic;
        public int sizeSpline = 48;
        int _offSetX = 0;
        int _offSetY = 0;
        public int offSetX
        {
            get { return _offSetX; }
            set 
            {
               // Invalidate();
                _offSetX = value;
            }
        }
        public int offSetY = 0;
        public Display(Control controls)
        {
            controlForm = controls;
            graphic = controlForm.CreateGraphics();
            offSetX = 0;
            offSetY = 0;
        }
        public void Invalidate()
        {
            controlForm.Invalidate();
        }
        public void Invalidate(int x, int y, int width, int height)
        {
            controlForm.Invalidate(new Rectangle(x + offSetX, y + offSetY, width, height));
        }
        public void Draw(Image texture, int x, int y, int width, int height, Rectangle rect)
        {
            x += offSetX;
            y += offSetY;
            Point[] poz = new Point[] {
                    new Point(x, y),
                    new Point(x + width, y),
                    new Point(x, y + height)};
            graphic.DrawImage(texture, poz, rect, GraphicsUnit.Pixel);
        }
        public void Update()
        {
            controlForm.Update();
        }
    }
}
