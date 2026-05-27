using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DofusSwap.UI
{
    public static class Animator
    {
        private static readonly Timer _tick = new Timer { Interval = 16 };
        private static readonly List<Animation> _animations = new List<Animation>();

        static Animator()
        {
            _tick.Tick += OnTick;
        }

        public static void SlideTo(Control control, Point target, int durationMs = 200)
        {
            if (control.Location == target && !IsAnimating(control, AnimationType.Position))
                return;

            RemoveAnimations(control, AnimationType.Position);

            _animations.Add(new Animation
            {
                Control = control,
                Type = AnimationType.Position,
                FromPoint = control.Location,
                ToPoint = target,
                Duration = durationMs
            });

            if (!_tick.Enabled) _tick.Start();
        }

        public static void FadeTo(Form form, double target, int durationMs = 200)
        {
            RemoveAnimations(form, AnimationType.Opacity);

            _animations.Add(new Animation
            {
                Control = form,
                Type = AnimationType.Opacity,
                FromOpacity = form.Opacity,
                ToOpacity = target,
                Duration = durationMs
            });

            if (!_tick.Enabled) _tick.Start();
        }

        public static void Cancel(Control control)
        {
            _animations.RemoveAll(a => a.Control == control);
            if (_animations.Count == 0) _tick.Stop();
        }

        private static bool IsAnimating(Control control, AnimationType type)
        {
            return _animations.Exists(a => a.Control == control && a.Type == type);
        }

        private static void RemoveAnimations(Control control, AnimationType type)
        {
            for (int i = _animations.Count - 1; i >= 0; i--)
            {
                if (_animations[i].Control == control && _animations[i].Type == type)
                    _animations.RemoveAt(i);
            }
        }

        private static void OnTick(object sender, EventArgs e)
        {
            for (int i = _animations.Count - 1; i >= 0; i--)
            {
                var anim = _animations[i];
                anim.Elapsed += _tick.Interval;
                float t = Math.Min(1f, (float)anim.Elapsed / anim.Duration);
                float eased = EaseOutCubic(t);

                try
                {
                    switch (anim.Type)
                    {
                        case AnimationType.Position:
                            anim.Control.Location = new Point(
                                (int)(anim.FromPoint.X + (anim.ToPoint.X - anim.FromPoint.X) * eased),
                                (int)(anim.FromPoint.Y + (anim.ToPoint.Y - anim.FromPoint.Y) * eased));
                            break;

                        case AnimationType.Opacity:
                            if (anim.Control is Form form)
                                form.Opacity = anim.FromOpacity + (anim.ToOpacity - anim.FromOpacity) * eased;
                            break;
                    }
                }
                catch (ObjectDisposedException)
                {
                    _animations.RemoveAt(i);
                    continue;
                }

                if (t >= 1f)
                    _animations.RemoveAt(i);
            }

            if (_animations.Count == 0) _tick.Stop();
        }

        private static float EaseOutCubic(float t)
        {
            float inv = 1f - t;
            return 1f - inv * inv * inv;
        }

        private enum AnimationType { Position, Opacity }

        private class Animation
        {
            public Control Control;
            public AnimationType Type;
            public Point FromPoint, ToPoint;
            public double FromOpacity, ToOpacity;
            public int Duration;
            public int Elapsed;
        }
    }
}
