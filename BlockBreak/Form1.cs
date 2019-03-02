using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;

namespace BlockBreak
{
    public partial class Form1 : Form
    {
        Vector ballPos;
        Vector ballVelocity;
        int ballRadius;
        Rectangle paddlePos;
        List<Rectangle> blockPos;
        List<Rectangle> blockPos2;
        int x = -2;
        double ballSpeed = 5;
        static Random r = new System.Random(10);
        double ballRadian = Math.PI / r.Next(10);
        int paddleLength = 100;

        public Form1()
        {
            InitializeComponent();

            this.ballPos = new Vector(200, 200);
            this.ballVelocity = SpeedToVelocity(ballSpeed, ballRadian);
            this.ballRadius = 10;
            this.paddlePos = new Rectangle(100, this.Height - 50, paddleLength, 5);

            this.blockPos = new List<Rectangle>();
            this.blockPos2 = new List<Rectangle>();
            for (int x = 0; x <= this.Height; x += 100)
            {
                for (int y = 0; y <= 150; y += 40)
                {
                    this.blockPos.Add(new Rectangle(25 + x, y, 80, 25));
                    //x += 100;
                    //this.blockPos2.Add(new Rectangle(25 + x, y, 80, 25));
                }
            }
           

            Timer timer = new Timer();
            timer.Interval = 5;
            timer.Tick += new EventHandler(Update);
            timer.Start();
        }

        Vector SpeedToVelocity(double Speed, double Kakudo)
        {
            Vector vec = new Vector(Speed * Math.Cos(Kakudo), Speed * Math.Sin(Kakudo));
            return vec;
        }

        double DotProduct(Vector a, Vector b)
        {
            return a.X * b.X + a.Y * b.Y; //内積計算
        }

        bool LineVsCircle(Vector p1, Vector p2, Vector center, float radius)
        {
            Vector lineDir = (p2 - p1);                         //パドルの方向ベクトル
            Vector n = new Vector(lineDir.Y, -lineDir.X);       //パドルの法線
            n.Normalize();

            Vector dir1 = center - p1;
            Vector dir2 = center - p2;

            double dist = Math.Abs(DotProduct(dir1, n));
            double a1 = DotProduct(dir1, lineDir);
            double a2 = DotProduct(dir2, lineDir);

            return (a1 * a2 < 0 && dist < radius) ? true : false;
        }

        int BlockVsCircle(Rectangle block, Vector ball)
        {
            if (LineVsCircle(new Vector(block.Left, block.Top),
                new Vector(block.Right, block.Top), ball, ballRadius))
                return 1;
            if (LineVsCircle(new Vector(block.Left, block.Bottom),
                new Vector(block.Right, block.Bottom), ball, ballRadius))
                return 2;
            if (LineVsCircle(new Vector(block.Right, block.Top),
                new Vector(block.Right, block.Bottom), ball, ballRadius))
                return 3;
            if (LineVsCircle(new Vector(block.Left, block.Top),
                new Vector(block.Left, block.Bottom), ball, ballRadius))
                return 4;

            return -1;


        }

        private void Update(object sender, EventArgs e)
        {
            //ボールの移動
            ballPos += ballVelocity;

            //左右の壁でのバウンド
            if (ballPos.X + ballRadius > this.ClientSize.Width || ballPos.X - ballRadius < 0)
            {
                ballRadian += Math.PI - ballRadian * 2;
            }

            //下の壁でのバウンド

            if (ballPos.Y + ballRadius > this.ClientSize.Height)
            {
                ballRadian *= -1;
            }

        　　//上の壁でのバウンド
            if (ballPos.Y - ballRadius < 0)
            {
                ballRadian *= -1;
            }

            //パドルの当たり判定

            if (LineVsCircle(new Vector(this.paddlePos.Left + 30, this.paddlePos.Top),
                new Vector(this.paddlePos.Right - 30, this.paddlePos.Top),
                ballPos, ballRadius))
            {
                ballRadian *= -1;
            }

            if (LineVsCircle(new Vector(this.paddlePos.Left, this.paddlePos.Top),
                new Vector(this.paddlePos.Right-70, this.paddlePos.Top),
                ballPos, ballRadius))
            {
                ballRadian *= -1;
                ballRadian -= Math.PI / 10; //ボールの角度変化
            }

            else if (LineVsCircle(new Vector(this.paddlePos.Left + 70, this.paddlePos.Top),
                new Vector(this.paddlePos.Right, this.paddlePos.Top),
                ballPos, ballRadius))
            {
                ballRadian *= -1;
                ballRadian += Math.PI / 10; //ボールの角度変化
            }
            

            //ブロックとのあたり判定
            for (int i = 0; i < this.blockPos.Count; i++)
            {
                int collision = BlockVsCircle(blockPos[i], ballPos);
                if (collision == 1 || collision == 2)
                {
                    ballRadian *= -1;
                    this.blockPos.Remove(blockPos[i]);          //ブロック消す
                }
                else if (collision == 3 || collision == 4)
                {
                    ballRadian *= -1;
                    this.blockPos.Remove(blockPos[i]);          //ブロック消す
                }
            }

            #region
            //for (int i = 0; i < this.blockPos2.Count; i++)
            //{
            //    int collision = BlockVsCircle(blockPos2[i], ballPos);
            //    if (collision == 1 || collision == 2)
            //    {
            //        ballRadian *= -1;
            //        this.blockPos2.Remove(blockPos2[i]);          //ブロック消す
            //    }
            //    else if (collision == 3 || collision == 4)
            //    {
            //        ballRadian *= -1;
            //        this.blockPos2.Remove(blockPos2[i]);          //ブロック消す
            //    }
            //}
            #endregion


            ballVelocity = SpeedToVelocity(ballSpeed, ballRadian);  //速さ→速度変換

            MovePaddle();   //パドル移動
            

            //再描画
            Invalidate();
        }

        private void Draw(object sender, PaintEventArgs e)
        {
            //e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;    //アンチエイリアス
            SolidBrush blueBrush = new SolidBrush(Color.PaleTurquoise);
            SolidBrush tanBrush = new SolidBrush(Color.Tan);
            //SolidBrush brownBrush = new SolidBrush(Color.SandyBrown);
            SolidBrush grayBrush = new SolidBrush(Color.DimGray);

            float px = (float)this.ballPos.X - ballRadius;
            float py = (float)this.ballPos.Y - ballRadius;

            e.Graphics.FillEllipse(blueBrush, px, py, this.ballRadius * 2, this.ballRadius * 2);        //
            e.Graphics.FillRectangle(grayBrush, paddlePos);

            for (int i = 0; i < this.blockPos.Count; i++)
            {
                e.Graphics.FillRectangle(tanBrush, blockPos[i]);
                //e.Graphics.FillRectangle(brownBrush, blockPos2[i]);
            }
            

        }


        bool flagW = false;
        bool flagA = false;
        bool flagS = false;
        bool flagD = false;

        private void MovePaddle()
        {
            #region
            /*
            if(e.KeyChar == 'a')    // aを押したら左へ
            {
                this.paddlePos.X -= 20;
            }
            else if(e.KeyChar == 'd')   // sを押したら右へ
            {
                this.paddlePos.X += 20;
            }
            
            if(e.KeyChar == 'w')
            {
                this.paddlePos.Y -= 20;
            }
            else if(e.KeyChar == 's')
            {
                this.paddlePos.Y += 20;
            }

            //  上と左同時移動しようとしたけどダメだった(到達不能コードs)
            if (e.KeyChar == 'a')
            {
                this.paddlePos.X -= 20;
                if (e.KeyChar == 'w')
                {
                    this.paddlePos.X -= 20;
                    this.paddlePos.Y -= 20;
                }
                
            }
            */
            #endregion

            if (flagA == true)              //上下左右の動き
            {
                this.paddlePos.X -= 8;
            }
            else if (flagD == true)
            {
                this.paddlePos.X += 8;
            }

            if (flagW == true)
            {
                this.paddlePos.Y -= 8;
            }
            else if (flagS == true)
            {
                this.paddlePos.Y += 8;
            }


            if (flagW == true && flagA == true)             //斜め4方向の動き
            {
                this.paddlePos.X -= 4;
                this.paddlePos.Y -= 4;
            }
            else if (flagW == true && flagD == true)
            {
                this.paddlePos.X += 4;
                this.paddlePos.Y -= 4;
            }

            if (flagS == true && flagA == true)
            {
                this.paddlePos.X -= 8;
                this.paddlePos.Y += 8;
            }
            else if (flagS == true && flagD == true)
            {
                this.paddlePos.X += 8;
                this.paddlePos.Y += 8;
            }

            //パドル制限
            if (this.paddlePos.X < 0)
            {
                this.paddlePos.X = 0;
            }
            if (this.paddlePos.Y < 150)
            {
                this.paddlePos.Y = 150;
            }
            if (this.paddlePos.Right > this.ClientSize.Width)
            {
                Console.WriteLine(this.paddlePos.Right);
                this.paddlePos.X = this.ClientSize.Width - this.paddlePos.Width;
            }
            if (this.paddlePos.Bottom > this.ClientSize.Height)
            {
                this.paddlePos.Y = this.ClientSize.Height - this.paddlePos.Height;
            }

        }

        private void KeyDowned(object sender, KeyEventArgs e)          //キーを押している間flagをtrue
        {
            Console.WriteLine("www");
            switch (e.KeyCode)
            {
                case Keys.W:
                    flagW = true;
                    break;
                case Keys.A:
                    flagA = true;
                    break;
                case Keys.S:
                    flagS = true;
                    break;
                case Keys.D:
                    flagD = true;
                    break;
                default:
                    break;
            }
        }

        private void KeyUpped(object sender, KeyEventArgs e)            //キーを離すとflagをfalse
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    flagW = false;
                    break;
                case Keys.A:
                    flagA = false;
                    break;
                case Keys.S:
                    flagS = false;
                    break;
                case Keys.D:
                    flagD = false;
                    break;
                default:
                    break;
            }
        }
    }
}
