using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Utilities
{
    public class ControllerHelper
    {
        public List<TouchState> Touches = new List<TouchState>(4);
        private bool onMobile;
        public ControllerHelper()
        {
            Platforms platform = Constants.GetPlatformType();
            onMobile = platform == Platforms.Android || platform == Platforms.IPhone;
            for (int i = 0; i < 6; i++)
            {
                Touches.Add(new TouchState { FingerId = i, active = false });
            }
        }

        public void Update()
        {
            foreach (TouchState touch in Touches)
            {
                touch.active = false;
            }
            try
            {
                if (onMobile)
                {
                    int count = Mathf.Min(Input.touchCount, 4);
                    for (int i = 0; i < count; i++)
                    {
                        Touch ut = Input.GetTouch(i);
                        TouchState t = Touches[i];
                        t.FingerId = ut.fingerId;
                        t.ScreenPosition = ut.position;
                        t.Position = Camera.main.ScreenToWorldPoint(new Vector3(ut.position.x, ut.position.y, 10f));
                        t.IsDown = ut.phase != TouchPhase.Ended && ut.phase != TouchPhase.Canceled;
                        t.active = true;
                        t.UpdateState();
                    }
                }
                else
                {
                    Vector3 screen = Input.mousePosition;
                    screen.z = 10f;
                    Vector2 mousePos = Camera.main.ScreenToWorldPoint(screen);
                    bool mouseDown = Input.GetMouseButton(0);
                    var t = Touches[0];
                    t.ScreenPosition = Input.mousePosition;
                    t.Position = mousePos;
                    t.IsDown = mouseDown;
                    t.active = mouseDown || Input.GetMouseButtonUp(0);
                    t.UpdateState();
                }
            }
            catch (Exception ex)
            {
                FungleAPIPlugin.Instance.Log.LogError(ex);
            }
        }
        public bool AnyClicked(out TouchState touch, Collider2D collider = null)
        {
            foreach (var t in Touches)
            {
                if (t.active && t.Clicked && (collider == null || collider != null && collider.OverlapPoint(t.Position)))
                {
                    touch = t;
                    return true;
                }
            }
            touch = null;
            return false;
        }
        public bool AnyStarted(out TouchState touch, Collider2D collider = null)
        {
            foreach (var t in Touches)
            {
                if (t.active && t.dragState == DragState.TouchStart && (collider == null || collider != null && collider.OverlapPoint(t.Position)))
                {
                    touch = t;
                    return true;
                }
            }
            touch = null;
            return false;
        }

        public bool AnyDragging(out TouchState touch, Collider2D collider = null)
        {
            foreach (var t in Touches)
            {
                if (t.active && t.dragState == DragState.Dragging && (collider == null || collider != null && collider.OverlapPoint(t.Position)))
                {
                    touch = t;
                    return true;
                }
            }
            touch = null;
            return false;
        }

        public bool AnyHolding(out TouchState touch, Collider2D collider = null)
        {
            foreach (var t in Touches)
            {
                if (t.active && t.dragState == DragState.Holding && (collider == null || collider != null && collider.OverlapPoint(t.Position)))
                {
                    touch = t;
                    return true;
                }
            }
            touch = null;
            return false;
        }
        public class TouchState
        {
            public int FingerId;
            public Vector2 ScreenPosition;
            public Vector2 Position;
            public Vector2 ScreenDownAt;
            public Vector2 DownAt;
            public bool IsDown;
            public bool WasDown;
            public bool Clicked;
            public float Timer;
            public bool active;
            public DragState dragState;
            public void UpdateState()
            {
                if (!active)
                {
                    Clicked = false;
                    dragState = DragState.NoTouch;
                    WasDown = false;
                    return;
                }
                if (IsDown && !WasDown)
                {
                    dragState = DragState.TouchStart;
                    ScreenDownAt = ScreenPosition;
                    DownAt = Position;
                    Timer = 0f;
                }
                else if (IsDown && WasDown)
                {
                    Timer += Time.deltaTime;
                    if (Vector2.Distance(ScreenDownAt, ScreenPosition) > 10 || dragState == DragState.Dragging)
                    {
                        dragState = DragState.Dragging;
                    }
                    else
                    {
                        dragState = DragState.Holding;
                    }
                }
                else if (!IsDown && WasDown)
                {
                    Clicked = Timer <= 0.3f && dragState != DragState.Dragging;
                    dragState = DragState.Released;
                    Timer = 0f;
                }
                else
                {
                    Clicked = false;
                    dragState = DragState.NoTouch;
                }
                WasDown = IsDown;
            }
        }
    }
}
