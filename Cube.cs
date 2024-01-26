using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Ghilescu_Dumitru_3131B
{
    //class for managing a cube in 3D space
    public class Cube
    {
        public Cube()
        {
            //constructor
        }

        //draw the cube - quads
        public void DrawCubeWithQuads(int nQuadsList, int[] arrQuadsList, int[,] arrVertex)
        {
            GL.Begin(PrimitiveType.Quads); //begin drawing quads

            for (int i = 0; i < nQuadsList; i++)
            {
                //assign colors based on index modulo to differentiate quads
                switch (i % 4)
                {
                    case 0:
                        GL.Color3(Color.Red); //red color
                        
                        break;
                    case 1:
                        GL.Color3(Color.Green); //green color
                        break;
                    case 2:
                        GL.Color3(Color.Blue); //blue color
                        break;
                    case 3:
                        GL.Color3(Color.Yellow); //yellow color
                        break;
                }

                int x = arrQuadsList[i];
                GL.Vertex3(arrVertex[x, 0], arrVertex[x, 1], arrVertex[x, 2]); //define vertices for the quads
            }
            GL.End(); //end drawing quads
        }

        //draw the cube - triangles
        public void DrawCubeWithTriangles(int nTrianglesList, int[] arrTrianglesList, int[,] arrVertex)
        {
            GL.Begin(PrimitiveType.Triangles); //begin drawing triangles

            for (int i = 0; i < nTrianglesList; i++)
            {
                //assign colors based on index modulo to differentiate triangles
                switch (i % 3)
                {
                    case 0:
                        GL.Color3(Color.Red); //red color
                        
                        break;
                    case 1:
                        GL.Color3(Color.Green); //green color
                        break;
                    case 2:
                        GL.Color3(Color.Blue); //blue color
                        break;
                }

                int x = arrTrianglesList[i];
                GL.Vertex3(arrVertex[x, 0], arrVertex[x, 1], arrVertex[x, 2]); //define vertices for the triangles
            }
            GL.End(); //end drawing triangles
        }
    }
}