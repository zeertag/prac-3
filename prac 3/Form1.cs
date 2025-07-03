using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace prac_3
{
    public partial class Form1 : Form
    {
        private List<(int X, int Y, int Depth)> nodes = new List<(int X, int Y, int Depth)>();
        public Form1()
        {
            InitializeComponent();
            pictureBox2.MouseClick += pictureBox2_MouseClick;
        }

        private void DrawTree(Graphics g, int x, int y, int angle, int length, int n, int stop)
        {
            var state = g.Save();
            g.TranslateTransform(x, y);
            g.RotateTransform(angle);

            int r = Math.Abs((255 - n * 30) % 256);
            int gr = Math.Abs((255 - n * 80) % 256);
            int b = Math.Abs((255 - n * 80) % 256);

            //рисуем большой квадрат
            g.FillRectangle(new SolidBrush(Color.FromArgb(r, gr, b)), -length / 2, -length, length, length);

            //рисуем 2 малых
            int nextX1 = (int)(-length / 4);
            int nextX2 = (int)(length / 4);
            int nextY = (int)(-length * 5 / 4);

            length = (int)(length / 1.4f);


            //1
            var state1 = g.Save();
            g.TranslateTransform(nextX1, nextY);
            g.RotateTransform(-45);
            g.FillRectangle(new SolidBrush(Color.FromArgb(r, gr, b)), -length / 2, -length, length, length);
            g.Restore(state1);

            //2
            var state2 = g.Save();
            g.TranslateTransform(nextX2, nextY);
            g.RotateTransform(45);
            g.FillRectangle(new SolidBrush(Color.FromArgb(r, gr, b)), -length / 2, -length, length, length);
            g.Restore(state2);



            if (n < stop)
            {
                int newX1 = (int)(-length / 4);
                int newX2 = (int)(length / 4);
                int newX3 = (int)(-length / 4);
                int newX4 = (int)(length / 4);
                int newY = (int)(-length * 5 / 4);

                var state3 = g.Save();
                g.TranslateTransform(nextX2, nextY);
                g.RotateTransform(45);

                DrawTree(g, newX1, newY, -45, (int)(length / 1.4f), n + 1, stop);
                DrawTree(g, newX2, newY, 45, (int)(length / 1.4f), n + 1, stop);
                g.Restore(state3);

                var state4 = g.Save();
                g.TranslateTransform(nextX1, nextY);
                g.RotateTransform(-45);
                DrawTree(g, newX3, newY, -45, (int)(length / 1.4f), n + 1, stop);
                DrawTree(g, newX4, newY, 45, (int)(length / 1.4f), n + 1, stop);
                g.Restore(state4);
            }

            g.Restore(state);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            double[] coefs = { 0.2, 0.5, 0.8 };

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);

                for (int i = 1; i < numericUpDown1.Value + 1; i++)
                {
                    if (i < 4)
                        DrawTree(g, (int)(pictureBox1.Width * coefs[(i - 1) % 3]), pictureBox1.Height / 2 - 10, 0, 35, 1, i);
                    else
                        DrawTree(g, (int)(pictureBox1.Width * coefs[(i - 1) % 3]), pictureBox1.Height - 50, 0, 35, 1, i);
                }
            }

            pictureBox1.Image = bmp;
        }


        public class TreeNodeInfo
        {
            public int X, Y;
            public int Depth;
        }

        private void DrawScelet(Graphics g, int x, int y, int n, int stop, int totalWidth)
        {
            int dy = 60;
            int radius = 6;

            nodes.Add((x, y, n));

            g.FillEllipse(Brushes.LightGreen, x - radius, y - radius, 2 * radius, 2 * radius);
            g.DrawEllipse(Pens.Black, x - radius, y - radius, 2 * radius, 2 * radius);

            if (n >= stop - 1) return;

            int numChildren = 4;
            int level = n + 1;
            int levelNodes = (int)Math.Pow(numChildren, level);

            int spacing = totalWidth / levelNodes;

            int baseX = x - ((spacing * (numChildren - 1)) / 2);
            int childY = y + dy;

            for (int i = 0; i < numChildren; i++)
            {
                int childX = baseX + i * spacing;

                g.DrawLine(Pens.Black, x, y + radius, childX, childY - radius);

                DrawScelet(g, childX, childY, n + 1, stop, totalWidth);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox2.Width, pictureBox2.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.White);

                int depth = 6;
                int margin = 20;
                int totalWidth = pictureBox2.Width - 2 * margin;

                nodes.Clear();
                DrawScelet(g, pictureBox2.Width / 2, 30, 0, depth, totalWidth);
            }

            pictureBox2.Image = bmp;
        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            int radius = 6;
            foreach (var node in nodes)
            {
                int dx = e.X - node.X;
                int dy = e.Y - node.Y;
                if (dx * dx + dy * dy <= radius * radius)
                {
                    MessageBox.Show($"Уровень узла: {node.Depth + 1}");
                    numericUpDown1.Value = node.Depth + 1;
                    return;
                }
            }
        }
    }
}
