using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Collections;

namespace WhereIsVoyager {
    public partial class FrmMain : Form {
        [DllImport("user32.dll")]//*********************拖动无窗体的控件
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        public double distance = 21172362181; // 截至2018年1月2日止，旅行者1号正处于离太阳211亿公里的距离
        public bool display_hundred_million = false;
        public int x = 1;
        public int y = 1;
        public class Star {
            public int x;
            public int y;
            public int speed;
            public Color color;
            public int r;
            public void Move()
            {
                x -= speed;
                if (x<0) { x = 376; }
            }
        }
        public ArrayList stars;

        public FrmMain()
        {
            InitializeComponent();
        }

        public void EasyMove()
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void FrmMain_MouseDown(object sender, MouseEventArgs e)
        {
            EasyMove();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Random rnd = new Random();
            distance += rnd.Next(1, 2);

            DrawStar();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            // 截至2018年1月2日止，旅行者1号正处于离太阳211亿公里的距离
            // 旅行者1号相对速度是17.062公里 / 秒，或61,452公里 / 每小时（约38,185哩 / 每小时）
            // 根据当前日期更新当前距离
            DateTime DateTime2 = DateTime.Now;
            DateTime DateTime1 = Convert.ToDateTime("2018-01-2 00:00:00");
            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();  
            distance += ts.TotalSeconds * 17.062 ;

            stars = new ArrayList(333);
            Random rnd = new Random();
            for (int i=0;i<333;i++) {
                Star p = new Star();
                p.x = rnd.Next(0, 375);
                p.y = rnd.Next(0, 667);
                p.r = rnd.Next(1, 2);
                if (p.r == 1) {
                    p.speed = rnd.Next(1, 9);
                    p.color = Color.Cyan;
                } else {
                    p.color = Color.White;
                    p.speed = rnd.Next(3, 6);
                }
                stars.Add((object)p);
            }

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            panel1.BackColor = Color.FromArgb(100, 0, 0, 100);
        }

        private void DrawStar()
        {
            // 离屏
            Bitmap BackBuffer = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(BackBuffer);
            g.Clear(this.BackColor);

            for (int i=0;i<stars.Count;i++) {
                Star p = (Star)stars[i];
                g.DrawEllipse(Pens.White, p.x, p.y, p.r, p.r);
                p.Move();
                stars[i] = p;
            }
            Font drawFont = new Font("微软雅黑", 16);
            SolidBrush drawBrush = new SolidBrush(Color.White);
            g.DrawString("旅行者1号距离地球", drawFont, drawBrush, 80, 40);

            Font drawFont2 = new Font("微软雅黑", 14);
            if (display_hundred_million) {
                g.DrawString((distance/100000000).ToString("0.00000000") + " 亿公里", drawFont2, drawBrush, 80, 80);
            } else {
                g.DrawString(distance.ToString("N0") + " 公里", drawFont2, drawBrush, 80, 80);
            }

            Bitmap bmOld = new Bitmap(pictureBox1.Image);
            g.DrawImage(bmOld, 24, 364, 300, 200);

            // 绘屏
            Graphics sg = this.CreateGraphics();
            //sg.Clear(Color.Black);
            sg.DrawImage(BackBuffer, 0, 0);
        }

        private void FrmMain_Paint(object sender, PaintEventArgs e)
        {
            DrawStar();
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            this.Close();
            this.Dispose();
        }

        private void btnChgDisp_Click(object sender, EventArgs e)
        {
            if (display_hundred_million) {
                btnChgDisp.Text = "显示为公里";
            } else {
                btnChgDisp.Text = "显示为亿公里";
            }
            display_hundred_million = !display_hundred_million;
            panel1.Visible = false;
        }
    }
}
