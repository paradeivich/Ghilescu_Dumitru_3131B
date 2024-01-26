using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Ghilescu_Dumitru_3131B
{
    class Axis
    {
        private bool myVisibility;
        private const int AXIS_LENGTH = 75; //constant for axis length

        public Axis()
        {
            myVisibility = true; //default visibility set to true
        }

        //method to draw the axes
        public void Draw()
        {
            if (myVisibility) //check visibility flag
            {
                GL.LineWidth(1.0f); //set line width

                GL.Begin(PrimitiveType.Lines); //begin drawing lines

                //X-axis (Red color)
                GL.Color3(Color.Red);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(AXIS_LENGTH, 0, 0);

                //Y-axis (Green color)
                GL.Color3(Color.ForestGreen);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, AXIS_LENGTH, 0);

                //Z-axis (Blue color)
                GL.Color3(Color.RoyalBlue);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, 0, AXIS_LENGTH);

                GL.End(); //end drawing lines
            }
        }

        //method to show the axes
        public void Show()
        {
            myVisibility = true; //set visibility to true
        }

        //method to hide the axes
        public void Hide()
        {
            myVisibility = false; //set visibility to false
        }

        //method to toggle visibility of the axes
        public void ToggleVisibility()
        {
            myVisibility = !myVisibility; //toggle visibility flag
        }
    }
}