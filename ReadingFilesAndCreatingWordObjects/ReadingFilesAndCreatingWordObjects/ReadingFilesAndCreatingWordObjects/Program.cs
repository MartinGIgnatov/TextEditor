using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

namespace ReadingFilesAndCreatingWordObjects
{
    class Program
    {
        static void Main(string[] args)
        {
            var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            var words = new List<WordStruct>();

            foreach (char letter in alphabet)
            {
                var htmsReader = new HtmlReader($"E:\\REPO\\Interns2019\\TextEditor\\htmlDictionaryForWords\\OPTED v0.03 Letter {letter}.html");
                words.AddRange(htmsReader.ReadAll());
            }
            
            ;

            var Inserter = new InserterWordToDatabase();

            Inserter.Insert(words);

        }
    }
}
