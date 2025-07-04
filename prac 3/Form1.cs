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


        public class SceletNode
        {
            public int X, Y;
            public int Depth;
            public List<SceletNode> Children = new List<SceletNode>();
        }

        private SceletNode root;

        private void BuildTree(SceletNode node, int maxDepth, int totalWidth, int numChildren, int dy)
        {
            if (node.Depth >= maxDepth - 1)
                return;

            int level = node.Depth + 1;
            int levelNodes = (int)Math.Pow(numChildren, level);
            int spacing = totalWidth / levelNodes;

            int baseX = node.X - ((spacing * (numChildren - 1)) / 2);
            int childY = node.Y + dy;

            for (int i = 0; i < numChildren; i++)
            {
                int childX = baseX + i * spacing;

                var child = new SceletNode
                {
                    X = childX,
                    Y = childY,
                    Depth = level
                };

                node.Children.Add(child);
                BuildTree(child, maxDepth, totalWidth, numChildren, dy);
            }
        }

        private void DrawTreeFromNode(Graphics g, SceletNode node)
        {
            int radius = 6;

            int r = Math.Abs((255 - (node.Depth + 1) * 30) % 256);
            int gr = Math.Abs((255 - (node.Depth + 1) * 80) % 256);
            int b = Math.Abs((255 - (node.Depth + 1) * 80) % 256);

            g.FillEllipse(new SolidBrush(Color.FromArgb(r, gr, b)), node.X - radius, node.Y - radius, 2 * radius, 2 * radius);
            g.DrawEllipse(Pens.Black, node.X - radius, node.Y - radius, 2 * radius, 2 * radius);

            nodes.Add((node.X, node.Y, node.Depth));

            foreach (var child in node.Children)
            {
                g.DrawLine(Pens.Black, node.X, node.Y + radius, child.X, child.Y - radius);
                DrawTreeFromNode(g, child);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox2.Width, pictureBox2.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.White);

                int depth = 6;
                int margin = 20;
                int totalWidth = pictureBox2.Width - 2 * margin;
                int startX = pictureBox2.Width / 2;
                int startY = 30;
                int dy = 60;
                int numChildren = 4;

                // Шаг 1: Строим дерево с координатами
                root = new SceletNode { X = startX, Y = startY, Depth = 0 };
                BuildTree(root, depth, totalWidth, numChildren, dy);

                // Шаг 2: Отрисовываем дерево
                nodes.Clear();
                DrawTreeFromNode(g, root);
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
