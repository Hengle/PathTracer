﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    static class MeshLoader
    {
        public static List<Triangle> LoadMesh(string path, Vector3 position, Vector3 euler, Vector3 scale, Shader shader)
        {
            Matrix matrix = Matrix.TRS(position, euler, scale);
            return LoadMesh(path, matrix, shader);
        }

        static List<Triangle> LoadMesh(string path, Matrix matrix, Shader shader)
        {
            if(System.IO.File.Exists(path) == false)
                return null;
            List<Triangle> triangles = new List<Triangle>();
            StreamReader reader = new StreamReader(path);

            List<Vector3> vlist = new List<Vector3>();
            List<Vector3> nlist = new List<Vector3>();
            List<Vector2> ulist = new List<Vector2>();

            while (reader.EndOfStream == false)
            {
                string line = reader.ReadLine();
                if (line.Length > 1)
                {
                    char df0 = line[0];
                    char df1 = line[1];

                    if (df0 == 'v')
                    {
                        string[] sp = line.Substring(2).Trim().Split(' ');
                        if (df1 == 'n')
                        {
                            double nx = double.Parse(sp[0]);
                            double ny = double.Parse(sp[1]);
                            double nz = double.Parse(sp[2]);
                            nlist.Add(new Vector3(nx, ny, nz));
                        }
                        else if (df1 == 't')
                        {
                            double ux = double.Parse(sp[0]);
                            double uy = double.Parse(sp[1]);
                            ulist.Add(new Vector2(ux, uy));
                        }
                        else
                        {
                            double px = double.Parse(sp[0]);
                            double py = double.Parse(sp[1]);
                            double pz = double.Parse(sp[2]);
                            vlist.Add(new Vector3(px, py, pz));
                        }
                    }
                    else if (df0 == 'f')
                    {
                        string[] sp = line.Split(' ');
                        string[] face0 = sp[1].Split('/');
                        string[] face1 = sp[2].Split('/');
                        string[] face2 = sp[3].Split('/');

                        int vindex0 = int.Parse(face0[0]) - 1;
                        int vindex1 = int.Parse(face1[0]) - 1;
                        int vindex2 = int.Parse(face2[0]) - 1;

                        int uindex0 = int.Parse(face0[1]) - 1;
                        int uindex1 = int.Parse(face1[1]) - 1;
                        int uindex2 = int.Parse(face2[1]) - 1;

                        int nindex0 = int.Parse(face0[2]) - 1;
                        int nindex1 = int.Parse(face1[2]) - 1;
                        int nindex2 = int.Parse(face2[2]) - 1;

                        Vector3 v0 = matrix.TransformPoint(vlist[vindex0]);
                        Vector3 v1 = matrix.TransformPoint(vlist[vindex1]);
                        Vector3 v2 = matrix.TransformPoint(vlist[vindex2]);

                        Vector3 n0 = matrix.TransformVector(nlist[nindex0]);
                        Vector3 n1 = matrix.TransformVector(nlist[nindex1]);
                        Vector3 n2 = matrix.TransformVector(nlist[nindex2]);

                        Vector2 uv0 = ulist[uindex0];
                        Vector2 uv1 = ulist[uindex1];
                        Vector2 uv2 = ulist[uindex2];

                        Triangle triangle = new Triangle(v0, v1, v2, n0, n1, n2, uv0, uv1, uv2, shader);

                        triangles.Add(triangle);
                    }
                }
            }

            return triangles;
        }
    }
}
