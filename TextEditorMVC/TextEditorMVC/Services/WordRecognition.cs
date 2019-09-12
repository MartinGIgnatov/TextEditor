using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextEditorMVC.CustomClasses;

namespace TextEditorMVC.Services
{
    public class WordRecognition : IWordRecognition
    {
        public Dictionary<string, SymbolBranch> WordTypeTrees { get; }

        public SymbolBranch WordTree { get; private set; }

        /// <summary>
        /// Initializes this instance of service and waits for all the Tries in the wordLoader to be loaded.
        /// </summary>
        /// <param name="wordLoader">An isntance of wordLoader to get the Tries for all words.</param>
        public WordRecognition(IWordLoader wordLoader)
        {
            while (!wordLoader.LoadingTask.IsCompleted) { }
            WordTypeTrees = wordLoader.WordTypeTrees;
            WordTree = wordLoader.WordTree;
        }

        /// <summary>
        /// Finds propositions for the word given.
        /// </summary>
        /// <param name="searchWord">The word to find propostions for.</param>
        /// <param name="propositionsMaxCount">The desired maximum number of propositions.</param>
        /// <param name="hasEndingSymbol">Determines if at the end of the word there should be a whitespace or a punctuation symbol with whitespace. True means there should be.</param>
        /// <returns>Retuns a list of word propositions derived from the given word.</returns>
        public List<WordProposition> FindWordPropositions(string searchWord, int propositionsMaxCount = 1, bool hasEndingSymbol = false)
        {
            var propositions = new List<WordProposition>();
            var levensteinZeros = new List<LevensteinZero>
            {
                new LevensteinZero(0,0,new List<LevensteinZero>())
            };

            FindClosestWords(searchWord.ToLower(), "", WordTree, levensteinZeros, propositions, propositionsMaxCount, hasEndingSymbol);
          
            return propositions;
        }

        /// <summary>
        /// Finds if the given word exists in the Trie of all words.
        /// </summary>
        /// <param name="word">The word to search for.</param>
        /// <returns>True id exists, false if not.</returns>
        public bool IsWord(string word)
        {
            var wordBuffer = word.ToLower();
            var currSymbolBranch = WordTree;
            while (wordBuffer.Length > 0)
            {
                var a = wordBuffer[0];
                if (currSymbolBranch.SymbolDictionery.ContainsKey(wordBuffer[0]))
                {
                    if (wordBuffer.Length == 1)
                    {
                        return currSymbolBranch.SymbolDictionery[wordBuffer[0]].IsWord;
                    }
                    else
                    {
                        currSymbolBranch = currSymbolBranch.SymbolDictionery[wordBuffer[0]];
                        wordBuffer = wordBuffer.Substring(1, wordBuffer.Length - 1);
                    }
                }
                else
                {
                    break;
                }

            }
            return false;
        }

        private void FindClosestWords(string searchWord, string currWord, SymbolBranch branch
        , List<LevensteinZero> levensteinZeros, List<WordProposition> propositions, int propositionsMaxCount, bool hasEndingSymbol)
        {
            if (searchWord.Length > currWord.Length && branch.SymbolDictionery.ContainsKey(searchWord[currWord.Length]))
            {
                currWord = currWord + searchWord[currWord.Length].ToString();

                var newZerosXCoordinates = FindNewZeros(currWord[currWord.Length - 1], searchWord);
                var addedZerosCount = 0;

                foreach (var xCoordinate in newZerosXCoordinates)
                {
                    addedZerosCount++;
                    levensteinZeros.Add(new LevensteinZero(xCoordinate, currWord.Length, levensteinZeros));
                }

                if (branch.SymbolDictionery[searchWord[currWord.Length - 1]].IsWord)
                {
                    if (hasEndingSymbol)
                    {
                        if (TryAddWithEndingSymbols(propositions, currWord, searchWord, levensteinZeros, propositionsMaxCount, branch.SymbolDictionery[searchWord[currWord.Length - 1]].Descriptions))
                        {
                            ;
                        }
                    }
                    else
                    {
                        if (TryAdd(propositions, currWord, searchWord, levensteinZeros, propositionsMaxCount, branch.SymbolDictionery[searchWord[currWord.Length - 1]].Descriptions))
                        {
                            ;
                        }
                    }
                }

                FindClosestWords(searchWord, currWord, branch.SymbolDictionery[searchWord[currWord.Length - 1]], levensteinZeros, propositions, propositionsMaxCount, hasEndingSymbol);

                for (int i = 0; i < addedZerosCount; i++)
                {
                    levensteinZeros[levensteinZeros.Count - 1].RemoveConnections();
                    levensteinZeros.RemoveAt(levensteinZeros.Count - 1);
                }

                currWord = currWord.Substring(0, currWord.Length - 1);

            }

            foreach (char letter in branch.SymbolDictionery.Keys)
            {
                if ((searchWord.Length > currWord.Length && letter != searchWord[currWord.Length]) || searchWord.Length <= currWord.Length)
                {
                    currWord = currWord + letter.ToString();

                    if (searchWord.Length > currWord.Length || propositions.Count == 0 || propositions[propositions.Count - 1].Length > Math.Max(currWord.Length, searchWord.Length) - Math.Min(currWord.Length, searchWord.Length))
                    {
                        var newZerosXCoordinates = FindNewZeros(letter, searchWord);
                        var addedZerosCount = 0;
                        foreach (var xCoordinate in newZerosXCoordinates)
                        {
                            addedZerosCount++;
                            levensteinZeros.Add(new LevensteinZero(xCoordinate, currWord.Length, levensteinZeros));
                        }

                        if (branch.SymbolDictionery[letter].IsWord)
                        {
                            if (hasEndingSymbol)
                            {
                                if (TryAddWithEndingSymbols(propositions, currWord, searchWord, levensteinZeros, propositionsMaxCount, branch.SymbolDictionery[letter].Descriptions))
                                {
                                    ;
                                }
                            }
                            else
                            {
                                if (TryAdd(propositions, currWord, searchWord, levensteinZeros, propositionsMaxCount, branch.SymbolDictionery[letter].Descriptions))
                                {
                                    ;
                                }
                            }
                        }

                        FindClosestWords(searchWord, currWord, branch.SymbolDictionery[letter], levensteinZeros, propositions, propositionsMaxCount, hasEndingSymbol);

                        for (int i = 0; i < addedZerosCount; i++)
                        {
                            levensteinZeros[levensteinZeros.Count - 1].RemoveConnections();
                            levensteinZeros.RemoveAt(levensteinZeros.Count - 1);
                        }
                    }

                    currWord = currWord.Substring(0, currWord.Length - 1);
                }
            }
        }

        private int FindLength(List<LevensteinZero> endingZeros, int endX, int endY)
        {
            int length = Math.Max(endX, endY);
            foreach (LevensteinZero zero in endingZeros)
            {
                if (zero.Length + Math.Max(endX - zero.XCoordinate, endY - zero.YCoordinate) < length)
                {
                    length = zero.Length + Math.Max(endX - zero.XCoordinate, endY - zero.YCoordinate);
                }
            }
            return length;
        }

        private List<int> FindNewZeros(char newLetter, string word)
        {
            var xCoordinates = new List<int>();

            for (int i = 0; i < word.Length; i++)
            {
                if (newLetter.Equals(word[i]))
                {
                    xCoordinates.Add(i + 1);
                }
            }

            return xCoordinates;
        }

        private bool TryAdd(List<WordProposition> propositions, string currWord, string searchWord, List<LevensteinZero> levensteinZeros, int propositionsMaxCount, List<string> descriptions)
        {
            var endingZeros = levensteinZeros[0].EndingZeros;

            var length = FindLength(endingZeros, searchWord.Length, currWord.Length);

            int index = -1;

            for (int i = propositions.Count - 1; i >= 0; i--)
            {
                try
                {
                    if (length < propositions[i].Length)
                    {
                        index = i;
                    }
                }
                catch
                {
                    break;
                }
            }

            if (index != -1)
            {
                propositions.Insert(index, new WordProposition
                {
                    ProposedWord = currWord,
                    Length = length,
                    Descriptions = descriptions
                });

                if (propositions.Count > propositionsMaxCount)
                {
                    propositions.RemoveAt(propositions.Count - 1);

                    if (propositions.Count > propositionsMaxCount)
                    {
                        throw new Exception($"List should not be bigger than {propositionsMaxCount}.");
                    }
                }

                return true;
            }
            else
            {
                if (propositions.Count < propositionsMaxCount)
                {
                    propositions.Add(new WordProposition
                    {
                        ProposedWord = currWord,
                        Length = length,
                        Descriptions = descriptions
                    });
                    return true;
                }

                return false;
            }
        }

        private bool TryAddWithEndingSymbols(List<WordProposition> propositions, string currWord, string searchWord,
            List<LevensteinZero> levensteinZeros, int propositionsMaxCount, List<string> descriptions)
        {
            bool hasAdded = false;

            var letters = new List<char> { '.', ',', ';', ':', '?', '!' };

            for (int i = 0; i <= letters.Count; i++)
            {
                var addedZerosSymbolCount = 0;
                var addedZerosSpaceCount = 0;
                if (i < letters.Count)
                {
                    currWord = currWord + letters[i].ToString();
                    var newZerosSymbolXCoordinates = FindNewZeros(letters[i], searchWord);

                    foreach (var xCoordinate in newZerosSymbolXCoordinates)
                    {
                        addedZerosSymbolCount++;
                        levensteinZeros.Add(new LevensteinZero(xCoordinate, currWord.Length, levensteinZeros));
                    }
                }

                currWord = currWord + ' '.ToString();
                var newZerosSpaceXCoordinates = FindNewZeros(' ', searchWord);

                foreach (var xCoordinate in newZerosSpaceXCoordinates)
                {
                    addedZerosSpaceCount++;
                    levensteinZeros.Add(new LevensteinZero(xCoordinate, currWord.Length, levensteinZeros));
                }

                hasAdded = TryAdd(propositions, currWord, searchWord, levensteinZeros, propositionsMaxCount, descriptions);

                for (int j = 0; j < addedZerosSpaceCount; j++)
                {
                    levensteinZeros[levensteinZeros.Count - 1].RemoveConnections();
                    levensteinZeros.RemoveAt(levensteinZeros.Count - 1);
                }

                currWord = currWord.Substring(0, currWord.Length - 1);

                if (i < letters.Count)
                {
                    for (int j = 0; j < addedZerosSymbolCount; j++)
                    {
                        levensteinZeros[levensteinZeros.Count - 1].RemoveConnections();
                        levensteinZeros.RemoveAt(levensteinZeros.Count - 1);
                    }
                    currWord = currWord.Substring(0, currWord.Length - 1);
                }
            }
            return hasAdded;
        }
    }
}
