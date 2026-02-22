using CaveDiver.Dialogue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaveDiver.Interfaces
{
    public interface IDialogueProvider
    {
        Task<string> GetResponseAsync(DialogueContext context);
    }
}
