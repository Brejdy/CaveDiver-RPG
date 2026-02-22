using CaveDiver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaveDiver.Dialogue
{
   public class DialogueContext
    {
        public Player Player { get; set; }
        public Companion Companion { get; set; }
        public string PlayerInput { get; set; }
    }
}
