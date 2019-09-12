using System;
using System.Collections.Generic;
using System.Text;

namespace ReadingFilesAndCreatingWordObjects
{
    public class WordTypeStringsCollection
    {
        public WordTypes Type { get; set; }

        public List<string> StringCollections { get; set; }

        private WordTypeStringsCollection() { }

        public static List<WordTypeStringsCollection> InitializeAll()
        {
            var AllCollections = new List<WordTypeStringsCollection>();

            AllCollections.Add(new WordTypeStringsCollection
            {
                Type = WordTypes.Noun,
                StringCollections = new List<string>
                {
                    "n.",
                    "n./",
                    "n. sing.",
                    "sing.",
                    "n. fem."
                }
            });

            AllCollections.Add(new WordTypeStringsCollection
            {
                Type = WordTypes.NounPlural,
                StringCollections = new List<string>
                {
                    "n. pl.",
                    "n. pl",
                    "pl.",
                    "pl"
                }
            });

            AllCollections.Add(new WordTypeStringsCollection
            {
                Type = WordTypes.Pronoun,
                StringCollections = new List<string>
                {
                    "pron.",
                    "conj.",
                    "obj.",
                    "dat."
                }
            });

            AllCollections.Add(new WordTypeStringsCollection
            {
                Type = WordTypes.Adjective,
                StringCollections = new List<string>
                {
                    "a.",
                    "a",
                    "a/",
                    "adj.",
                    "superl.",
                    "a. superl."
                }
            });

            AllCollections.Add(new WordTypeStringsCollection
            {
                Type = WordTypes.VerbTransitive,
                StringCollections = new List<string>
                {
                    "v. t.",
                    "v."
                }
            });

            AllCollections.Add(new WordTypeStringsCollection
            {
                Type = WordTypes.VerbIntransitive,
                StringCollections = new List<string>
                {
                    "v. i."
                }
            });

            AllCollections.Add(new WordTypeStringsCollection
            {
                Type = WordTypes.Interjection,
                StringCollections = new List<string>
                {
                    "interj."
                }
            });

            AllCollections.Add(new WordTypeStringsCollection
            {
                Type = WordTypes.Adverb,
                StringCollections = new List<string>
                {
                    "adv."
                }
            });

            AllCollections.Add(new WordTypeStringsCollection
            {
                Type = WordTypes.Adverb,
                StringCollections = new List<string>
                {
                    "A prefix.",
                    "pref."
                }
            });

            AllCollections.Add(new WordTypeStringsCollection
            {
                Type = WordTypes.Preposition,
                StringCollections = new List<string>
                {
                    "prep.",
                    "p. a."
                }
            });

            AllCollections.Add(new WordTypeStringsCollection
            {
                Type = WordTypes.PastParticip,
                StringCollections = new List<string>
                {
                    "p. p."
                }
            });

            AllCollections.Add(new WordTypeStringsCollection
            {
                Type = WordTypes.Imperative,
                StringCollections = new List<string>
                {
                    "imp."
                }
            });

            AllCollections.Add(new WordTypeStringsCollection
            {
                Type = WordTypes.Infinitive,
                StringCollections = new List<string>
                {
                    "inf."
                }
            });

            AllCollections.Add(new WordTypeStringsCollection
            {
                Type = WordTypes.PresentParticip,
                StringCollections = new List<string>
                {
                    "p. pr."
                }
            });

            AllCollections.Add(new WordTypeStringsCollection
            {
                Type = WordTypes.VerbalNoun,
                StringCollections = new List<string>
                {
                    "vb. n."
                }
            });

            return AllCollections;
        }
    }
}
