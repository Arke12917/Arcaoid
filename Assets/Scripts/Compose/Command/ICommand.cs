using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Arcaoid.Compose.Command
{
    public interface ICommand
    {
        string Name
        {
            get;
        }
        void Do();
        void Undo();
    }
    public class BatchCommand : ICommand
    {
        public string Name { get; private set; }
        private readonly ICommand[] commands = null;
        public BatchCommand(ICommand[] commands, string description = "")
        {
            this.commands = commands;
            Name = $"{description}: ";
            Dictionary<string, int> commandTypes = new Dictionary<string, int>();
            foreach (var c in commands)
            {
                if (!commandTypes.ContainsKey(c.Name))
                {
                    commandTypes.Add(c.Name, 0);
                }
                commandTypes[c.Name]++;
            }
            string[] keys = commandTypes.Keys.ToArray();
            for (int i = 0; i < keys.Length; ++i)
            {
                string t = keys[i];
                int count = commandTypes[t];
                if (count >= 2)
                    Name += ($"{count} 次 {t}", $"{count} times {t}");
                else
                    Name += t;
                if (i != keys.Length - 1) Name += ", ";
            }
        }
        public void Do()
        {
            foreach (var c in commands)
            {
                c.Do();
            }
        }
        public void Undo()
        {
            foreach (var c in commands)
            {
                c.Undo();
            }
        }
    }
}