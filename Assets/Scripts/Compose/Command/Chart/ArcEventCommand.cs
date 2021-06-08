﻿using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Arcaoid.Gameplay.Chart;
using Arcaoid.Gameplay;
using Arcaoid.Compose.Editing;

namespace Arcaoid.Compose.Command
{
    public class AddArcEventCommand : ICommand
    {
        private readonly ArcEvent note = null;
        public AddArcEventCommand(ArcEvent note)
        {
            this.note = note;
        }
        public string Name
        {
            get
            {
                return "添加 Note";
            }
        }
        public void Do()
        {
            if (note is ArcTap)
            {
                ArcTapNoteManager.Instance.Add(note as ArcTap);
            }
            else if (note is ArcHold)
            {
                ArcHoldNoteManager.Instance.Add(note as ArcHold);
            }
            else if (note is ArcArc)
            {
                ArcArcManager.Instance.Add(note as ArcArc);
            }
            else if (note is ArcTiming)
            {
                ArcTimingManager.Instance.Add(note as ArcTiming);
            }
            /*switch (note)
            {
                case ArcTap note:
                    ArcTapNoteManager.Instance.Add(note);
                    break;
                case ArcHold note:
                    ArcHoldNoteManager.Instance.Add(note);
                    break;
                case ArcArc note:
                    ArcArcManager.Instance.Add(note);
                    break;
            }*/
        }
        public void Undo()
        {
            if (note is ArcNote)
            {
                AdeCursorManager.Instance.DeselectNote(note as ArcNote);
            }
            if (note is ArcTap)
            {
                ArcTapNoteManager.Instance.Remove(note as ArcTap);
            }
            else if (note is ArcHold)
            {
                ArcHoldNoteManager.Instance.Remove(note as ArcHold);
            }
            else if (note is ArcArc)
            {
                ArcArcManager.Instance.Remove(note as ArcArc);
            }
            else if (note is ArcTiming)
            {
                ArcTimingManager.Instance.Remove(note as ArcTiming);
            }
        }
    }
    public class RemoveArcEventCommand : ICommand
    {
        private readonly ArcEvent note = null;
        public RemoveArcEventCommand(ArcEvent note)
        {
            this.note = note;
        }
        public string Name
        {
            get
            {
                return "删除 Note";
            }
        }
        public void Do()
        {
            if (note is ArcNote)
            {
                AdeCursorManager.Instance.DeselectNote(note as ArcNote);
            }
            if (note is ArcTap)
            {
                ArcTapNoteManager.Instance.Remove(note as ArcTap);
            }
            else if (note is ArcHold)
            {
                ArcHoldNoteManager.Instance.Remove(note as ArcHold);
            }
            else if (note is ArcArc)
            {
                ArcArcManager.Instance.Remove(note as ArcArc);
            }
            else if (note is ArcTiming)
            {
                ArcTimingManager.Instance.Remove(note as ArcTiming);
            }
        }
        public void Undo()
        {
            if (note is ArcTap)
            {
                ArcTapNoteManager.Instance.Add(note as ArcTap);
            }
            else if (note is ArcHold)
            {
                ArcHoldNoteManager.Instance.Add(note as ArcHold);
            }
            else if (note is ArcArc)
            {
                ArcArcManager.Instance.Add(note as ArcArc);
            }
            else if (note is ArcTiming)
            {
                ArcTimingManager.Instance.Add(note as ArcTiming);
            }
        }
    }
    public class EditArcEventCommand : ICommand
    {
        private readonly ArcEvent note = null;
        private readonly ArcEvent oldValues, newValues;
        public EditArcEventCommand(ArcEvent note, ArcEvent newValues)
        {
            this.note = note;
            oldValues = note.Clone();
            this.newValues = newValues;
        }
        public string Name
        {
            get
            {
                return "修改 Note";
            }
        }
        public void Do()
        {
            (note as ArcArcTap)?.RemoveArcTapConnection();
            note.Assign(newValues);
            (note as ArcArcTap)?.Relocate();
            (note as ArcArc)?.Rebuild();
            (note as ArcTap)?.SetupArcTapConnection();
            if (note is ArcArc) ArcArcManager.Instance.CalculateArcRelationship();
            if (note is ArcTiming) AdeTimingEditor.Instance.BuildList();
        }
        public void Undo()
        {
            (note as ArcArcTap)?.RemoveArcTapConnection();
            note.Assign(oldValues);
            (note as ArcArcTap)?.Relocate();
            (note as ArcArc)?.Rebuild();
            (note as ArcTap)?.SetupArcTapConnection();
            if (note is ArcArc) ArcArcManager.Instance.CalculateArcRelationship();
            if (note is ArcTiming) AdeTimingEditor.Instance.BuildList();
        }
    }

    public class AddArcTapCommand : ICommand
    {
        private readonly ArcArc arc;
        private readonly ArcArcTap arctap;
        public AddArcTapCommand(ArcArc arc, ArcArcTap arctap)
        {
            this.arc = arc;
            this.arctap = arctap;
        }
        public string Name
        {
            get
            {
                return "添加 ArcTap";
            }
        }
        public void Do()
        {
            arc.AddArcTap(arctap);
        }
        public void Undo()
        {
            AdeCursorManager.Instance.DeselectNote(arctap);
            arc.RemoveArcTap(arctap);
        }
    }
    public class RemoveArcTapCommand : ICommand
    {
        private readonly ArcArc arc;
        private readonly ArcArcTap arctap;
        public RemoveArcTapCommand(ArcArc arc, ArcArcTap arctap)
        {
            this.arc = arc;
            this.arctap = arctap;
        }
        public string Name
        {
            get
            {
                return "删除 ArcTap";
            }
        }
        public void Do()
        {
            AdeCursorManager.Instance.DeselectNote(arctap);
            arc.RemoveArcTap(arctap);
        }
        public void Undo()
        {
            arc.AddArcTap(arctap);
        }
    }
}