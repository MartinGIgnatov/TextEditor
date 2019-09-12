using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using TextEditorMVC.Models;
using TextEditorMVC.CustomClasses;

namespace TextEditorMVC.Services
{
    public class WordLoader : IWordLoader
    {
        private string _connectionString;

        public Task LoadingTask { get; }

        public int LongestWordLength { get; private set; }

        public Dictionary<string, SymbolBranch> WordTypeTrees { get; private set; }

        public SymbolBranch WordTree { get; private set; }

        /// <summary>
        /// Instantiates the service. Starts the task of downloading the words and putting them in the Trie of all words and the Tries of the word types. The task also finds the longest word in the dictionery.
        /// </summary>
        /// <param name="configuration">The configuration of the MVC, required to get the connection string for the database.</param>
        public WordLoader(IConfiguration configuration)
        {
#if DEBUG
            _connectionString = configuration.GetConnectionString("Development");
#else
            _connectionString = configuration.GetConnectionString("Production");
#endif
            ;
            string queryGetWordTypes = @"SELECT * FROM [WordType] WHERE NOT [name] = 'NotFound'";
            string queryGetNotFoundWordType = @"SELECT Id FROM [WordType] WHERE [name] = 'NotFound'";
            string queryGetWords = @"select * from [Word] order by Id";
            string queryGetWordWordTypes = @"select * from [Word_WordType] order by Id_Word";

            List<Word> words;
            List<WordType> wordTypes;
            List<WordWordType> wordWordTypes;
            Guid idNotFound;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                wordTypes = connection.Query<WordType>(queryGetWordTypes).ToList();
                words = connection.Query<Word>(queryGetWords).ToList();
                wordWordTypes = connection.Query<WordWordType>(queryGetWordWordTypes).ToList();
                idNotFound = connection.QueryFirst(queryGetNotFoundWordType).Id;
            }

            LoadingTask = Task.Run(() =>
            {
                LoadingTaskMethod(words, wordTypes, wordWordTypes, idNotFound);
            });
        }

        private void Add(SymbolBranch symbolBranch, string word, string description)
        {
            if (!symbolBranch.SymbolDictionery.ContainsKey(word[0]))
            {
                symbolBranch.SymbolDictionery.Add(word[0], new SymbolBranch(false));
            }

            if (word.Length == 1)
            {
                symbolBranch.SymbolDictionery[word[0]].IsWord = true;
                symbolBranch.SymbolDictionery[word[0]].Descriptions.Add(description);
            }

            else
            {
                Add(symbolBranch.SymbolDictionery[word[0]], word.Substring(1, word.Length - 1), description);
            }
        }

        private void LoadingTaskMethod(List<Word> words, List<WordType> wordTypes, List<WordWordType> wordWordTypes, Guid idNotFound)
        {
            var wordWordTypesIndex = 0;

            var longestPrefixLength = 0;
            var longestNonPrefixLength = 0;

            //Create the list for wordtypes containing the trees
            WordTypeTrees = new Dictionary<string, SymbolBranch>();
            foreach (var type in wordTypes)
            {
                WordTypeTrees.Add(type.Name, new SymbolBranch(false));
            }

            WordTree = new SymbolBranch(false);

            foreach (var word in words)
            {
                var currWordwordTypes = new List<WordWordType>();
                
                var a = word.Id;
                var b = wordWordTypes[wordWordTypesIndex].Id_Word;

                while (wordWordTypesIndex < wordWordTypes.Count && wordWordTypes[wordWordTypesIndex].Id_Word == word.Id)
                {
                    currWordwordTypes.Add(wordWordTypes[wordWordTypesIndex]);
                    wordWordTypesIndex++;
                }

                foreach (var idType in currWordwordTypes)
                {
                    if (idType.Id_WordType != idNotFound)
                    {
                        var type = wordTypes.Find(el =>
                        {
                            return el.Id.Equals(idType.Id_WordType);
                        }).Name;

                        if(type == "Prefix" && word.Name.Length> longestNonPrefixLength)
                        {
                            longestNonPrefixLength = word.Name.Length;
                        }
                        if(type != "Prefix" && word.Name.Length > longestPrefixLength)
                        {
                            longestPrefixLength = word.Name.Length;
                        }

                        Add(WordTypeTrees[type], word.Name.ToLower(), word.Description);
                        Add(WordTree, word.Name.ToLower(), word.Description);
                    }
                }
            }

            LongestWordLength = longestNonPrefixLength + longestPrefixLength;
        }
    }
}
