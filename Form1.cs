using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace Ghilescu_Dumitru_3131B
{

    public partial class Form1 : Form
    {
        //control states for camera
        private int vp_X, vp_Y, vp_Z; //viewport coordinates

        private Point mouse; //mouse position
        private float cam; //camera state

        //control states for mouse
        private bool scm_2D, scm_3D, scm_Down; //mouse control flags

        //control states for coordinate axis
        private bool sca; //coordinate axis control flag

        //control states for lighting
        private bool lightON; //flag for main light source
        private bool lightON_0; //flag for the first light source

        //modification for lab 9 - point 3
        //control state for lighting the second light source
        private bool lightON_1; //flag for the second light source

        //control states for 3D objects
        private string statusCube; //status of a 3D object

        //storage for vertices and vertex lists
        private int[,] arrVertex = new int[50, 3]; //matrix to store vertices (X, Y, Z)
        private int nVertex; //number of vertices

        private int[] arrQuadsList = new int[100]; //vertex list for constructing a cube using quads
        private int nQuadsList; //number of quads

        private int[] arrTrianglesList = new int[100]; //vertex list for constructing a cube using triangles
        private int nTrianglesList; //number of triangles

        //files for manipulating vertices
        private string fileVertex = @"./../../vList.txt"; //vertex file path
        private string fileQList = @"./../../qVList.txt"; //quad list file path
        private string fileTList = @"./../../tVList.txt"; //triangle list file path
        private bool statusFiles; //status of file operations

        //ambient light values template 0
        private float[] valuesAmbientTemplate0 = new float[] { 0.1f, 0.1f, 0.1f, 1.0f };
        private float[] valuesDiffuseTemplate0 = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        private float[] valuesSpecularTemplate0 = new float[] { 0.1f, 0.1f, 0.1f, 1.0f };
        private float[] valuesPositionTemplate0 = new float[] { 0.0f, 0.0f, 5.0f, 1.0f };

        //ambient light values template 1, for the second light source
        private float[] valuesAmbientTemplate1 = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
        private float[] valuesDiffuseTemplate1 = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        private float[] valuesSpecularTemplate1 = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        private float[] valuesPositionTemplate1 = new float[] { 1.0f, 1.0f, 1.0f, 0.0f };

        //arrays to hold light values
        private float[] valuesAmbient0 = new float[4];
        private float[] valuesDiffuse0 = new float[4];
        private float[] valuesSpecular0 = new float[4];
        private float[] valuesPosition0 = new float[4];

        private float[] valuesAmbient1 = new float[4];
        private float[] valuesDiffuse1 = new float[4];
        private float[] valuesSpecular1 = new float[4];
        private float[] valuesPosition1 = new float[4];

        private Axis xyz; //instance of the Axis class
        private Cube cube; //instance of the Cube class

        //similar to the OnLoad() method - loads resources
        public Form1()
        {
            xyz = new Axis(); //initialize Axis object
            cube = new Cube(); //initialize Cube object

            InitializeComponent(); //initialize form components
        }

        //method triggered when the form is loaded
        private void Form1_Load(object sender, EventArgs e)
        {
            SetupValues(); //set initial values
            SetupWindowGUI(); //setup Window GUI
        }

        //set initial values
        private void SetupValues()
        {
            vp_X = 100;
            vp_Y = 100;
            vp_Z = 50;

            cam = 1.04f;

            setLight0Values(); //set values for light 0
            setLight1Values(); //set values for light 1

            //set numeric values for eye coordinates
            numericXeye.Value = vp_X;
            numericYeye.Value = vp_Y;
            numericZeye.Value = vp_Z;
        }

        //method to set up the graphical user interface of the window
        private void SetupWindowGUI()
        {
            //set mouse controls initially to false
            setControlMouse2D(false);
            setControlMouse3D(false);

            //set camera depth value from cam variable
            numericCameraDepth.Value = (int)cam;

            //enable control for the axis
            setControlAxe(true);

            //set initial statuses for different components
            setCubeStatus("OFF"); //cube status
            setIlluminationStatus(false); //illumination status
            setSource0Status(false); //status for light source 0
            setSource1Status(false); //status for light source 1

            //set default values for light source 0
            setTrackLigh0Default();
            setColorAmbientLigh0Default();
            setColorDifuseLigh0Default();
            setColorSpecularLigh0Default();

            //set default values for light source 1
            setLight1Default();
            setColorAmbientLigh1Default();
            setColorDifuseLigh1Default();
            setColorSpecularLigh1Default();
        }

        //method to load vertex coordinates and lists for 3D object creation
        private void loadVertex()
        {
            try
            {
                //check if the vertex file exists
                StreamReader fileReader = new StreamReader((fileVertex));

                //read the number of vertices from the file
                nVertex = Convert.ToInt32(fileReader.ReadLine().Trim());
                Console.WriteLine("\nVertexuri citite: " + nVertex.ToString());

                string tmpStr = "";
                string[] str = new string[3];

                //read vertex coordinates from the file and store them in the array
                for (int i = 0; i < nVertex; i++)
                {
                    tmpStr = fileReader.ReadLine();
                    str = tmpStr.Trim().Split(' ');
                    arrVertex[i, 0] = Convert.ToInt32(str[0].Trim());
                    arrVertex[i, 1] = Convert.ToInt32(str[1].Trim());
                    arrVertex[i, 2] = Convert.ToInt32(str[2].Trim());
                }

                fileReader.Close();
            }
            catch (Exception)
            {
                statusFiles = false;
                Console.WriteLine("\nFisierul <" + fileVertex + "> nu exista!");
                MessageBox.Show("\nFisierul <" + fileVertex + "> nu exista!");
            }
        }

        //method to load quad lists for 3D object creation
        private void loadQList()
        {
            try
            {
                //check if the quad list file exists
                StreamReader fileReader = new StreamReader(fileQList);

                int tmp;
                string line;
                nQuadsList = 0;

                //read quad lists from the file and store them in the array
                while ((line = fileReader.ReadLine()) != null)
                {
                    tmp = Convert.ToInt32(line.Trim());
                    arrQuadsList[nQuadsList] = tmp;
                    nQuadsList++;
                }

                fileReader.Close();
            }
            catch (Exception)
            {
                statusFiles = false;
                MessageBox.Show("\nFisierul <" + fileQList + "> nu exista!");
            }
        }

        //method to load triangle lists for 3D object creation
        private void loadTList()
        {
            try
            {
                //check if the triangle list file exists
                StreamReader fileReader = new StreamReader(fileTList);

                int tmp;
                string line;
                nTrianglesList = 0;

                //read triangle lists from the file and store them in the array
                while ((line = fileReader.ReadLine()) != null)
                {
                    tmp = Convert.ToInt32(line.Trim());
                    arrTrianglesList[nTrianglesList] = tmp;
                    nTrianglesList++;
                }

                fileReader.Close();
            }
            catch (Exception)
            {
                statusFiles = false;
                MessageBox.Show("\nFisierul <" + fileTList + "> nu exista!");
            }
        }

        //camera control handlers
        //handler for changes in X-axis value of the camera
        private void numericXeye_ValueChanged(object sender, EventArgs e)
        {
            vp_X = (int)numericXeye.Value;
            GlControl1.Invalidate(); //force redraw of the entire OpenGL control, changes will be reflected (updated)
        }

        //handler for changes in Y-axis value of the camera
        private void numericYeye_ValueChanged(object sender, EventArgs e)
        {
            vp_Y = (int)numericYeye.Value;
            GlControl1.Invalidate(); //force redraw of the entire OpenGL control, changes will be reflected (updated)
        }

        //handler for changes in Z-axis value of the camera
        private void numericZeye_ValueChanged(object sender, EventArgs e)
        {
            vp_Z = (int)numericZeye.Value;
            GlControl1.Invalidate(); //force redraw of the entire OpenGL control, changes will be reflected (updated)
        }

        //handler for controlling the camera depth from (0,0,0)
        private void numericCameraDepth_ValueChanged(object sender, EventArgs e)
        {
            cam = 1 + ((float)numericCameraDepth.Value) * 0.1f;
            GlControl1.Invalidate(); //force redraw of the entire OpenGL control, changes will be reflected (updated)
        }

        //mouse control settings
        //method to set the 2D mouse control status
        private void setControlMouse2D(bool status)
        {
            if (status == false)
            {
                scm_2D = false;
                btnMouseControl2D.Text = "2D mouse control OFF";
            }
            else
            {
                scm_2D = true;
                btnMouseControl2D.Text = "2D mouse control ON";
            }
        }

        //method to set the 3D mouse control status
        private void setControlMouse3D(bool status)
        {
            if (status == false)
            {
                scm_3D = false;
                btnMouseControl3D.Text = "3D mouse control OFF";
            }
            else
            {
                scm_3D = true;
                btnMouseControl3D.Text = "3D mouse control ON";
            }
        }

        //mouse control and interaction handlers
        //button click event for toggling 2D mouse control
        private void btnMouseControl2D_Click(object sender, EventArgs e)
        {
            if (scm_2D == true)
            {
                setControlMouse2D(false);
            }
            else
            {
                setControlMouse3D(false);
                setControlMouse2D(true);
            }
        }

        //button click event for toggling 3D mouse control
        private void btnMouseControl3D_Click(object sender, EventArgs e)
        {
            if (scm_3D == true)
            {
                setControlMouse3D(false);
            }
            else
            {
                setControlMouse2D(false);
                setControlMouse3D(true);
            }
        }

        //mouse movement event handling for controlling 3D world movement
        private void GlControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (scm_Down == true)
            {
                mouse = new Point(e.X, e.Y);
                GlControl1.Invalidate(); //force redraw of the entire OpenGL control,changes will be reflected (updated)
            }
        }

        //mouse button down event handling
        private void GlControl1_MouseDown(object sender, MouseEventArgs e)
        {
            scm_Down = true;
        }

        //mouse button up event handling
        private void GlControl1_MouseUp(object sender, MouseEventArgs e)
        {
            scm_Down = false;
        }

        //illumination control
        //method to set illumination status ON/OFF
        private void setIlluminationStatus(bool status)
        {
            if (status == false)
            {
                lightON = false;
                btnLights.Text = "Iluminare OFF";
            }
            else
            {
                lightON = true;
                btnLights.Text = "Iluminare ON";
            }
        }

        //button click event to toggle illumination
        private void btnLights_Click(object sender, EventArgs e)
        {
            if (lightON == false)
            {
                setIlluminationStatus(true);
            }
            else
            {
                setIlluminationStatus(false);
            }
            GlControl1.Invalidate(); //force redraw of the entire OpenGL control, changes will be reflected (updated)
        }

        //button click event to get the maximum number of lights supported in OpenGL
        private void btnLightsNo_Click(object sender, EventArgs e)
        {
            int nr = GL.GetInteger(GetPName.MaxLights);
            MessageBox.Show("Nr. maxim de luminii pentru aceasta implementare este <" + nr.ToString() + ">.");
        }

        //method to set status for light source 0 ON/OFF
        private void setSource0Status(bool status)
        {
            if (status == false)
            {
                lightON_0 = false;
                btnLight0.Text = "Sursa 0 OFF";
            }
            else
            {
                lightON_0 = true;
                btnLight0.Text = "Sursa 0 ON";
            }
        }

        //method to set status for the second light source ON/OFF
        private void setSource1Status(bool status)
        {
            if (status == false)
            {
                lightON_1 = false; //second Light OFF
                btnLight1.Text = "Sursa 1 OFF";
            }
            else
            {
                lightON_1 = true; //second Light ON
                btnLight1.Text = "Sursa 1 ON";
            }
        }

        //button click event to toggle the first light source
        private void btnLight0_Click(object sender, EventArgs e)
        {
            if (lightON == true)
            {
                if (lightON_0 == false)
                {
                    setSource0Status(true);
                }
                else
                {
                    setSource0Status(false);
                }
                GlControl1.Invalidate();
            }
        }

        //button click event to toggle the second light source
        private void btnLight1_Click(object sender, EventArgs e)
        {
            if (lightON == true)
            {
                if (lightON_1 == false)
                {
                    setSource1Status(true);
                }
                else
                {
                    setSource1Status(false);
                }
                GlControl1.Invalidate();
            }
        }

        //method to set default position for the first light source
        private void setTrackLigh0Default()
        {
            trackLight0PositionX.Value = (int)valuesPosition0[0];
            trackLight0PositionY.Value = (int)valuesPosition0[1];
            trackLight0PositionZ.Value = (int)valuesPosition0[2];
        }

        //method to set default values for the second light source position
        private void setLight1Default()
        {
            valuesPosition1[0] = 0;
            valuesPosition1[1] = 0;
            valuesPosition1[2] = 0;
        }

        //handlers for changing the X, Y, Z position of the first light source
        private void trackLight0PositionX_Scroll(object sender, EventArgs e)
        {
            valuesPosition0[0] = trackLight0PositionX.Value;
            GlControl1.Invalidate();
        }

        private void trackLight0PositionY_Scroll(object sender, EventArgs e)
        {
            valuesPosition0[1] = trackLight0PositionY.Value;
            GlControl1.Invalidate();
        }

        private void trackLight0PositionZ_Scroll(object sender, EventArgs e)
        {
            valuesPosition0[2] = trackLight0PositionZ.Value;
            GlControl1.Invalidate();
        }

        //method to set default ambient light color for the first light source
        private void setColorAmbientLigh0Default()
        {
            numericLight0Ambient_Red.Value = (decimal)valuesAmbient0[0];
            numericLight0Ambient_Green.Value = (decimal)valuesAmbient0[1];
            numericLight0Ambient_Blue.Value = (decimal)valuesAmbient0[2];
        }

        //lab 9 modification - point 3
        //changing the color of Light Source 1 (ambient) in the RGB domain
        private void setColorAmbientLigh1Default()
        {
            numericLight0Ambient_Red.Value = (decimal)valuesAmbient1[0];
            numericLight0Ambient_Green.Value = (decimal)valuesAmbient1[1];
            numericLight0Ambient_Blue.Value = (decimal)valuesAmbient1[2];
        }

        private void numericLight0Ambient_Red_ValueChanged(object sender, EventArgs e)
        {
            valuesAmbient0[0] = (float)numericLight0Ambient_Red.Value / 100;
            valuesAmbient1[0] = (float)numericLight0Ambient_Red.Value / 100;
            GlControl1.Invalidate();
        }

        private void numericLight0Ambient_Green_ValueChanged(object sender, EventArgs e)
        {
            valuesAmbient0[1] = (float)numericLight0Ambient_Green.Value / 100;
            valuesAmbient1[1] = (float)numericLight0Ambient_Green.Value / 100;
            GlControl1.Invalidate();
        }

        private void numericLight0Ambient_Blue_ValueChanged(object sender, EventArgs e)
        {
            valuesAmbient0[2] = (float)numericLight0Ambient_Blue.Value / 100;
            valuesAmbient1[2] = (float)numericLight0Ambient_Blue.Value / 100;
            GlControl1.Invalidate();
        }

        //changing the color of Light Source 0 (diffuse) in the RGB domain
        private void setColorDifuseLigh0Default()
        {
            numericLight0Difuse_Red.Value = (decimal)valuesDiffuse0[0];
            numericLight0Difuse_Green.Value = (decimal)valuesDiffuse0[1];
            numericLight0Difuse_Blue.Value = (decimal)valuesDiffuse0[2];
        }

        //lab 9 modification - point 3
        //changing the color of Light Source 1 (diffuse) in the RGB domain
        private void setColorDifuseLigh1Default()
        {
            numericLight0Difuse_Red.Value = (decimal)valuesDiffuse1[0];
            numericLight0Difuse_Green.Value = (decimal)valuesDiffuse1[1];
            numericLight0Difuse_Blue.Value = (decimal)valuesDiffuse1[2];
        }

        private void numericLight0Difuse_Red_ValueChanged(object sender, EventArgs e)
        {
            valuesDiffuse0[0] = (float)numericLight0Difuse_Red.Value / 100;
            valuesDiffuse1[0] = (float)numericLight0Difuse_Red.Value / 100;
            GlControl1.Invalidate();
        }

        private void numericLight0Difuse_Green_ValueChanged(object sender, EventArgs e)
        {
            valuesDiffuse0[1] = (float)numericLight0Difuse_Green.Value / 100;
            valuesDiffuse1[1] = (float)numericLight0Difuse_Green.Value / 100;
            GlControl1.Invalidate();
        }

        private void numericLight0Difuse_Blue_ValueChanged(object sender, EventArgs e)
        {
            valuesDiffuse0[2] = (float)numericLight0Difuse_Blue.Value / 100;
            valuesDiffuse1[2] = (float)numericLight0Difuse_Blue.Value / 100;
            GlControl1.Invalidate();
        }

        //changing the color of Light Source 0 (specular) in the RGB domain
        private void setColorSpecularLigh0Default()
        {
            numericLight0Specular_Red.Value = (decimal)valuesSpecular0[0];
            numericLight0Specular_Green.Value = (decimal)valuesSpecular0[1];
            numericLight0Specular_Blue.Value = (decimal)valuesSpecular0[2];
        }

        //lab 9 modification - point 3
        //changing the color of Light Source 1 (specular) in the RGB domain
        private void setColorSpecularLigh1Default()
        {
            numericLight0Specular_Red.Value = (decimal)valuesSpecular1[0];
            numericLight0Specular_Green.Value = (decimal)valuesSpecular1[1];
            numericLight0Specular_Blue.Value = (decimal)valuesSpecular1[2];
        }

        private void numericLight0Specular_Red_ValueChanged(object sender, EventArgs e)
        {
            valuesSpecular0[0] = (float)numericLight0Specular_Red.Value / 100;
            valuesSpecular1[0] = (float)numericLight0Specular_Red.Value / 100;
            GlControl1.Invalidate();
        }

        private void numericLight0Specular_Green_ValueChanged(object sender, EventArgs e)
        {
            valuesSpecular0[1] = (float)numericLight0Specular_Green.Value / 100;
            valuesSpecular1[1] = (float)numericLight0Specular_Green.Value / 100;
            GlControl1.Invalidate();
        }

        private void numericLight0Specular_Blue_ValueChanged(object sender, EventArgs e)
        {
            valuesSpecular0[2] = (float)numericLight0Specular_Blue.Value / 100;
            valuesSpecular1[2] = (float)numericLight0Specular_Blue.Value / 100;
            GlControl1.Invalidate();
        }

        //resetting state of Light Source 0
        private void setLight0Values()
        {
            for (int i = 0; i < valuesAmbientTemplate0.Length; i++)
            {
                valuesAmbient0[i] = valuesAmbientTemplate0[i];
            }

            for (int i = 0; i < valuesDiffuseTemplate0.Length; i++)
            {
                valuesDiffuse0[i] = valuesDiffuseTemplate0[i];
            }

            for (int i = 0; i < valuesPositionTemplate0.Length; i++)
            {
                valuesPosition0[i] = valuesPositionTemplate0[i];
            }
        }

        //lab 9 modification - point 3, resetting state of Light Source 1 (the second one)
        private void setLight1Values()
        {
            for (int i = 0; i < valuesAmbientTemplate1.Length; i++)
            {
                valuesAmbient1[i] = valuesAmbientTemplate1[i];
            }

            for (int i = 0; i < valuesDiffuseTemplate1.Length; i++)
            {
                valuesDiffuse1[i] = valuesDiffuseTemplate1[i];
            }

            for (int i = 0; i < valuesPositionTemplate1.Length; i++)
            {
                valuesPosition1[i] = valuesPositionTemplate1[i];
            }
        }

        //lab 9 modification - point 3
        //update with setLight1Values()
        private void btnLight0Reset_Click(object sender, EventArgs e)
        {
            setLight0Values();
            setLight1Values();
            setTrackLigh0Default();
            setColorAmbientLigh0Default();
            setColorDifuseLigh0Default();
            setColorSpecularLigh0Default();
            setLight1Default();
            setColorAmbientLigh1Default();
            setColorDifuseLigh1Default();
            setColorSpecularLigh1Default();

            GlControl1.Invalidate();
        }

        //3D object control
        //set the state variable for displaying/hiding the coordinate system
        private void setControlAxe(bool status)
        {
            if (status == false)
            {
                sca = false;
                btnShowAxes.Text = "Axe Oxyz OFF";
            }
            else
            {
                sca = true;
                btnShowAxes.Text = "Axe Oxyz ON";
            }
        }

        //coordinate axis control (ON/OFF)
        private void btnShowAxis_Click(object sender, EventArgs e)
        {
            if (sca == true)
            {
                setControlAxe(false);
            }
            else
            {
                setControlAxe(true);
            }

            GlControl1.Invalidate();
        }

        //set the state variable for drawing the cube. Acceptable values are:
        //TRIANGLES = cube is drawn using triangles
        //QUADS = cube is drawn using quads
        //OFF (or anything else) = cube is not drawn
        private void setCubeStatus(string status)
        {
            if (status.Trim().ToUpper().Equals("TRIANGLES"))
            {
                statusCube = "TRIANGLES";
            }
            else if (status.Trim().ToUpper().Equals("QUADS"))
            {
                statusCube = "QUADS";
            }
            else
            {
                statusCube = "OFF";
            }
        }

        private void btnCubeQ_Click(object sender, EventArgs e)
        {
            statusFiles = true;

            loadVertex();
            loadQList();
            setCubeStatus("QUADS");

            GlControl1.Invalidate();
        }

        private void btnCubeT_Click(object sender, EventArgs e)
        {
            statusFiles = true;

            loadVertex();
            loadTList();
            setCubeStatus("TRIANGLES");

            GlControl1.Invalidate();
        }

        private void btnResetObjects_Click(object sender, EventArgs e)
        {
            setCubeStatus("OFF");

            GlControl1.Invalidate();
        }

        //lab 9 modification - point 3
        //the light source can be moved using 6 keys (3 spatial axis)
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Q)
            {
                valuesPosition1[0]++;
                GlControl1.Invalidate();
                Console.WriteLine("Sursa 1 de lumina. Incrementare valoare pe pozitia X. Valoarea este: " + valuesPosition1[0]);
            }

            if (e.KeyCode == Keys.A)
            {
                valuesPosition1[0]--;
                GlControl1.Invalidate();
                Console.WriteLine("Sursa 1 de lumina. Decrementare valoare pe pozitia X. Valoarea este: " + valuesPosition1[0]);
            }

            if (e.KeyCode == Keys.W)
            {
                valuesPosition1[1]++;
                GlControl1.Invalidate();
                Console.WriteLine("Sursa 1 de lumina. Incrementare valoare pe pozitia Y. Valoarea este: " + valuesPosition1[1]);
            }

            if (e.KeyCode == Keys.S)
            {
                valuesPosition1[1]--;
                GlControl1.Invalidate();
                Console.WriteLine("Sursa 1 de lumina. Decrementare valoare pe pozitia Y. Valoarea este: " + valuesPosition1[1]);
            }

            if (e.KeyCode == Keys.E)
            {
                valuesPosition1[2]++;
                GlControl1.Invalidate();
                Console.WriteLine("Sursa 1 de lumina. Incrementare valoare pe pozitia Z. Valoarea este: " + valuesPosition1[2]);
            }

            if (e.KeyCode == Keys.D)
            {
                valuesPosition1[2]--;
                GlControl1.Invalidate();
                Console.WriteLine("Sursa 1 de lumina. Decrementare valoare pe pozitia Z. Valoarea este: " + valuesPosition1[2]);
            }
        }

        //3D mode administration (MAIN METHOD)
        private void GlControl1_Paint(object sender, PaintEventArgs e)
        {
            //reset buffers to default values
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            //default color of the environment
            GL.ClearColor(Color.White);

            //preliminary settings for the 3D environment declaring the spatial perspective
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(cam, 4 / 3, 1, 10000);

            //declaring the camera (initial state)
            Matrix4 lookat = Matrix4.LookAt(vp_X, vp_Y, vp_Z, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            //loading the camera model
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookat);

            //size of the rendering surface (3D scene is projected onto this)
            GL.Viewport(0, 0, GlControl1.Width, GlControl1.Height);

            //depth corrections
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            //turning on lighting (if permitted)
            if (lightON == true)
            {
                //enable lighting. Without this correction, lighting won't work
                GL.Enable(EnableCap.Lighting);
            }
            else
            {
                //disable lighting
                GL.Disable(EnableCap.Lighting);
            }

            //creating the lighting source. In this case, we are using a single source
            //the number of light sources depends on the OpenGL implementation, but usually at least 8 sources are possible simultaneously
            GL.Light(LightName.Light0, LightParameter.Ambient, valuesAmbient0);
            GL.Light(LightName.Light0, LightParameter.Diffuse, valuesDiffuse0);
            GL.Light(LightName.Light0, LightParameter.Specular, valuesSpecular0);
            GL.Light(LightName.Light0, LightParameter.Position, valuesPosition0);

            //second lighting source
            GL.Light(LightName.Light1, LightParameter.Ambient, valuesAmbient1);
            GL.Light(LightName.Light1, LightParameter.Diffuse, valuesDiffuse1);
            GL.Light(LightName.Light1, LightParameter.Specular, valuesSpecular1);
            GL.Light(LightName.Light1, LightParameter.Position, valuesPosition1);

            if ((lightON == true) && (lightON_0 == true))
            {
                //activate light source 0. Without this action, we won't have illumination
                GL.Enable(EnableCap.Light0);
            }
            else
            {
                //deactivate light source 0
                GL.Disable(EnableCap.Light0);
            }

            if ((lightON == true) && (lightON_1 == true))
            {
                //activate light source 1. Without this action, we won't have illumination
                GL.Enable(EnableCap.Light1);
            }
            else
            {
                //deactivate light source 1
                GL.Disable(EnableCap.Light1);
            }

            //control rotation with mouse (2D)
            if (scm_2D == true)
            {
                GL.Rotate(mouse.X, 0, 1, 0);
            }

            //control rotation with mouse (3D)
            if (scm_3D == true)
            {
                GL.Rotate(mouse.X, 0, 1, 1);
            }

            //lab 9 modification - point 3
            //additional light source can be moved using: (b) mouse - (2 spatial axis)
            if (scm_2D == true)
            {
                //updating the position of the additional light source with mouse X axis
                valuesPosition1[0] = mouse.X;
                //valuesPosition1[1] = mousePos.Y; //this line might be used for Y-axis movement
                //GlControl1.Invalidate(); //refreshing the control (might be required)
            }

            //description of 3D objects
            //coordinate axis
            if (sca == true)
            {
                xyz.Draw(); //drawing coordinate axis
            }

            //drawing 3D objects (cube made of quads or triangles)
            if (statusCube.ToUpper().Equals("QUADS"))
            {
                cube.DrawCubeWithQuads(nQuadsList, arrQuadsList, arrVertex); //drawing the cube with quads
            }
            else if (statusCube.ToUpper().Equals("TRIANGLES"))
            {
                cube.DrawCubeWithTriangles(nTrianglesList, arrTrianglesList, arrVertex); //drawing the cube with triangles
            }

            GlControl1.SwapBuffers(); //swapping buffers for display

        }
    }
}