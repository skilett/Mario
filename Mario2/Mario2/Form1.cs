using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mario2
{
    public partial class Form1 : Form
    {
        public Game game1;
        public UI MainMenu;
        bool lagButton = false;   //храним нажата ли кнопка   
        int button = 0;
        int i = 0;
        System.Windows.Forms.Timer Time = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer TimeButton = new System.Windows.Forms.Timer();

        public Form1()
        {
            InitializeComponent();
            labelScore.BackColor = Color.Transparent;
            lScore.BackColor = Color.Transparent;
            
            DoubleBuffered = true; //двойная буферизация
            SetStyle(ControlStyles.UserPaint, true);
            Time.Interval = 20; //50
            Time.Tick += new EventHandler(Tick); //обсчет движений на карте
            Time.Start();
            TimeButton.Interval = 100; //как часто обрабатываются нажатия клавиш
            TimeButton.Tick += new EventHandler(TickButton); //при нажатой кнопке вызывает цикличный вызов кнопки

            game1 = new Game(this);
            game1.player.live = 0;
            MainMenu = new UI(this);
            this.KeyPreview = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
        }
        

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == button)
            {
                TimeButton.Stop();
            }
            lagButton = false;
            game1.player.MovePoz(0, 0);   
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            button = e.KeyValue;
            if (!lagButton)
            {
                lagButton = true; //true временно отключает перевызов кнопки
                TimeButton.Start();
                TickButton(sender, e);
            } 
        }
        private void TickButton(object sender, EventArgs e) //таймер зажатой кнопки
        {
            switch (button)
            {
                case 38:  //up
                    game1.player.Move(0, -1);   //game1.player.Move(0, -game1.step);
                    break;
                case 40: //down
                    game1.player.Move(0, 1);
                    break;
                case 37: //left
                    game1.player.Move(-1, 0);
                    break;
                case 39: //right
                    game1.player.Move(1, 0);                    
                    break;
                case 116://F5
                    //game1 = new Game(this);
                    break;
            }
        }
        public void Tick(object Sender, EventArgs e) 
        {
            if ((MainMenu == null) && (game1.player.live < 1))  //возвращаем меню если погибли
            {
                MainMenu = new UI(this);
            }
            game1.Tick(); //такт обсчета физики
            i++;
            label1.Text = i.ToString();
            lScore.Text = game1.player.Score.ToString(); //отобразить изменения счета на экране
            game1.Update();
        }

        private void lScore_Paint(object sender, PaintEventArgs e)
        {
                
        }

        private void labelScore_Click(object sender, EventArgs e)
        {

        }
        public void DisposeUI()
        {
            MainMenu = null;           
        }
    }
    public class UI
    {
        Control MainForm;
        Button Btn;
        public UI(Control Controls)
        {
            MainForm = Controls;
            Btn = new UIButton();
            Btn.Text = "НАЧАТЬ ИГРУ";
            Btn.Font = new Font(Btn.Font.Name, 30);
            Btn.Size = new Size(Controls.Controls.Owner.Width / 2, Controls.Controls.Owner.Height / 2);
            Btn.Location = new Point(Convert.ToInt32(Controls.Controls.Owner.Width / 2 - Btn.Size.Width / 2), Convert.ToInt32(Controls.Controls.Owner.Height / 2 - Btn.Size.Height / 2));
            MainForm.Controls.Add(Btn);
            Btn.Click += new System.EventHandler(UI_Click);
        }
        private void UI_Click(object sender, EventArgs e)
        {
            Form1 f1 = MainForm as Form1;
            f1.game1 = new Game(f1);
            Btn.Dispose();
            f1.DisposeUI();
        }
    }
}
