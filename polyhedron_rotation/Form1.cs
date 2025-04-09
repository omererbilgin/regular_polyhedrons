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

namespace polyhedron_rotation
{
    public partial class Form1 : Form
    {


        public delegate void Rotation(ref Point3D pt, double angle_in_degree);

        public struct Point2D  // Stucture of a point coordinates in 2D
        {
            public float x, y;
            public Point2D(float a, float b)
            {
                x = a; y = b;
            }
        }
        public struct Point3D    // Structure of a point coordinates in 3D
        {
            public float x, y, z;
            public Point3D(float a, float b, float c)
            {
                x = a; y = b; z = c;
            }
        }

        const float DEFAULT_POLYHEDRON_EDGE_LEN = 8f; // The length of an edge of all polyhedrons

        // smart to keep it with vertice data
        enum PolyhedronType
        {
            Tetrahedron = 4,
            Cube = 8,
            Octahedron = 6,
            Dodecahedron = 20,
            Icosahedron = 12
        }

        Point3D[] Polyhedron_Point_Generator(float edgeLength, PolyhedronType polyhedron)
        {
            // scaling for iso and dode
            float phi = (float)(1 + Math.Sqrt(5)) / 2; // Golden ratio
            float unit_form = 2 / (float)Math.Sqrt(3);
            float scale = edgeLength / unit_form;

            Console.WriteLine("Scale: " + scale);
            Console.WriteLine("Unit Form: " + unit_form);

            switch (polyhedron)
            {
                case PolyhedronType.Cube:
                    return new Point3D[]
                    {
                new Point3D(0, 0, 0),
                new Point3D(edgeLength, 0, 0),
                new Point3D(edgeLength, edgeLength, 0),
                new Point3D(0, edgeLength, 0),

                new Point3D(0, 0, edgeLength),
                new Point3D(edgeLength, 0, edgeLength),
                new Point3D(edgeLength, edgeLength, edgeLength),
                new Point3D(0, edgeLength, edgeLength)
                    };

                case PolyhedronType.Tetrahedron:
                    return new Point3D[]
                    {
                new Point3D(0, 0, 0),
                new Point3D(edgeLength, 0, 0),
                new Point3D(edgeLength / 2, (float)(Math.Sqrt(3) / 2 * edgeLength), 0),
                new Point3D(edgeLength / 2, (float)(Math.Sqrt(3) / 6 * edgeLength),
                            (float)(Math.Sqrt(6) / 3 * edgeLength))
                    };
                    
                // for simplicity of the centre shifts, I made the centre at origo for the other polyhedrons
                // honestly, i didnt do the others like that just because the professors said take ref from source code exmpl
                case PolyhedronType.Octahedron:
                    return new Point3D[]
                    {
                new Point3D(0, 0, edgeLength),
                new Point3D(edgeLength, 0, 0),
                new Point3D(0, edgeLength, 0),
                new Point3D(-edgeLength, 0, 0),
                new Point3D(0, -edgeLength, 0),
                new Point3D(0, 0, -edgeLength)
                    };
                    
                case PolyhedronType.Dodecahedron:

            return new Point3D[]
            {
                    // corners
                    new Point3D( 1*scale,  1*scale,  1*scale),
                    new Point3D( 1*scale,  1*scale, -1*scale),
                    new Point3D( 1*scale, -1*scale,  1*scale),
                    new Point3D( 1*scale, -1*scale, -1*scale),
                    new Point3D(-1*scale,  1*scale,  1*scale),
                    new Point3D(-1*scale,  1*scale, -1*scale),
                    new Point3D(-1*scale, -1*scale,  1*scale),
                    new Point3D(-1*scale, -1*scale, -1*scale),

                    // face centered
                    new Point3D(0,             (1f/phi)*scale,  phi*scale),
                    new Point3D(0,            -(1f/phi)*scale,  phi*scale),
                    new Point3D(0,             (1f/phi)*scale, -phi*scale),
                    new Point3D(0,            -(1f/phi)*scale, -phi*scale),
                    new Point3D((1f/phi)*scale,   phi*scale, 0),
                    new Point3D(-(1f/phi)*scale,  phi*scale, 0),
                    new Point3D((1f/phi)*scale,  -phi*scale, 0),
                    new Point3D(-(1f/phi)*scale, -phi*scale, 0),
                    new Point3D(phi*scale,  0,   (1f/phi)*scale),
                    new Point3D(phi*scale,  0,  -(1f/phi)*scale),
                    new Point3D(-phi*scale, 0,   (1f/phi)*scale),
                    new Point3D(-phi*scale, 0,  -(1f/phi)*scale)
            };
                    
                case PolyhedronType.Icosahedron:

            return new Point3D[]
            {
new Point3D(-scale,  phi * scale, 0),
new Point3D( scale,  phi * scale, 0),
new Point3D(-scale, -phi * scale, 0),
new Point3D( scale, -phi * scale, 0),
new Point3D(0, -scale,  phi * scale),
new Point3D(0,  scale,  phi * scale),
new Point3D(0, -scale, -phi * scale),
new Point3D(0,  scale, -phi * scale),
new Point3D( phi * scale, 0, -scale),
new Point3D( phi * scale, 0,  scale),
new Point3D(-phi * scale, 0, -scale),
new Point3D(-phi * scale, 0,  scale)
            };
                    
                default:
                    throw new ArgumentException("Error: wrong polyhedron type called");
            }
        }

        // indices
        int[] Polyhedron_Indices(PolyhedronType polyhedron)
        {
            switch (polyhedron)
            {
                case PolyhedronType.Tetrahedron:
                    return new int[] { 0, 1, 1, 2, 2, 0, 0, 3, 1, 3, 2, 3 };

                case PolyhedronType.Cube:
                    return new int[]
                    {
                0, 1, 1, 2, 2, 3, 3, 0,
                4, 5, 5, 6, 6, 7, 7, 4,
                0, 4, 1, 5, 2, 6, 3, 7
                    };

                case PolyhedronType.Octahedron:
                    return new int[]
                    {
                0, 1, 0, 2, 0, 3, 0, 4,
                1, 2, 1, 5, 2, 3, 3, 4,
                4, 5, 5, 2, 5, 3, 4, 1
                    };

                case PolyhedronType.Dodecahedron:
                    return new int[]
                    {
                0, 8,   0,12,  0,16,
                1,10,  1,12,  1,17,
                2, 9,   2,14,  2,16,
                3,11,  3,14,  3,17,
                4, 8,   4,13,  4,18,
                5,10,  5,13,  5,19,
                6, 9,   6,15,  6,18,
                7,11,  7,15,  7,19,

                // connect short edges
                8,  9,
                10, 11,
                12, 13,
                14, 15,
                16, 17,
                18, 19
                    };


                case PolyhedronType.Icosahedron:
                    return new int[]
                    {
0, 1,
0, 5,
0, 7,
0,10,
0,11,

1,5,
1,7,
1,8,
1,9,

2,3,
2,4,
2,6,
2,10,
2,11,

3,4,
3,6,
3,8,
3,9,

4,5,
4,9,
4,11,

5,9,
5,11,

6,7,
6,8,
6,10,

7,8,
7,10,

8,9,

2,3,
2,4
                    };

                default:
                    throw new ArgumentException("Error: wrong polyhedron type called at determination of indices");
            }
        }

        void Shift(ref Point3D point, Point3D shiftVector)
        {
            point.x += shiftVector.x;
            point.y += shiftVector.y;
            point.z += shiftVector.z;
        }
        Point3D[] Polyhedron_Centre_Generator(float edgeLength, PolyhedronType polyhedron)
        {
            Point3D[] polyhedron3D_Origo = new Point3D[0];

            switch (polyhedron)
            {
                case PolyhedronType.Cube:
                    
                    polyhedron3D_Origo = Polyhedron_Point_Generator(edgeLength, PolyhedronType.Cube);

                    // cube centre calc
                    Point3D cube_center = new Point3D(edgeLength / 2, edgeLength / 2, edgeLength / 2);

                    // set cube centre to 0,0,0
                    Point3D cube_center_distance = new Point3D(-cube_center.x, -cube_center.y, -cube_center.z);
                    for (int i = 0; i < polyhedron3D_Origo.Length; i++)
                    {
                        Shift(ref polyhedron3D_Origo[i], cube_center_distance);
                    }
                    break;

                case PolyhedronType.Tetrahedron:
                    
                    polyhedron3D_Origo = Polyhedron_Point_Generator(edgeLength, PolyhedronType.Tetrahedron);

                    // tetrahedron centre calc
                    Point3D tetrahedron_center = new Point3D(
                        edgeLength / 2f,
                        (float)(Math.Sqrt(3) / 6) * edgeLength,
                        (float)(Math.Sqrt(6) / 12) * edgeLength
                    );

                    // shift to origo
                    Point3D tetrahedron_center_distance = new Point3D(-tetrahedron_center.x, -tetrahedron_center.y, -tetrahedron_center.z);
                    for (int i = 0; i < polyhedron3D_Origo.Length; i++)
                    {
                        Shift(ref polyhedron3D_Origo[i], tetrahedron_center_distance);
                    }
                    break;

                case PolyhedronType.Octahedron:

                    polyhedron3D_Origo = Polyhedron_Point_Generator(edgeLength, PolyhedronType.Octahedron);

                    break;

                case PolyhedronType.Dodecahedron:

                    polyhedron3D_Origo = Polyhedron_Point_Generator(edgeLength, PolyhedronType.Dodecahedron);

                    break;

                case PolyhedronType.Icosahedron:

                    polyhedron3D_Origo = Polyhedron_Point_Generator(edgeLength, PolyhedronType.Icosahedron);

                    break;

                default:
                    throw new ArgumentException("Error: wrong polyhedron type called for centre gen");
            }

            return polyhedron3D_Origo;
        
        }

        private void InitializeNumericUpDown() 
        {
            numericUpDown1.Minimum = 1;   // Set minimum to a float value
            numericUpDown1.Maximum = 15;    // Set maximum to a float value
            numericUpDown1.DecimalPlaces = 2; // Set the decimal precision to 2 digits
        }




        public Form1()
        {
            InitializeComponent();
            InitializeNumericUpDown();
            InitializeTrackBars();

            numericUpDown1.Value = (int)DEFAULT_POLYHEDRON_EDGE_LEN;

        }

        private void InitializeTrackBars()
        {

            trackBar1.Value = (int)DEFAULT_POLYHEDRON_EDGE_LEN;
            trackBar1.Maximum = 150;
            trackBar1.Minimum = 1;

            // TrackBar for X-axis rotation
            TrackBar trackBarX = new TrackBar();
            trackBarX.Minimum = -30;
            trackBarX.Maximum = 30;
            trackBarX.Value = 0;
            trackBarX.TickFrequency = 5;
            trackBarX.Location = new Point(10, 10);
            trackBarX.Scroll += (sender, e) => Refresh();
            trackBarX.Name = "trackBarX";
            Controls.Add(trackBarX);

            // TrackBar for Y-axis rotation
            TrackBar trackBarY = new TrackBar();
            trackBarY.Minimum = -30;
            trackBarY.Maximum = 30;
            trackBarY.Value = 0;
            trackBarY.TickFrequency = 5;
            trackBarY.Location = new Point(10, 50);
            trackBarY.Scroll += (sender, e) => Refresh();
            trackBarY.Name = "trackBarY";

            Controls.Add(trackBarY);

            // TrackBar for Z-axis rotation
            TrackBar trackBarZ = new TrackBar();
            trackBarZ.Minimum = -30;
            trackBarZ.Maximum = 30;
            trackBarZ.Value = 0;
            trackBarZ.TickFrequency = 5;
            trackBarZ.Location = new Point(10, 90);
            trackBarZ.Scroll += (sender, e) => Refresh();
            trackBarZ.Name = "trackBarZ";

            Controls.Add(trackBarZ);

            // TrackBar for Timer Interval
            TrackBar trackBarInterval = new TrackBar();
            trackBarInterval.Minimum = 1;
            trackBarInterval.Maximum = 2000;
            trackBarInterval.Value = 100;
            trackBarInterval.TickFrequency = 100;
            trackBarInterval.Location = new Point(10, 130);
            trackBarInterval.Scroll += (sender, e) => timer1.Interval = trackBarInterval.Value;
            trackBarInterval.Name = "trackBarInterval";

            Controls.Add(trackBarInterval);
        }

        public Point2D Axonometric(Point3D pt3)   // Isometric projection, user defined function
        {
            Point2D pt2;
            pt2.x = 20 * (-0.35F * pt3.x + 1 * pt3.y + 0 * pt3.z);
            pt2.y = 20 * (-0.35F * pt3.x + 0 * pt3.y + 1 * pt3.z);
            return pt2;
        }
        /*
        private void label1_Click(object sender, EventArgs e)
        {

        }
        */
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Interval = 100;
            timer1.Enabled = !timer1.Enabled; // Start or stop timer alternatively
        }

        PolyhedronType GetPolyhedron() {

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    return 0;
                case 1:
                    return PolyhedronType.Tetrahedron;
                case 2:
                    return PolyhedronType.Cube;
                case 3:
                    return PolyhedronType.Octahedron;
                case 4:
                    return PolyhedronType.Dodecahedron;
                case 5:
                    return PolyhedronType.Icosahedron;
                default:
                    throw new Exception("WTF");
            }

        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            PolyhedronType polyhedron = GetPolyhedron();
            if (polyhedron == 0)
                return;

            TrackBar trackBarX = (TrackBar)Controls.Find("trackBarX", true)[0];
            TrackBar trackBarY = (TrackBar)Controls.Find("trackBarY", true)[0];
            TrackBar trackBarZ = (TrackBar)Controls.Find("trackBarZ", true)[0];

            double angleX = trackBarX.Value;
            double angleY = trackBarY.Value;
            double angleZ = trackBarZ.Value;

            // this should be globally defined instead of here but Im lazy
            Point3D[] origo3D = Polyhedron_Centre_Generator((float)numericUpDown1.Value, polyhedron);
            Point2D[] coord2D = new Point2D[(int)polyhedron];

            for (int i = 0; i < (int)polyhedron; i++)
            {
                Point3D pt = origo3D[i];
                RotateX(ref pt, angleX);
                RotateY(ref pt, angleY);
                RotateZ(ref pt, angleZ);
                coord2D[i] = Axonometric(pt);
            }
            e.Graphics.TranslateTransform(ClientSize.Width / 1.5f, ClientSize.Height / 2);

            int[] indices = Polyhedron_Indices(polyhedron);

            for (int i = 0; i < indices.Length; i += 2)  // draw lines between the appropriate vertex points in 2D
            {
                e.Graphics.DrawLine(Pens.GreenYellow, coord2D[indices[i]].x, coord2D[indices[i]].y,
                                              coord2D[indices[i + 1]].x, coord2D[indices[i + 1]].y);
            }


            if (checkBox1.Checked)
            {
                //Debug points
                for (int i = 0; i < coord2D.Length; i++)
                {
                    e.Graphics.FillEllipse(Pens.Red.Brush, coord2D[i].x - 2, coord2D[i].y - 2, 4, 4);
                }
            }

        }


        public static void RotateX(ref Point3D pt, double angle_in_degree)
        {
            //Euler matrix formula 3D point to turn image with x radian around the x-axis
            //[ 1    0        0   ]
            //[ 0   cos(x)  sin(x)]
            //[ 0   -sin(x) cos(x)]

            double radian = (Math.PI * angle_in_degree) / 180.0f;
            double cosx = Math.Cos(radian);
            double sinx = Math.Sin(radian);

            double yy = (pt.y * cosx) + (pt.z * sinx);
            double zz = (pt.y * -sinx) + (pt.z * cosx);

            pt.y = (float)yy;
            pt.z = (float)zz;
        }

        public static void RotateY(ref Point3D pt, double angle_in_degree)
        {
            //y-tengely
            //[ cos(x)   0    sin(x)]
            //[   0      1      0   ]
            //[-sin(x)   0    cos(x)]

            double radian = (Math.PI * angle_in_degree) / 180.0f;
            double cosx = Math.Cos(radian);
            double sinx = Math.Sin(radian);

            double xx = (pt.x * cosx) + (pt.z * sinx);
            double zz = (pt.x * -sinx) + (pt.z * cosx);

            pt.x = (float)xx;
            pt.z = (float)zz;
        }

        public static void RotateZ(ref Point3D pt, double angle_in_degree)
        {
            //z-axis
            //[ cos(x)  sin(x) 0]
            //[ -sin(x) cos(x) 0]
            //[    0     0     1]

            double radian = (Math.PI * angle_in_degree) / 180.0f;
            double cosx = Math.Cos(radian);
            double sinx = Math.Sin(radian);

            double xx = (pt.x * cosx) + (pt.y * sinx);
            double yy = (pt.x * -sinx) + (pt.y * cosx);

            pt.x = (float)xx;
            pt.y = (float)yy;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            Random rnd = new Random();

            TrackBar trackBarX = (TrackBar)Controls.Find("trackBarX", true)[0];
            TrackBar trackBarY = (TrackBar)Controls.Find("trackBarY", true)[0];
            TrackBar trackBarZ = (TrackBar)Controls.Find("trackBarZ", true)[0];

            switch (rnd.Next(3))
            { 
                case 0: trackBarX.Value = rnd.Next(-30,30); break;
                case 1: trackBarY.Value = rnd.Next(-30, 30); break;
                case 2: trackBarZ.Value = rnd.Next(-30, 30); break;
                default: throw new Exception("Rnd failed");
            }

            Refresh(); // Clean drawing to automatically generate a Paint event

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1 == null)
                return;
            else
                trackBar1.Value = (int)numericUpDown1.Value * 10;
            
            Refresh();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            
            if (trackBar1.Value < 10)
                numericUpDown1.Value = 1;
            else
                numericUpDown1.Value = (decimal)((float)trackBar1.Value / 10);

            Refresh();

        }


        private void button2_Click(object sender, EventArgs e)
        {
            TrackBar trackBarX = (TrackBar)Controls.Find("trackBarX", true)[0];
            TrackBar trackBarY = (TrackBar)Controls.Find("trackBarY", true)[0];
            TrackBar trackBarZ = (TrackBar)Controls.Find("trackBarZ", true)[0];

            trackBarX.Value = 0;
            trackBarY.Value = 0;
            trackBarZ.Value = 0;
            Refresh();
        }
    }
}
