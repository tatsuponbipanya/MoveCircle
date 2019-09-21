using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoveCircle
{
    public partial class FormBallGame : Form
    {
        //クラス共通の変数
        private Bitmap canvas;
        private string[] kanjis;
        private Brush[] brushes;
        private Ball[] balls;           //ボールを管理

        private string fontName = "HG教科書体";
        private string correctText = "蜀";
        private string mistakeText = "魏";
        private string circleText = "○";

        private double nowTime = 0;
        private int ballCount = 5;      //ボールの数
        private int randomResult = 0;

        public FormBallGame()
        {
            InitializeComponent();
        }

        //以下イベントハンドラ

        private void FormBallGame_Load(object sender, EventArgs e)
        {
            InitGraphics();
            SetStartPosition();
        }

        //正解の文字を表示させるサブルーチン
        private void DrowMainPictureBox()
        {
            //描画先のImageオブジェクトを生成
            Bitmap canvas = new Bitmap(mainPictureBox.Width, mainPictureBox.Height);
            using (Graphics g = Graphics.FromImage(canvas))
            {
                //背景に引数で指定した文字列を描写
                g.DrawString("蜀", new Font("HG教科書体", mainPictureBox.Height - mainPictureBox.Height / 4),
                    Brushes.Gray, 0, 0, new StringFormat());

                //mainPictureに表示
                mainPictureBox.Image = canvas;
            }
        }


        //再スタートボタン
        private void restartButton_Click(object sender, EventArgs e)
        {
            InitGraphics();
            SetStartPosition();
        }

        //上のピクチャーボックス
        private void selectPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            //再スタートボタンが操作可能な場合は何もせずに終了
            if(restartButton.Enabled)
            {
                return;
            }

            //押されたx座標で正解判定
            if(e.Button == MouseButtons.Left)
            {
                int selectCircle = e.X / selectPictureBox.Height;

                if (randomResult == selectCircle)
                {
                    timer1.Stop();
                    DrowMainPictureBox(Brushes.Red, circleText, true);
                    restartButton.Enabled = true;       //再スタートボタンを操作可能にする
                }
                else
                {
                    DrowMainPictureBox(Brushes.Red, correctText, false);
                    //移動の割合を減少させる
                    for(int i = 0; i < ballCount; i++)
                    {
                        balls[i].pitch = balls[i].pitch - balls[i].pitch / 2;
                    }
                    nowTime = nowTime + 10;     //ペナルティ
                }

            }
        


        }

        //下のピクチャーボックス
        private void mainPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if(restartButton.Enabled)
            {
                return;
            }

            SetBalls(e.X, e.Y);     //マウスをクリックした位置にボールをセット
        }

        //タイマー
        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < ballCount; i++)
            {
                balls[i].Move();
            }

            nowTime = nowTime + 0.02;
            textTimer.Text = nowTime.ToString("0.00");
        }

        //以下独自のメソッド


        //上のボックスに円を描くサブルーチン
        private void DrawCircleSelectPictureBox()
        {
            int height = selectPictureBox.Height;       //高さ
            int width = selectPictureBox.Width;         //幅

            Bitmap selectCanvas = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(selectCanvas))
            {
                //g.FillEllipse(Brushes.LightBlue, 0, 0, height, height);

                for (int i = 0; i < ballCount; i++)
                {
                    g.FillEllipse(brushes[i], i * height, 0, height, height);
                }

                selectPictureBox.Image = selectCanvas;
            }
        }

        //下のボックスに円を描くサブルーチン
        private void DrowMainPictureBox(Brush color, string text, bool trueFlag)
        {
            int height = mainPictureBox.Height;    //高さ
            int width = mainPictureBox.Width;       //幅

            //描画先とするImageオブジェクトを作成
            if (canvas == null)
            {
                canvas = new Bitmap(width, height);
            }

            using (Graphics g = Graphics.FromImage(canvas))
            {
                //正解用の背景色にする
                if (trueFlag)
                {
                    g.FillRectangle(Brushes.LightPink, 0, 0, width, height);
                }
                else
                {
                    g.FillRectangle(Brushes.White, 0, 0, width, height);
                }

                //背景に文字列を描画
                g.DrawString(text, new Font(fontName, height - height / 4),
                    color, 0, 0, new StringFormat());

                //mainPictureBoxに表示
                mainPictureBox.Image = canvas;
            }
        }

        //配列の初期化、画面の初期設定を行う
        private void InitGraphics()
        {
            brushes = new Brush[ballCount];
            kanjis = new string[ballCount];
            balls = new Ball[ballCount];

            //ブラシの色の設定
            brushes[0] = Brushes.LightPink;
            brushes[1] = Brushes.LightBlue;
            brushes[2] = Brushes.LightGray;
            brushes[3] = Brushes.LightCoral;
            brushes[4] = Brushes.LightGreen;

            //上のイメージオブジェクト
            DrawCircleSelectPictureBox();


            //下のイメージオブジェクト
            DrowMainPictureBox(Brushes.Gray, correctText, false);

            restartButton.Enabled = false;      //再スタートボタンを操作出来ないようにする
            textHunt.Text = correctText;
        }

        //ボールのインスタンス作成とランダムな位置にボールを描く
        private void SetStartPosition()
        {
            //漢字の設定
            for(int i = 0; i < ballCount; i++)
            {
                kanjis[i] = mistakeText;
            }

            randomResult = new Random().Next(ballCount);
            kanjis[randomResult] = correctText;

            //ボールのインスタンス作成
            for(int i = 0; i < ballCount; i++)
            {
                balls[i] = new Ball(mainPictureBox, canvas, brushes[i], kanjis[i]);
            }

            //ランダムな位置にボールを置く
            int rndXMax = mainPictureBox.Width;
            int rndYMax = mainPictureBox.Height;
            SetBalls(new Random().Next(rndXMax), new Random().Next(rndYMax));

            //タイマーをスタートさせる
            nowTime = 0;
            timer1.Start();
        }
        
        //引数の位置情報を利用してランダムにボールを描く
        private void SetBalls(int x, int y)
        {
            int rndXMax = mainPictureBox.Width;
            int rndYMax = mainPictureBox.Height;
            int rndX;
            int rndY;

            for(int i = 0; i < ballCount; i++)
            {
                rndX = new Random(i * x).Next(rndXMax);
                rndY = new Random(i * y).Next(rndYMax);
                balls[i].DeleteCircle();
                balls[i].PutCircle(rndX, rndY);
            }



        }



    }
}
