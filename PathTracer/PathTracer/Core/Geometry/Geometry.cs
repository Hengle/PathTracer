﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public abstract class Geometry
    {
        public Shader shader
        {
            get;
            set;
        }

        public Bounds bounds;

        public Geometry(Shader shader)
        {
            this.shader = shader;
        }

        

        public abstract bool RayCast(Ray ray, double epsilon, ref RayCastHit hit);
    }
}
