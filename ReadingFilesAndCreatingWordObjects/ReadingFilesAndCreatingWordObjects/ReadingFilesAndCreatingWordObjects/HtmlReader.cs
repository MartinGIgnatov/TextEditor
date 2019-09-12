using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReadingFilesAndCreatingWordObjects
{
    public class HtmlReader : Reader
    {
        public HtmlReader(string path) : base(path)
        {

        }

        /// <summary>
        /// Read all words in the given html file and coverds the to WordStruct
        /// </summary>
        /// <returns>Returns a list of WordStruct</returns>
        public override List<WordStruct> ReadAll()
        {
            List<WordStruct> words = new List<WordStruct>();

            string content;

            //try
            //{
            using (StreamReader sr = new StreamReader(Path))
            {
                content = (sr.ReadToEnd());
            }

            words = FindAll(content);

            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("The process failed: {0}", e.ToString());
            //}


            return words;
        }

        private List<WordStruct> FindAll(string content)
        {
            List<WordStruct> words = new List<WordStruct>();
            int startIndex = 0, nextStartingIndex;
            string word, typeField, description;
            BoudryIndexes paragraphBoudry;

            while (true)
            {
                paragraphBoudry = FindBoudry(content, new BoudryIndexes(startIndex, content.Length - 1), "p", out nextStartingIndex);

                if (nextStartingIndex == -1)
                {
                    break;
                }

                //Console.WriteLine(content.Substring(paragraphBoudry.Start, paragraphBoudry.End - paragraphBoudry.Start + 1) + "\n\n\n");

                description = ClearElements(content.Substring(paragraphBoudry.Start, paragraphBoudry.End - paragraphBoudry.Start + 1)).Trim();

                word = FindSubContent(content, paragraphBoudry, "b").Trim();

                ;
                var b = content.Length;
                var a = content.Substring(paragraphBoudry.Start, paragraphBoudry.End - paragraphBoudry.Start + 1);

                typeField = FindSubContent(content, paragraphBoudry, "i");

                var types = TypeSeparator(typeField, word);

                words.Add(new WordStruct(word, types, description));

                startIndex = nextStartingIndex;
            }
            return words;
        }

        private BoudryIndexes FindBoudry(string content, BoudryIndexes boudry, string elementName, out int closingIndex)
        {
            var startingElement = "<" + elementName + ">";
            var endingElement = "</" + elementName + ">";
            int endIndex, startIndex;
            ;

            var a = content.Substring(boudry.Start, boudry.End - boudry.Start + 1);

            try
            {
                var startPattern = new Regex(startingElement);
                var endPattern = new Regex(endingElement);

                var start = startPattern.Match(content, boudry.Start, boudry.End - boudry.Start + 1);
                startIndex = start.Index + startingElement.Length;
                var end = endPattern.Match(content, startIndex);
                endIndex = end.Index - 1;
                if (start.Success == false || end.Success == false)
                {
                    closingIndex = -1;
                    return new BoudryIndexes(-1, -1);
                }
                //if( (startIndex>boudry.End || ) )
            }
            catch
            {
                closingIndex = -1;
                return new BoudryIndexes(-1, -1);
            }

            if (startIndex - startingElement.Length < 0 || endIndex + 1 < 0 || startIndex > endIndex)
            {
                closingIndex = -1;
                return new BoudryIndexes(-1, -1);
            }

            //var a = content.Substring(startIndex, endIndex - startIndex);
            ;

            closingIndex = endIndex + endingElement.Length;
            return new BoudryIndexes(startIndex, endIndex);
        }

        private string FindSubContent(string content, BoudryIndexes boudry, string elementName)
        {
            var startingElement = "<" + elementName + ">";
            var endingElement = "</" + elementName + ">";
            int end, start;


            var startPattern = new Regex(startingElement);
            var endPattern = new Regex(endingElement);

            start = startPattern.Match(content, boudry.Start, boudry.End - boudry.Start + 1).Index + startingElement.Length;
            end = endPattern.Match(content, start).Index;

            if (start < 0 || end < 0 || start > end)
            {
                return "";
            }

            return content.Substring(start, end - start);
        }

        private string ClearElements(string subContent)
        {
            string cleaned = subContent;

            var openingPattern = new Regex("<\\w*>");
            var openingMatch = openingPattern.Match(subContent);

            while (openingMatch.Success)
            {
                var closingString = "</" + (openingMatch.Value).Substring(1, openingMatch.Length - 2) + ">";
                var closingPattern = new Regex(closingString);
                var closeMatch = closingPattern.Match(cleaned);

                if (closeMatch.Success)
                {
                    cleaned = cleaned.Remove(openingMatch.Index, closeMatch.Index - openingMatch.Index + closeMatch.Length);
                }
                else
                {
                    throw new Exception($"The string given is improper : {subContent} \n derived to \n {cleaned}");
                }

                openingMatch = openingPattern.Match(cleaned);
            }

            var bracketsPattern = new Regex("\\(\\s*\\)");
            var brackets = bracketsPattern.Match(cleaned);

            while (brackets.Success)
            {
                cleaned = cleaned.Remove(brackets.Index, brackets.Length);
                brackets = bracketsPattern.Match(cleaned);
            }

            return cleaned;
        }

        private List<string> TypeSeparator(string type, string word)
        {
            var types = new List<string>();
            int prevIndex = 0;

            var patterns = new List<Regex>
            {
                new Regex("&amp;"),
                new Regex("/"),
                new Regex(","),
                new Regex("or")
            };

            //var ampesandPattern = new Regex("&amp;");
            //var linePattern = new Regex("/");
            //var commaPattern = new Regex(",");

            var matches = (from pattern in patterns select pattern.Match(type)).ToList();

            //var ampersand = ampesandPattern.Match(type);
            //var line = linePattern.Match(type);
            //var comma = commaPattern.Match(type);

            bool hasSeparator = false;
            int lowestIndex = -1;
            int currSeparatorLength = 0;
            string typeString;
            int matchIndex=0;

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    hasSeparator = true;
                    break;
                }
            }

            while (hasSeparator)
            {
                hasSeparator = false;

                //if (ampersand.Success && (!line.Success || line.Index > ampersand.Index) && (!comma.Success || comma.Index > ampersand.Index))
                //{
                //    typeString = type.Substring(prevIndex, ampersand.Index - prevIndex);
                //    if (typeString.Trim() != "")
                //    {
                //        types.Add(typeString);
                //    }
                //    prevIndex = ampersand.Index + ampersand.Length;
                //    ampersand = ampesandPattern.Match(type, prevIndex);
                //}


                for (int i = 0; i < matches.Count; i++)
                {
                    if (matches[i].Success && (lowestIndex == -1 || matches[i].Index < lowestIndex))
                    {
                        lowestIndex = matches[i].Index;
                        currSeparatorLength = matches[i].Length;
                        matchIndex = i;
                        hasSeparator = true;
                    }
                }
                if (hasSeparator)
                {
                    typeString = type.Substring(prevIndex, lowestIndex - prevIndex);
                    if (typeString.Trim() != "")
                    {
                        types.Add(typeString);
                    }
                    prevIndex = lowestIndex + currSeparatorLength;
                    try
                    {
                        matches[matchIndex] = patterns[matchIndex].Match(type, prevIndex + 1);
                    }
                    catch
                    {
                        ;
                        return types;
                    }
                }
                lowestIndex = -1;
            }

            typeString = type.Substring(prevIndex);
            if (typeString.Trim() != "")
            {
                types.Add(typeString);
            }

            return types;
        }
    }
}
