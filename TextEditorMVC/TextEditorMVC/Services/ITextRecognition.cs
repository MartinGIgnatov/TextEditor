using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextEditorMVC.Models;

namespace TextEditorMVC.Services
{
    public interface ITextRecognition
    {
        TextForView ChangeWord(TextForView textForView, int erorWordIndex, int propositionIndex);

        TextForView Process(TextForView text);
        
    }
}
