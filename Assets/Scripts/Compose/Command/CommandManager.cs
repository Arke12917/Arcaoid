using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Arcaoid.Compose.Command
{
    public class CommandManager : MonoBehaviour
    {
        public static CommandManager Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }

        private bool undoClickable, redoClickable;
        private bool UndoClickable
        {
            get
            {
                return undoClickable;
            }
            set
            {
                if (undoClickable != value)
                {
                    UndoButton.interactable = value;
                    undoClickable = value;
                }
            }
        }
        private bool RedoClickable
        {
            get
            {
                return redoClickable;
            }
            set
            {
                if (redoClickable != value)
                {
                    RedoButton.interactable = value;
                    redoClickable = value;
                }
            }
        }
        public Button UndoButton, RedoButton;

        private Stack<ICommand> undo = new Stack<ICommand>();
        private Stack<ICommand> redo = new Stack<ICommand>();

        private void Update()
        {
           /* if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.Z)) Undo();
                else if (Input.GetKeyDown(KeyCode.Y)) Redo();
            }
            UndoClickable = undo.Count != 0;
            RedoClickable = redo.Count != 0;*/
        }

        public void Add(ICommand command)
        {
            command.Do();
            undo.Push(command);
            redo.Clear();
        }
        public void Undo()
        {
            if (undo.Count == 0) return;
            ICommand cmd = undo.Pop();
            cmd.Undo();
            redo.Push(cmd);
        }
        public void Redo()
        {
            if (redo.Count == 0) return;
            ICommand cmd = redo.Pop();
            cmd.Do();
            undo.Push(cmd);
        }
    }
}
