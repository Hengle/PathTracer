﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public struct Vector2
    {
        public double x, y;

        public double magnitude
        {
            get { return System.Math.Sqrt(x * x + y * y); }
        }

        public double sqrMagnitude
        {
            get { return x * x + y * y; }
        }

        public Vector2 normalized
        {
            get
            {
                double invm = 1.0 / this.magnitude;
                return new Vector2(x * invm, y * invm);
            }
        }

        public double this[int index]
        {
            get
            {
                if (index == 0)
                    return x;
                else if (index == 1)
                    return y;
                throw new System.IndexOutOfRangeException();
            }
            set
            {
                if (index == 0)
                    x = value;
                else if (index == 1)
                    y = value;
                throw new System.IndexOutOfRangeException();
            }
        }

        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public void Normalize()
        {
            double invm = 1.0 / this.magnitude;
            this.x *= invm;
            this.y *= invm;
        }

        public void Scale(Vector2 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
        }

        public static bool operator ==(Vector2 v1, Vector2 v2)
        {
            if (v1.x - v2.x > double.Epsilon)
                return false;
            if (v2.x - v1.x > double.Epsilon)
                return false;
            if (v1.y - v2.y > double.Epsilon)
                return false;
            if (v2.y - v1.y > double.Epsilon)
                return false;
            return true;
        }

        public static bool operator !=(Vector2 v1, Vector2 v2)
        {
            if (v1.x - v2.x > double.Epsilon)
                return true;
            if (v2.x - v1.x > double.Epsilon)
                return true;
            if (v1.y - v2.y > double.Epsilon)
                return true;
            if (v2.y - v1.y > double.Epsilon)
                return true;
            return false;
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x - v2.x, v1.y - v2.y);
        }

        public static Vector2 operator *(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x * v2.x, v1.y * v2.y);
        }

        public static Vector2 operator *(Vector2 v, double f)
        {
            return new Vector2(v.x * f, v.y * f);
        }

        public static Vector2 operator *(double f, Vector2 v)
        {
            return new Vector2(v.x * f, v.y * f);
        }

        public static Vector2 operator /(Vector2 v, double f)
        {
            double invf = 1.0 / f;
            return new Vector2(v.x * invf, v.y * invf);
        }

        public static Vector2 operator /(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x / v2.x, v1.y / v2.y);
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, double t)
        {
            return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }

        public static Vector2 Reflect(Vector2 i, Vector2 n)
        {
            return i - 2.0*Vector2.Dot(n, i)*n;
        }

        public static Vector2 Refract(Vector2 i, Vector2 n, double eta)
        {
            double cosi = Vector2.Dot(-1.0 * i, n);
            double cost2 = 1.0 - eta * eta * (1.0 - cosi * cosi);
            Vector2 t = eta * i + ((eta * cosi - Math.Sqrt(Math.Abs(cost2))) * n);
            double v = cost2 > 0 ? 1.0 : 0.0;
            return new Vector2(v * t.x, v * t.y);
        }

        public static double Angle(Vector2 from, Vector2 to)
        {
            Vector2 fn = from.normalized;
            Vector2 tn = to.normalized;
            double v = Vector2.Dot(fn, tn);
            if (v < -1.0)
                v = -1.0;
            else if (v > 1.0)
                v = 1.0;
            return Math.Acos(v) * 57.29578;
        }

        public static double Dot(Vector2 lhs, Vector2 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y;
        }

        public static double Distance(Vector2 lhs, Vector2 rhs)
        {
            return (lhs - rhs).magnitude;
        }

        public static Vector2 Max(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
        }

        public static Vector2 Min(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
        }

        public override bool Equals(object obj)
        {
            var objVector = (Vector2) obj;
            return objVector.x == this.x && objVector.y == this.y;
        }

        public override string ToString()
        {
            return $"Vector2({this.x},{this.y})";
        }
    }

    public struct Vector3
    {
        public double x, y, z;

        public double magnitude
        {
            get { return System.Math.Sqrt(x * x + y * y + z * z); }
        }

        public double sqrMagnitude
        {
            get { return x * x + y * y + z * z; }
        }

        public Vector3 normalized
        {
            get
            {
                double invm = 1.0 / this.magnitude;
                return new Vector3(x * invm, y * invm, z * invm);
            }
        }

        public double this[int index]
        {
            get
            {
                if (index == 0)
                    return x;
                else if (index == 1)
                    return y;
                else if (index == 2)
                    return z;
                throw new System.IndexOutOfRangeException();
            }
            set
            {
                if (index == 0)
                    x = value;
                else if (index == 1)
                    y = value;
                else if (index == 2)
                    z = value;
                throw new System.IndexOutOfRangeException();
            }
        }

        public static Vector3 one
        {
            get { return new Vector3(1, 1, 1); }
        }

        public static Vector3 zero
        {
            get { return new Vector3(0, 0, 0); }
        }

        public static Vector3 right
        {
            get { return new Vector3(1, 0, 0); }
        } 

        public  static Vector3 left
        {
            get { return new Vector3(-1, 0, 0); }
        }

        public  static Vector3 up
        {
            get { return new Vector3(0, 1, 0); }
        }

        public  static  Vector3 down
        {
            get { return new Vector3(0, -1, 0); }
        }

        public  static Vector3 forward
        {
            get { return new Vector3(0, 0, 1); }
        }

        public static Vector3 back
        {
            get { return new Vector3(0, 0, -1); }
        }

        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void Normalize()
        {
            double invm = 1.0 / this.magnitude;
            this.x *= invm;
            this.y *= invm;
            this.z *= invm;
        }

        public void Scale(Vector3 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
        }

        public static bool operator ==(Vector3 v1, Vector3 v2)
        {
            if (v1.x - v2.x > double.Epsilon)
                return false;
            if (v2.x - v1.x > double.Epsilon)
                return false;
            if (v1.y - v2.y > double.Epsilon)
                return false;
            if (v2.y - v1.y > double.Epsilon)
                return false;
            if (v1.z - v2.z > double.Epsilon)
                return false;
            if (v2.z - v1.z > double.Epsilon)
                return false;
            return true;
        }

        public static bool operator !=(Vector3 v1, Vector3 v2)
        {
            if (v1.x - v2.x > double.Epsilon)
                return true;
            if (v2.x - v1.x > double.Epsilon)
                return true;
            if (v1.y - v2.y > double.Epsilon)
                return true;
            if (v2.y - v1.y > double.Epsilon)
                return true;
            if (v1.z - v2.z > double.Epsilon)
                return true;
            if (v2.z - v1.z > double.Epsilon)
                return true;
            return false;
        }

        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public static Vector3 operator *(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }

        public static Vector3 operator *(Vector3 v, double f)
        {
            return new Vector3(v.x * f, v.y * f, v.z * f);
        }

        public static Vector3 operator *(double f, Vector3 v)
        {
            return new Vector3(v.x * f, v.y * f, v.z * f);
        }

        public static Vector3 operator /(Vector3 v, double f)
        {
            double invf = 1.0 / f;
            return new Vector3(v.x * invf, v.y * invf, v.z * invf);
        }

        public static Vector3 operator /(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, double t)
        {
            return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        public static Vector3 Reflect(Vector3 i, Vector3 n)
        {
            return i - 2.0 * Vector3.Dot(n, i) * n;
        }

        public static Vector3 Refract(Vector3 i, Vector3 n, double eta)
        {
            double cosi = Vector3.Dot(-1.0 * i, n);
            double cost2 = 1.0 - eta * eta * (1.0 - cosi * cosi);
            Vector3 t = eta * i + ((eta * cosi - Math.Sqrt(Math.Abs(cost2))) * n);
            double v = cost2 > 0 ? 1.0 : 0.0;
            return new Vector3(v * t.x, v * t.y, v * t.z);
        }

        public static double Angle(Vector3 from, Vector3 to)
        {
            Vector3 fn = from.normalized;
            Vector3 tn = to.normalized;
            double v = Vector3.Dot(fn, tn);
            if (v < -1.0)
                v = -1.0;
            else if (v > 1.0)
                v = 1.0;
            return Math.Acos(v) * 57.29578;
        }

        public static double Dot(Vector3 lhs, Vector3 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }

        public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
        }

        public static double Distance(Vector3 lhs, Vector3 rhs)
        {
            return (lhs - rhs).magnitude;
        }

        public static Vector3 Max(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z));
        }

        public static Vector3 Min(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z));
        }

        public static Vector3 Project(Vector3 vector, Vector3 normal)
        {
            double n = Vector3.Dot(normal, normal);
            Vector3 result = default(Vector3);
            if (n >= double.Epsilon)
                result = normal*Vector3.Dot(vector, normal)/n;
            return result;
        }

        public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
        {
            Vector3 pj = Vector3.Project(vector, planeNormal);
            return vector - pj;
        }

        public override bool Equals(object obj)
        {
            var objVector = (Vector3)obj;
            return objVector.x == this.x && objVector.y == this.y && objVector.z == this.z;
        }

        public override string ToString()
        {
            return $"Vector3({this.x},{this.y},{this.z})";
        }
    }
}
