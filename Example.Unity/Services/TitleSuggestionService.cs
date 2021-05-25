using System;
using System.Collections.Generic;
using Example.CastleWindsor.Services;
using Example.Unity.Contracts;

namespace Example.Unity.Services
{
    /// <summary>
    /// This is a Kinda dummy service which suggests a Title from a movie based on posts content. Do not expect
    ///  this to work very seriously.
    /// </summary>
    public class TitleSuggestionService : ITitleSuggestionService
    {
        private readonly char[] _wordSplit =
        {
            ' ', '\t', '\r', '\n', ',', '.', '/', '\\', '\t',
            ':', ';', '?', '!', '&', '(', ')', '-', '+', '|'
        };

        public string SuggestTitleFor(string content)
        {
            content = content.ToLower();
            var allWords = content.Split(_wordSplit, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, int> ranks = RankWords(allWords);

            var chosenWords = HighestBin(ranks);

            var movieTitles = Resources.Movies.Split("\n", StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, int> titleRanks = RankTitles(movieTitles, chosenWords);

            var highestTitle = PickHighest(titleRanks);

            return highestTitle;
        }

        /// <summary>
        /// Looks the ranks as a weighted histogram, and chooses the words of bin which has largest sum of ranks.
        /// </summary>
        /// <param name="ranks"></param>
        /// <returns></returns>
        private List<string> HighestBin(Dictionary<string, int> ranks)
        {
            Dictionary<int, List<string>> reorder = new Dictionary<int, List<string>>();

            foreach (var keyValue in ranks)
            {
                if (!reorder.ContainsKey(keyValue.Value))
                {
                    reorder.Add(keyValue.Value, new List<string>());
                }

                reorder[keyValue.Value].Add(keyValue.Key);
            }

            List<string> highestList = new List<string>();

            int highestTotal = 0;

            foreach (var keyValue in reorder)
            {
                var total = keyValue.Key * keyValue.Value.Count;

                if (total > highestTotal)
                {
                    highestTotal = total;
                    highestList = keyValue.Value;
                }
            }

            return highestList;
        }

        /// <summary>
        /// For each Movie title, measures closeness between the list of words in the title and chosen words list.
        /// Then decreases the effect of string lengths by dividing the value over number of words in title. Then to
        /// avoid loosing details in casting, multiplies the result by 100 before casting it into int. 
        /// </summary>
        /// <param name="movieTitles"></param>
        /// <param name="chosenWords"></param>
        /// <returns></returns>
        private Dictionary<string, int> RankTitles(string[] movieTitles, List<string> chosenWords)
        {
            var counts = new Dictionary<string, int>();

            foreach (var title in movieTitles)
            {
                var titleWords = title.Split(_wordSplit, StringSplitOptions.RemoveEmptyEntries);

                var closeness = Closeness(titleWords, chosenWords);

                var rank = closeness * 100 / ((double) titleWords.Length);

                counts[title] = (int) rank;
            }

            return counts;
        }

        /// <summary>
        /// calculates a value representing how similar are to list of words by summing up the similarity of all pairs of
        /// words in both lists. It calculates similarity by normalizing the Levenshtein Distance of two words and then
        /// inverting this number (1 - distance). 
        /// </summary>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        private double Closeness(IEnumerable<string> list1, IEnumerable<string> list2)
        {
            double closeness = 0;

            foreach (var item1 in list1)
            {
                foreach (var item2 in list2)
                {
                    var distance = LevenshteinDistance.Calculate(item1, item2);
                    var distanceMax = Math.Max(item1.Length, item2.Length);
                    var similarity = 1 - ((double) distance) / ((double) distanceMax);
                    closeness += similarity;
                }
            }

            return closeness;
        }

        /// <summary>
        /// Finds the word with larger rank.
        /// </summary>
        /// <param name="ranks"></param>
        /// <returns></returns>
        private string PickHighest(Dictionary<string, int> ranks)
        {
            int highestRank = 0;
            string highestWord = "Title";

            foreach (var rank in ranks)
            {
                if (highestRank < rank.Value)
                {
                    highestRank = rank.Value;
                    highestWord = rank.Key;
                }
            }

            return highestWord;
        }

        /// <summary>
        /// This method ranks words of the list by counting how many times a word is appeared.
        /// Each time a word is seen, its rank incremented by amount of its length/3. this way very small
        /// words like (as,to,the,is,...) get lower rank in comparison with Nouns for example. 
        /// </summary>
        /// <param name="allWords"></param>
        /// <returns></returns>
        private Dictionary<string, int> RankWords(string[] allWords)
        {
            var counts = new Dictionary<string, int>();

            foreach (var word in allWords)
            {
                if (!counts.ContainsKey(word))
                {
                    counts.Add(word, 0);
                }

                counts[word] += word.Length / 3;
            }

            return counts;
        }
    }
}