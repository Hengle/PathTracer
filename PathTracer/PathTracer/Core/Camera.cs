﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASL.PathTracer
{
    public class Camera
    {
        private class RenderJob
        {
            public int x;
            public int y;
            public Camera camera;
            public Scene scene;
            public SamplerBase sampler;
            public ManualResetEvent resetEvent;

            public RenderJob(int x, int y, SamplerBase sampler, Scene scene, Camera camera)
            {
                this.x = x;
                this.y = y;
                this.sampler = sampler;
                this.scene = scene;
                this.camera = camera;
                this.resetEvent = new ManualResetEvent(false);
            }

            public void Render(System.Object stateObject)
            {
                camera.RenderPixel(x, y, sampler, scene);
                resetEvent.Set();
            }
        }

        private class FastRenderJob
        {
            public int x;
            public int y;
            public Camera camera;
            public Scene scene;
            public ManualResetEvent resetEvent;

            public FastRenderJob(int x, int y, Scene scene, Camera camera)
            {
                this.x = x;
                this.y = y;
                this.scene = scene;
                this.camera = camera;
                this.resetEvent = new ManualResetEvent(false);
            }

            public void Render(System.Object stateObject)
            {
                camera.FastRenderPixel(x, y, scene);
                resetEvent.Set();
            }
        }

        public Vector3 position { get; private set; }

        public Vector3 right { get { return m_Right; } }

        public Vector3 up { get { return m_Up; } }

        public Vector3 forward { get { return m_Forward; } }

        public double near { get; private set; }

        public double fieldOfView { get; private set; }

        private Vector3 m_Right;
        private Vector3 m_Up;
        private Vector3 m_Forward;

        private Texture m_RenderTarget;
        private SamplerBase[] m_Samplers;

        private double m_Height;
        private double m_Width;

        public Camera(Vector3 position, Vector3 euler, double near, double fieldOfView)
        {
            this.near = near;
            this.fieldOfView = fieldOfView;
            this.position = position;

            double cosx = Math.Cos(euler.x * 0.01745329252 * 0.5);
            double cosy = Math.Cos(euler.y * 0.01745329252 * 0.5);
            double cosz = Math.Cos(euler.z * 0.01745329252 * 0.5);

            double sinx = Math.Sin(euler.x * 0.01745329252 * 0.5);
            double siny = Math.Sin(euler.y * 0.01745329252 * 0.5);
            double sinz = Math.Sin(euler.z * 0.01745329252 * 0.5);

            double rx = cosy * sinx * cosz + siny * cosx * sinz;
            double ry = siny * cosx * cosz - cosy * sinx * sinz;
            double rz = cosy * cosx * sinz - siny * sinx * cosz;
            double rw = cosy * cosx * cosz + siny * sinx * sinz;

            double x2 = 2.0 * rx * rx;
            double y2 = 2.0 * ry * ry;
            double z2 = 2.0 * rz * rz;
            double xy = 2.0 * rx * ry;
            double xz = 2.0 * rx * rz;
            double xw = 2.0 * rx * rw;
            double yz = 2.0 * ry * rz;
            double yw = 2.0 * ry * rw;
            double zw = 2.0 * rz * rw;

            double ra = 1.0 - y2 - z2;
            double rb = xy + zw;
            double rc = xz - yw;
            double rd = xy - zw;
            double re = 1.0 - x2 - z2;
            double rf = yz + xw;
            double rg = xz + yw;
            double rh = yz - xw;
            double ri = 1.0 - x2 - y2;

            m_Right.x = ra;
            m_Right.y = rb;
            m_Right.z = rc;

            m_Up.x = rd;
            m_Up.y = re;
            m_Up.z = rf;

            m_Forward.x = rg;
            m_Forward.y = rh;
            m_Forward.z = ri;
            
        }

        public void SetRenderTarget(Texture renderTarget)
        {
            m_RenderTarget = renderTarget;
            if (m_RenderTarget == null)
                return;

            float aspect = (((float) renderTarget.width) / renderTarget.height);

            m_Height = near * Math.Tan(fieldOfView * 0.5 * 0.01745329252);
            m_Width = aspect * m_Height;
        }

        public void SetSampler(SamplerType samplerType, int numSamples, int numSets = 83)
        {
            m_Samplers = new SamplerBase[Environment.ProcessorCount];
            for (int i = 0; i < m_Samplers.Length; i++)
                m_Samplers[i] = SamplerFactory.Create(samplerType, numSamples, numSets);
            //m_Sampler = sampler;
        }

        public Ray GetRayFromPixel(double x, double y)
        {
            if(m_RenderTarget == null)
                throw new System.NullReferenceException();
            x = x / m_RenderTarget.width * 2 - 1;
            y = y / m_RenderTarget.height * 2 - 1;

            Vector2 point = new Vector2(x * m_Width, y * m_Height);
            return GetRayFromPoint(point);
        }

        public Ray GetRayFromPoint(Vector2 point)
        {
            Vector3 dir = m_Right*point.x + m_Up*point.y + m_Forward*this.near;
            Vector3 ori = this.position + dir;
            dir.Normalize();
            return new Ray(ori, dir);
        }

        public void Render(Scene scene, bool multiThread, System.Action<int, int> progressCallBackAction = null)
        {
            if (scene == null)
                throw new System.ArgumentNullException();
            if (m_RenderTarget == null)
                throw new System.NullReferenceException("未设置RenderTarget");

            if(multiThread)
                RenderMultiThread(scene, progressCallBackAction);
            else 
                RenderSingleThread(scene, progressCallBackAction);
        }

        public void FastRender(Scene scene, bool multiThread, System.Action<int, int> progressCallBackAction = null)
        {
            if (scene == null)
                throw new System.ArgumentNullException();
            if (m_RenderTarget == null)
                throw new System.NullReferenceException("未设置RenderTarget");

            if (multiThread)
                FastRenderMultiThread(scene, progressCallBackAction);
            else
                FastRenderSingleThread(scene, progressCallBackAction);
        }

        private void RenderSingleThread(Scene scene, System.Action<int, int> progressCallBackAction)
        {
            var sampler = m_Samplers[0];
            int total = m_RenderTarget.width * m_RenderTarget.height;
            for (int j = 0; j < m_RenderTarget.height; j++)
            {
                for (int i = 0; i < m_RenderTarget.width; i++)
                {
                    RenderPixel(i, j, sampler, scene);
                    progressCallBackAction?.Invoke(j * m_RenderTarget.width + i, total);
                }
            }
        }

        private void FastRenderSingleThread(Scene scene, System.Action<int, int> progressCallBackAction)
        {
            int total = m_RenderTarget.width * m_RenderTarget.height;
            for (int j = 0; j < m_RenderTarget.height; j++)
            {
                for (int i = 0; i < m_RenderTarget.width; i++)
                {
                    FastRenderPixel(i, j, scene);
                    progressCallBackAction?.Invoke(j * m_RenderTarget.width + i, total);
                }
            }
        }

        private void RenderMultiThread(Scene scene, System.Action<int, int> progressCallBackAction)
        {
            List<ManualResetEvent> eventList = new List<ManualResetEvent>();

            int samplerIndex = 0;
            int total = m_RenderTarget.width * m_RenderTarget.height;
            int finished = 0;
            for (int j = 0; j < m_RenderTarget.height; j++)
            {
                for (int i = 0; i < m_RenderTarget.width; i++)
                {
                    var job = new RenderJob(i, j, m_Samplers[samplerIndex], scene, this);
                    samplerIndex += 1;
                    if (samplerIndex >= m_Samplers.Length)
                        samplerIndex = 0;
                    eventList.Add(job.resetEvent);
                    ThreadPool.QueueUserWorkItem(job.Render);

                    if (eventList.Count >= Environment.ProcessorCount)
                    {
                        WaitRender(eventList);
                        progressCallBackAction?.Invoke(finished, total);
                        finished++;
                    }
                }
            }

            while (eventList.Count > 0)
            {
                WaitRender(eventList);
                progressCallBackAction?.Invoke(finished, total);
                finished++;
            }
        }

        private void FastRenderMultiThread(Scene scene, System.Action<int, int> progressCallBackAction)
        {
            List<ManualResetEvent> eventList = new List<ManualResetEvent>();
            
            int total = m_RenderTarget.width * m_RenderTarget.height;
            int finished = 0;
            for (int j = 0; j < m_RenderTarget.height; j++)
            {
                for (int i = 0; i < m_RenderTarget.width; i++)
                {
                    var job = new FastRenderJob(i, j, scene, this);
                    eventList.Add(job.resetEvent);
                    ThreadPool.QueueUserWorkItem(job.Render);

                    if (eventList.Count >= Environment.ProcessorCount)
                    {
                        WaitRender(eventList);
                        progressCallBackAction?.Invoke(finished, total);
                        finished++;
                    }
                }
            }

            while (eventList.Count > 0)
            {
                WaitRender(eventList);
                progressCallBackAction?.Invoke(finished, total);
                finished++;
            }
        }

        internal void RenderPixel(int x, int y, SamplerBase sampler, Scene scene)
        {
            for (int k = 0; k < sampler.numSamples; k++)
            {
                var sample = sampler.Sample();
                Ray ray = GetRayFromPixel(x + sample.x, y + sample.y);
                m_RenderTarget.SetPixel(x, y, scene.tracer.Tracing(ray, scene.sky, sampler));
            }
        }

        internal void FastRenderPixel(int x, int y, Scene scene)
        {
            Ray ray = GetRayFromPixel(x + 0.5f, y + 0.5f);
            m_RenderTarget.SetPixel(x, y, scene.tracer.FastTracing(ray));
        }

        private void WaitRender(List<ManualResetEvent> events)
        {
            int finished = WaitHandle.WaitAny(events.ToArray());
            if (finished == WaitHandle.WaitTimeout)
            {
            }
            events.RemoveAt(finished);
        }
    }
}
