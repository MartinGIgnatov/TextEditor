using System;
using System.Collections.Generic;
using System.Linq;
using TextEditorMVC.Models;
using TextEditorMVC.CustomClasses;
using System.Diagnostics;

namespace TextEditorMVC.Services
{
    public class TextRecognition : ITextRecognition
    {
        private IWordRecognition _recognition;

        /// <summary>
        /// Creates an instance of the given service and remembers an instance of the WordRecognition service.
        /// </summary>
        /// <param name="recognition">WordRecignition service.</param>
        public TextRecognition(IWordRecognition recognition)
        {
            _recognition = recognition;
        }

        /// <summary>
        /// Changes the given wrong word with the given proposition in the text.
        /// </summary>
        /// <param name="textForView">The instance of TextForView to apply the changes to.</param>
        /// <param name="erorWordIndex">The index of the wrong word.</param>
        /// <param name="propositionIndex">The index of the proposition for the wrong word.</param>
        /// <returns>The instance of the given TextForView after the applied changes.</returns>
        public TextForView ChangeWord(TextForView textForView, int erorWordIndex, int propositionIndex)
        {
            textForView = Process(textForView);

            textForView.Text = textForView.Text.Remove(textForView.WordPositions[textForView.ErrorPositions[erorWordIndex]].StartIndex,
                textForView.WordPositions[textForView.ErrorPositions[erorWordIndex]].EndIndex
                - textForView.WordPositions[textForView.ErrorPositions[erorWordIndex]].StartIndex + 1);

            textForView.Text = textForView.Text.Insert(textForView.WordPositions[textForView.ErrorPositions[erorWordIndex]].StartIndex, textForView.FixPropositions[erorWordIndex][propositionIndex]);

            return textForView;
        }

        /// <summary>
        /// Processes the text by finding all the wrong words in it and finds propositions for them. This information is remembered in the given TextForView.
        /// </summary>
        /// <param name="text">The instange of TextForView</param>
        /// <returns>The instance of the given TextForView after the added propositions.</returns>
        public TextForView Process(TextForView text)
        {
            if (text == null || text.Text == null)
            {
                return null;
            }

            InstanciateElementsIfNull(text);

            SplitText(text);

            for (int i = 0; i < text.WordPositions.Count; i++)
            {
                if (!_recognition.IsWord(text.Text.Substring(text.WordPositions[i].StartIndex, text.WordPositions[i].EndIndex - text.WordPositions[i].StartIndex + 1)))
                {
                    text.ErrorPositions.Add(i);
                }
            }

            foreach (int error in text.ErrorPositions)
            {
                var propositions = _recognition.FindWordPropositions(text.Text.Substring(text.WordPositions[error].StartIndex, text.WordPositions[error].EndIndex - text.WordPositions[error].StartIndex + 1), 10, false);
                var stringPropositions = propositions.Select(el => el.ProposedWord).ToList();
                text.FixPropositions.Add(stringPropositions);
            }

            return text;
        }

        private void InstanciateElementsIfNull(TextForView textForView)
        {
            if (textForView.FixPropositions == null)
            {
                textForView.FixPropositions = new List<List<string>>();
            }

            if (textForView.ErrorPositions == null)
            {
                textForView.ErrorPositions = new List<int>();
            }

            if (textForView.WordPositions == null)
            {
                textForView.WordPositions = new List<WordBoundry>();
            }
        }

        private void MinimizeDistance(string snippet)
        {
            var sequencePropositions = new List<List<List<WordProposition>>>();

            List<List<WordProposition>> buffer;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < snippet.Length; i++)
            {
                buffer = new List<List<WordProposition>>();
                for (int j = i; j < snippet.Length; j++)
                {
                    buffer.Add(_recognition.FindWordPropositions(snippet.Substring(i, j - i + 1), 5, true));
                }
                sequencePropositions.Add(buffer);
            }

            stopwatch.Stop();

        }

        private void SplitText(TextForView textForView)
        {
            var wordPositions = new List<WordBoundry>();

            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };

            var currStartIndex = 0;

            var check = false;

            for (int i = 0; i < textForView.Text.Length; i++)
            {
                foreach (var delimeter in delimiterChars)
                {
                    if (textForView.Text[i] == delimeter)
                    {
                        check = true;
                    }
                }

                if (!check)
                {
                    wordPositions.Add(new WordBoundry
                    {
                        StartIndex = i,
                        EndIndex = -1
                    });
                    break;
                }
            }

            for (int i = 0; i < textForView.Text.Length; i++)
            {
                foreach (var delimeter in delimiterChars)
                {
                    if (textForView.Text[i] == delimeter)
                    {
                        if (currStartIndex <= i - 1)
                        {
                            wordPositions[wordPositions.Count - 1].EndIndex = i - 1;
                        }
                        else
                        {
                            wordPositions.RemoveAt(wordPositions.Count - 1);
                        }
                        currStartIndex = i + 1;
                        wordPositions.Add(new WordBoundry
                        {
                            StartIndex = currStartIndex,
                            EndIndex = -1
                        });
                    }
                }
            }

            check = false;

            foreach (var delimeter in delimiterChars)
            {
                if (textForView.Text[textForView.Text.Length - 1] == delimeter)
                {
                    check = true;
                }
            }

            if (check)
            {
                wordPositions.RemoveAt(wordPositions.Count - 1);
            }
            else
            {
                wordPositions[wordPositions.Count - 1].EndIndex = textForView.Text.Length - 1;
            }

            foreach (var el in wordPositions)
            {
                if (el.EndIndex == -1)
                {
                    throw new Exception("There is unclosed word in the text.");
                }
            }

            textForView.WordPositions = wordPositions;
        }

    }
}
