using System;
using System.Collections.Generic;
using System.Text;
using Dapper;

namespace ReadingFilesAndCreatingWordObjects
{
    public enum WordTypes
    {
        Noun,
        NounPlural,
        Pronoun,
        Adjective,
        Adverb,
        Conjuction,
        PastParticip,
        PresentParticip,
        Preposition,
        Prefix,
        VerbTransitive,
        VerbIntransitive,
        VerbalNoun,
        Imperative,
        Infinitive,
        Interjection,
        Specific,
        NotFound
    }

    [Table("Word")]
    public class WordStruct
    {
        [Required]
        public string Word { get; }

        [IgnoreInsert]
        public List<WordTypes> Types { get; }

        [Required]
        public string Description { get; }

        private List<WordTypeStringsCollection> TypeCollection = WordTypeStringsCollection.InitializeAll();

        public WordStruct(string word, List<string> types, string description)
        {
            Word = word;
            ;
            Types = TypeDetermination(types, word);
            Description = description;
        }

        private List<WordTypes> TypeDetermination(List<string> types, string word)
        {
            var wordTypes = new List<WordTypes>();

            if (types.Count == 0)
            {
                wordTypes.Add(WordTypes.Specific);
                return wordTypes;
            }

            bool HasFound = false;

            foreach (string type in types) {

                foreach(WordTypeStringsCollection collection in TypeCollection)
                {
                    HasFound = false;

                    if (collection.StringCollections.Contains(type.Trim()))
                    {
                        if (!wordTypes.Contains(collection.Type))
                        {
                            wordTypes.Add(collection.Type);
                        }
                        HasFound = true;
                        break;
                    }
                }

                if (type.Trim() == "i." && wordTypes.Contains(WordTypes.VerbTransitive))
                {
                    if (!wordTypes.Contains(WordTypes.VerbIntransitive))
                    {
                        wordTypes.Add(WordTypes.VerbIntransitive);
                    }

                    HasFound = true;
                }

                if (type.Trim() == "t." && wordTypes.Contains(WordTypes.VerbIntransitive))
                {
                    if (!wordTypes.Contains(WordTypes.VerbTransitive))
                    {
                        wordTypes.Add(WordTypes.VerbTransitive);
                    }

                    HasFound = true;
                }

                if(!HasFound)
                {
                    ;
                    _ = word;
                    _ = type;
                    if (!wordTypes.Contains(WordTypes.NotFound))
                    {
                        wordTypes.Add(WordTypes.NotFound);
                    }
                }
            }
            return wordTypes;
        }
    }
}
