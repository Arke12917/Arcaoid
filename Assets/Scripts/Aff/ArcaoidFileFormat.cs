﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using Arcaoid.Aff.Advanced;
using Arcaoid.Gameplay;

namespace Arcaoid.Aff
{
    public class ArcaoidAffFormatException : Exception
    {
        public string Reason;
        public ArcaoidAffFormatException(string reason)
        {
            Reason = reason;
        }
        public ArcaoidAffFormatException(string content, int line)
               : base($"在第 {line} 行发生读取错误。\n{content}")
        {

        }
        public ArcaoidAffFormatException(EventType type, string content, int line)
            : base($"在第 {line} 行处的 {type} 型事件中发生读取错误。\n{content}")
        {

        }
        public ArcaoidAffFormatException(EventType type, string content, int line, string reason)
            : base($"在第 {line} 行处的 {type} 型事件中发生读取错误。\n{content}\n{reason}")
        {

        }
    }
    public enum EventType
    {
        Timing,
        Tap,
        Hold,
        Arc,
        Camera,
        Unknown,
        Special,
        ArcHold
    }
    public class ArcaoidAffEvent
    {
        public int Timing;
        public EventType Type;
    }
    public class ArcaoidAffTiming : ArcaoidAffEvent
    {
        public float Bpm;
        public float BeatsPerLine;
    }
    public class ArcaoidAffTap : ArcaoidAffEvent
    {
        public int Track;
    }
    public class ArcaoidAffHold : ArcaoidAffEvent
    {
        public int EndTiming;
        public int Track;
    }
    public class ArcaoidAffArc : ArcaoidAffEvent
    {
        public int EndTiming;
        public float XStart;
        public float XEnd;
        public string LineType;
        public float YStart;
        public float YEnd;
        public int Color;
        public bool IsVoid;
        public List<int> ArcTaps;
    }
    public class ArcaoidAffCamera : ArcaoidAffEvent
    {
        public Vector3 Move, Rotate;
        public string CameraType;
        public int Duration;
    }
    namespace Advanced
    {
        public enum SpecialType
        {
            Unknown,
            TextArea,
            Fade
        }
        public class ArcaoidAffSpecial : ArcaoidAffEvent
        {
            public SpecialType SpecialType;
            public string param1;
            public string param2;
            public string param3;
        }
    }
    public class ArcaoidAffArcHold : ArcaoidAffEvent
    {
        public int EndTiming;
        public float XStart;
        public float XEnd;
        public string LineType;
        public float YStart;
        public float YEnd;
        public int Color;       
    }
    public class ArcaoidAffReader
    {
        public int AudioOffset;
        public List<ArcaoidAffEvent> Events = new List<ArcaoidAffEvent>();
        public ArcaoidAffReader(string path)
        {
            Parse(path);
        }
        private EventType DetermineType(string line)
        {
            if (line.StartsWith("(")) return EventType.Tap;
            else if (line.StartsWith("timing")) return EventType.Timing;
            else if (line.StartsWith("hold")) return EventType.Hold;
            else if (line.StartsWith("archold")) return EventType.ArcHold;
            else if (line.StartsWith("arc")) return EventType.Arc;
            else if (line.StartsWith("camera")) return EventType.Camera;
            else if (line.StartsWith("special")) return EventType.Special;          
            return EventType.Unknown;
        }
        private void ParseTiming(string line)
        {
            try
            {
                StringParser s = new StringParser(line);
                s.Skip(7);
                int tick = s.ReadInt(",");
                float bpm = s.ReadFloat(",");
                float beatsPerLine = s.ReadFloat(")");
                Events.Add(new ArcaoidAffTiming()
                {
                    Timing = tick,
                    BeatsPerLine = beatsPerLine,
                    Bpm = bpm,
                    Type = EventType.Timing
                });
                if (Mathf.Abs(bpm) > 1000000) throw new ArcaoidAffFormatException("bpm too large");
                if (beatsPerLine < 0) throw new ArcaoidAffFormatException("Beats per line is less than 0");
            }
            catch (ArcaoidAffFormatException Ex)
            {
                throw Ex;
            }
            catch (Exception)
            {
                throw new ArcaoidAffFormatException("符号错误");
            }
        }
        private void ParseTap(string line)
        {
            try
            {
                StringParser s = new StringParser(line);
                s.Skip(1);
                int tick = s.ReadInt(",");
                int track = s.ReadInt(")");
                if (ArcScoreManager.ismirrored)
                    track = 5 - track;
                else { }                
                Events.Add(new ArcaoidAffTap()
                {
                    Timing = tick,
                    Track = track,
                    Type = EventType.Tap
                });
                
                if (track <= 0 || track >= 5) throw new ArcaoidAffFormatException("轨道错误");
            }
            catch (ArcaoidAffFormatException Ex)
            {
                throw Ex;
            }
            catch (Exception)
            {
                throw new ArcaoidAffFormatException("符号错误");
            }
        }
        private void ParseHold(string line)
        {
            try
            {
                StringParser s = new StringParser(line);
                s.Skip(5);
                int tick = s.ReadInt(",");
                int endtick = s.ReadInt(",");
                int track = s.ReadInt(")");
                if (ArcScoreManager.ismirrored)
                    track = 5 - track;
                else { }
                Events.Add(new ArcaoidAffHold()
                {
                    Timing = tick,
                    EndTiming = endtick,
                    Track = track,
                    Type = EventType.Hold
                });
                if (track <= 0 || track >= 5) throw new ArcaoidAffFormatException("轨道错误");
                if (endtick < tick) throw new ArcaoidAffFormatException("持续时间小于0");
            }
            catch (ArcaoidAffFormatException Ex)
            {
                throw Ex;
            }
            catch (Exception)
            {
                throw new ArcaoidAffFormatException("符号错误");
            }
        }
        private void ParseArc(string line)
        {
            try
            {
                StringParser s = new StringParser(line);
                s.Skip(4);
                int tick = s.ReadInt(",");
                int endtick = s.ReadInt(",");
                float startx = s.ReadFloat(",");
                float endx = s.ReadFloat(",");
                string linetype = s.ReadString(",");
                float starty = s.ReadFloat(",");
                float endy = s.ReadFloat(",");
                int color = s.ReadInt(",");
                s.ReadString(",");
                bool isvoid = s.ReadBool(")");
                List<int> arctap = null;
                if (s.Current != ";")
                {
                    arctap = new List<int>();
                    isvoid = true;
                    while (true)
                    {
                        s.Skip(8);
                        arctap.Add(s.ReadInt(")"));
                        if (s.Current != ",") break;
                    }
                }
                if (ArcScoreManager.ismirrored)
                {
                    startx = 1 - startx;
                    endx = 1 - endx;
                    if(!isvoid&&color>1==false)
                    color = 1 - color;
                }
                else { }
                Events.Add(new ArcaoidAffArc()
                {
                    Timing = tick,
                    EndTiming = endtick,
                    XStart = startx,
                    XEnd = endx,
                    LineType = linetype,
                    YStart = starty,
                    YEnd = endy,
                    Color = color,
                    IsVoid = isvoid,
                    Type = EventType.Arc,
                    ArcTaps = arctap
                });
                if (endtick < tick) throw new ArcaoidAffFormatException("持续时间小于0");
            }
            catch (ArcaoidAffFormatException Ex)
            {
                throw Ex;
            }
            catch (Exception)
            {
                throw new ArcaoidAffFormatException("符号错误");
            }
        }
        private void ParseArcHold(string line)
        {
            try
            {
                StringParser s = new StringParser(line);
                s.Skip(8);
                int tick = s.ReadInt(",");
                int endtick = s.ReadInt(",");
                float startx = s.ReadFloat(",");
                float endx = s.ReadFloat(",");
                float starty = s.ReadFloat(",");
                float endy = s.ReadFloat(",");
                int color = s.ReadInt(")");
                if (ArcScoreManager.ismirrored)
                {
                    startx = 1 - startx;
                    if (color > 1 == false)
                        color = 1 - color;
                }
                else { }
                Events.Add(new ArcaoidAffArcHold()
                {
                    Timing = tick,
                    EndTiming = endtick,
                    XStart = startx,
                    XEnd = endx,
                    LineType = "b",
                    YStart = starty,
                    YEnd = endy,
                    Color = color,
                    Type = EventType.ArcHold
                });
                if (endtick < tick) throw new ArcaoidAffFormatException("持续时间小于0");
            }
            catch
            {

            }
            
        }
        private void ParseCamera(string line)
        {
            try
            {
                StringParser s = new StringParser(line);
                s.Skip(7);
                int tick = s.ReadInt(",");
                Vector3 move = new Vector3(s.ReadFloat(","), s.ReadFloat(","), s.ReadFloat(","));
                Vector3 rotate = new Vector3(s.ReadFloat(","), s.ReadFloat(","), s.ReadFloat(","));
                if (ArcScoreManager.ismirrored)
                {
                    move = new Vector3(-move.x, move.y, move.z);
                    rotate = new Vector3(-rotate.x, rotate.y, -rotate.z);
                }
                else { }
                string type = s.ReadString(",");
                int duration = s.ReadInt(")");
                Events.Add(new ArcaoidAffCamera()
                {
                    Timing = tick,
                    Duration = duration,
                    Move = move,
                    Rotate = rotate,
                    CameraType = type,
                    Type = EventType.Camera,
                });
                if (duration < 0) throw new ArcaoidAffFormatException("持续时间小于0");
            }
            catch (ArcaoidAffFormatException Ex)
            {
                throw Ex;
            }
            catch (Exception)
            {
                throw new ArcaoidAffFormatException("符号错误 Symbol Error.");
            }
        }
        private void ParseSpecial(string line)
        {
            try
            {
                StringParser s = new StringParser(line);
                s.Skip(8);
                int tick = s.ReadInt(",");
                string type = s.ReadString(",");
                SpecialType specialType = SpecialType.Unknown;
                string param1 = null, param2 = null, param3 = null;
                switch (type)
                {
                    case "text":
                        specialType = SpecialType.TextArea;
                        param1 = s.Peek(2);
                        if (param1 == "in")
                        {
                            param1 = s.ReadString(",");
                            param2 = s.ReadString(")");
                            param2 = param2.Replace("<br>", "\n");
                        }
                        else
                        {
                            param1 = s.ReadString(")");
                        }
                        break;
                    case "fade":
                        specialType = SpecialType.Fade;
                        param1 = s.ReadString(")");
                        break;
                }
                Events.Add(new ArcaoidAffSpecial()
                {
                    Timing = tick,
                    Type = EventType.Special,
                    param1 = param1,
                    param2 = param2,
                    param3 = param3,
                    SpecialType = specialType
                });
            }
            catch (ArcaoidAffFormatException Ex)
            {
                throw Ex;
            }
            catch (Exception)
            {
                throw new ArcaoidAffFormatException(("符号错误 Symbol Error."));
            }
        }
        public void Parse(string path)
        {
            string[] lines = File.ReadAllLines(path);
            try
            {
                AudioOffset = int.Parse(lines[0].Replace("AudioOffset:", ""));
            }
            catch (Exception)
            {
                throw new ArcaoidAffFormatException(lines[0], 1);
            }
            try
            {
                ParseTiming(lines[2]);
            }
            catch (Exception)
            {
                throw new ArcaoidAffFormatException(EventType.Timing, lines[2], 3);
            }
            for (int i = 3; i < lines.Length; ++i)
            {
                string line = lines[i];
                EventType type = DetermineType(line);
                try
                {
                    switch (type)
                    {
                        case EventType.Timing:
                            ParseTiming(line);
                            break;
                        case EventType.Tap:
                            ParseTap(line);
                            break;
                        case EventType.Hold:
                            ParseHold(line);
                            break;
                        case EventType.Arc:
                            ParseArc(line);
                            break;
                        case EventType.Camera:
                            ParseCamera(line);
                            break;
                        case EventType.ArcHold:
                            ParseArcHold(line);
                            break;

                    }
                }
                catch (ArcaoidAffFormatException Ex)
                {
                    throw new ArcaoidAffFormatException(type, line, i + 1, Ex.Reason);
                }
                catch (Exception)
                {
                    throw new ArcaoidAffFormatException(type, line, i + 1);
                }
            }
            Events.Sort((ArcaoidAffEvent a, ArcaoidAffEvent b) => { return a.Timing.CompareTo(b.Timing); });
        }
    }
    public class ArcaoidAffWriter : StreamWriter
    {
        public ArcaoidAffWriter(Stream stream, int audioOffset) : base(stream)
        {
            WriteLine($"AudioOffset:{audioOffset}");
            WriteLine("-");
        }
        public void WriteEvent(ArcaoidAffEvent affEvent)
        {
            switch (affEvent.Type)
            {
                case EventType.Timing:
                    ArcaoidAffTiming timing = affEvent as ArcaoidAffTiming;
                    WriteLine($"timing({timing.Timing},{timing.Bpm.ToString("f2")},{timing.BeatsPerLine.ToString("f2")});");
                    break;
                case EventType.Tap:
                    ArcaoidAffTap tap = affEvent as ArcaoidAffTap;
                    WriteLine($"({tap.Timing},{tap.Track});");
                    break;
                case EventType.Hold:
                    ArcaoidAffHold hold = affEvent as ArcaoidAffHold;
                    WriteLine($"hold({hold.Timing},{hold.EndTiming},{hold.Track});");
                    break;
                case EventType.Arc:
                    ArcaoidAffArc arc = affEvent as ArcaoidAffArc;
                    string arcStr = $"arc({arc.Timing},{arc.EndTiming},{arc.XStart.ToString("f2")},{arc.XEnd.ToString("f2")}";
                    arcStr += $",{arc.LineType},{arc.YStart.ToString("f2")},{arc.YEnd.ToString("f2")},{arc.Color},none,{((arc.ArcTaps == null || arc.ArcTaps.Count == 0) ? arc.IsVoid.ToString().ToLower() : "true")})";
                    if (arc.ArcTaps != null && arc.ArcTaps.Count != 0)
                    {
                        arcStr += "[";
                        for (int i = 0; i < arc.ArcTaps.Count; ++i)
                        {
                            arcStr += $"arctap({arc.ArcTaps[i]})";
                            if (i != arc.ArcTaps.Count - 1) arcStr += ",";
                        }
                        arcStr += "]";
                    }
                    arcStr += ";";
                    WriteLine(arcStr);
                    break;
                case EventType.Camera:
                    ArcaoidAffCamera cam = affEvent as ArcaoidAffCamera;
                    WriteLine($"camera({cam.Timing},{cam.Move.x},{cam.Move.y},{cam.Move.z},{cam.Rotate.x},{cam.Rotate.y},{cam.Rotate.z},{cam.CameraType},{cam.Duration});");
                    break;
                case EventType.Special:
                    ArcaoidAffSpecial spe = affEvent as ArcaoidAffSpecial;
                    string type = "error";
                    switch (spe.SpecialType)
                    {
                        case SpecialType.Fade:
                            type = "fade";
                            WriteLine($"special({spe.Timing},{type},{spe.param1 ?? "null"});");
                            break;
                        case SpecialType.TextArea:
                            type = "text";
                            if (spe.param1 == "in")
                                WriteLine($"special({spe.Timing},{type},{spe.param1 ?? "null"},{(spe.param2 == null ? "null" : spe.param2.Replace("\n", "<br>"))});");
                            else
                                WriteLine($"special({spe.Timing},{type},{spe.param1 ?? "null"});");
                            break;
                    }
                    break;
            }
        }
    }
}