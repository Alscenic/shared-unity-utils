using UnityEngine;

namespace CGenStudios.UnityUtils
{
    public partial class Console
    {
        public class Window
        {
            public const float DEFAULT_WIDTH = 800.0f;

            public const float DEFAULT_HEIGHT = 600.0f;

            public const float MIN_WIDTH = 400.0f;

            public const float MIN_HEIGHT = 125.0f;

            private Rect LocalRect { get; set; } = new Rect(20.0f, 20.0f, DEFAULT_WIDTH, DEFAULT_HEIGHT);

            public Rect WindowRect
            {
                get => LocalRect;
                set
                {
                    LocalRect = value;

                    LocalRect = new Rect(new Vector2(
                        Mathf.Clamp(LocalRect.x, 0.0f, Mathf.Max(0.0f, Screen.width - LocalRect.width)),
                        Mathf.Clamp(LocalRect.y, 0.0f, Mathf.Max(0.0f, Screen.height - LocalRect.height))),
                        LocalRect.size);

                    LocalRect = new Rect(LocalRect.position, new Vector2(
                        Mathf.Clamp(LocalRect.width, MIN_WIDTH, Screen.width - LocalRect.x),
                        Mathf.Clamp(LocalRect.height, MIN_HEIGHT, Screen.height - LocalRect.y)));
                }
            }

            public Vector4 Padding => new Vector4(GripSize, GripSize, GripSize, GripSize);

            public Rect InteriorRect => new Rect(WindowRect.position + new Vector2(Padding.x, Padding.y), WindowRect.size - new Vector2(Padding.x, Padding.y) - new Vector2(Padding.z, Padding.w));

            public float GripSize { get; set; } = 20.0f;

            public float InputFieldSize { get; set; } = 40.0f;

            public bool MouseDown { get; private set; } = false;

            public bool Grabbing { get; private set; } = false;

            public bool SizingX { get; private set; } = false;

            public bool SizingY { get; private set; } = false;

            public Vector2 MouseDownOrigin { get; private set; } = new Vector2();

            public Vector2 MouseDownOffset { get; private set; } = new Vector2();

            public string CurrentInput { get; set; } = "";

            public Rect GrabBarRect => new Rect(WindowRect.position, new Vector2(WindowRect.width, GripSize));

            public Rect SizeBarXRect => new Rect(WindowRect.position + new Vector2(WindowRect.size.x - GripSize, GripSize),
            new Vector2(GripSize, WindowRect.height - GripSize));

            public Rect SizeBarYRect => new Rect(WindowRect.position + new Vector2(0.0f, WindowRect.size.y - GripSize),
            new Vector2(WindowRect.width, GripSize));

            public Rect InputFieldRect => new Rect(InteriorRect.position + new Vector2(0.0f, InteriorRect.size.y - InputFieldSize),
            new Vector2(InteriorRect.size.x, InputFieldSize));

            public Rect LogAreaScrollRect => new Rect(InteriorRect.position,
            InteriorRect.size - new Vector2(0.0f, InputFieldSize));

            public float GutterWidth { get; set; } = 0.0f;

            public float ScrollbarHeight => GUI.skin.horizontalScrollbar.CalcHeight(new GUIContent(), 100.0f) + 4.0f;

            public Rect LogAreaRect => new Rect(Vector2.zero, new Vector2(0.0f, LogAreaScrollRect.size.y - 1.0f) + new Vector2(MaxLogAreaWidth, -ScrollbarHeight));

            public float MaxLogAreaWidth { get; private set; } = 0.0f;

            private Vector2 MousePos => Event.current.mousePosition;

            public Vector2 ScrollPos { get; set; } = new Vector2();

            private int LocalScrollLine { get; set; } = 0;

            private bool JustOpenedThisFrame { get; set; } = false;

            public float LineOffset { get; private set; } = 0.0f;

            private int LastLineNum { get; set; } = 0;

            public string LastCommand { get; set; } = "";

            public int ScrollLine
            {
                get => LocalScrollLine;
                set => LocalScrollLine = Mathf.Clamp(value, 0, Instance.LocalLogger.Entries.Count - 1);
            }

            public void Draw()
            {
                if (Event.current.keyCode == KeyCode.UpArrow)
                {
                    CurrentInput = LastCommand;
                }

                GUI.Box(WindowRect, new GUIContent());

                // Vector2 actualGripSquare = new Vector2(GripSize, GripSize);
                // GUI.Box(new Rect(WindowRect.position + WindowRect.size - actualGripSquare, actualGripSquare), new GUIContent(), GUI.skin.GetStyle("grabme"));

                GUI.SetNextControlName("ConsoleInput");
                CurrentInput = GUI.TextField(InputFieldRect, CurrentInput);
                if (JustOpenedThisFrame)
                {
                    GUI.FocusControl("ConsoleInput");

                    for (int lvl = 0; lvl < typeof(LogLevel).GetEnumValues().Length; lvl++)
                    {
                        GutterWidth = Mathf.Max(GutterWidth, GUI.skin.label.CalcSize(new GUIContent(((LogLevel)lvl).ToString() + "[]")).x);
                    }
                    GutterWidth += 4.0f;

                    JustOpenedThisFrame = false;
                }

                while (CurrentInput.IndexOf('`') >= 0)
                {
                    CurrentInput = CurrentInput.Replace("`", "");
                }

                ScrollPos = GUI.BeginScrollView(LogAreaScrollRect, ScrollPos, LogAreaRect);

                MaxLogAreaWidth = 0.0f;

                float lineHeight = GUI.skin.label.CalcHeight(new GUIContent(" "), 10.0f);

                int bottomLineNum = Instance.LocalLogger.Entries.Count - ScrollLine - 1;
                if (LastLineNum != bottomLineNum)
                {
                    LineOffset = (bottomLineNum - LastLineNum) * lineHeight;
                }

                int i = 0;
                while ((i + 1) * lineHeight <= LogAreaRect.height && Instance.LocalLogger.Entries.Count - (i + ScrollLine) - 1 >= 0)
                {
                    int lineNum = Instance.LocalLogger.Entries.Count - (i + ScrollLine) - 1;
                    string lineNumString = lineNum.ToString("n0") + ". ";
                    float gutter = GUI.skin.label.CalcSize(new GUIContent(lineNumString.Replace(' ', '_'))).x + GutterWidth;
                    string gutterString = lineNumString + Instance.LocalLogger.Entries[lineNum].Prefix;
                    string line = Instance.LocalLogger.Entries[lineNum].Body;
                    Vector2 lineSize = GUI.skin.label.CalcSize(new GUIContent(line));

                    MaxLogAreaWidth = Mathf.Max(lineSize.x + gutter, MaxLogAreaWidth);

                    GUI.Label(new Rect(new Vector2(0.0f, (LogAreaRect.height - i * lineHeight - ScrollbarHeight) + LineOffset), new Vector2(gutter, lineSize.y)), gutterString);
                    GUI.Label(new Rect(new Vector2(gutter, (LogAreaRect.height - i * lineHeight - ScrollbarHeight) + LineOffset), new Vector2(lineSize.x, lineSize.y)), line);
                    i++;
                }
                LastLineNum = bottomLineNum;
                GUI.EndScrollView(false);
            }

            public void CalculateMouse()
            {
                if (Event.current.isMouse && Event.current.button == 0)
                {
                    if (MouseDown && Event.current.type == EventType.MouseUp)
                    {
                        MouseDown = false;
                        Grabbing = false;
                        SizingX = false;
                        SizingY = false;
                    }
                    else if (!MouseDown && Event.current.type == EventType.MouseDown)
                    {
                        MouseDown = true;
                        MouseDownOrigin = MousePos;
                        MouseDownOffset = MousePos - WindowRect.position;

                        if (GrabBarRect.Contains(MousePos))
                        {
                            Grabbing = true;
                        }
                        else
                        {
                            if (SizeBarXRect.Contains(MousePos))
                            {
                                SizingX = true;
                            }
                            if (SizeBarYRect.Contains(MousePos))
                            {
                                SizingY = true;
                            }
                        }
                    }
                }

                if (WindowRect.Contains(MousePos) && Event.current.isScrollWheel)
                {
                    ScrollLine -= (int)Mathf.Sign(Event.current.delta.y);
                }

                if (Grabbing)
                {
                    WindowRect = new Rect(MousePos - MouseDownOffset, WindowRect.size);
                }
                else
                {
                    if (SizingX)
                    {
                        WindowRect = new Rect(WindowRect.position, new Vector2((MousePos.x - MouseDownOrigin.x) + MouseDownOffset.x, WindowRect.size.y));
                    }

                    if (SizingY)
                    {
                        WindowRect = new Rect(WindowRect.position, new Vector2(WindowRect.size.x, (MousePos.y - MouseDownOrigin.y) + MouseDownOffset.y));
                    }
                }
            }

            public void Open()
            {
                JustOpenedThisFrame = true;
            }

            public void Send()
            {
                if (!string.IsNullOrWhiteSpace(CurrentInput))
                {
                    ArgumentResult result = ParseCommand(CurrentInput, out Command cmd);
                    if (result == ArgumentResult.Success)
                    {
                        cmd.Prototype.Fire(cmd);
                    }
                    else
                    {
                        Log("Could not execute command: " + result.ToString(), LogLevel.Info);
                    }
                }

                LastCommand = CurrentInput;
                CurrentInput = "";
            }

            public void Update()
            {
                LineOffset = Mathf.Lerp(LineOffset, 0.0f, Time.deltaTime * 10.0f);

                WindowRect = WindowRect;
            }
        }
    }
}
