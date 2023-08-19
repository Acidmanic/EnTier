using System.Collections.Generic;
using EnTier.Query;
using Xunit;

namespace Entier.Test.Unit
{
    public class SearchQueryTests
    {
        [Fact]
        public void SimilarSQueriesMustProduceSameHashes()
        {
            var sq1 = CreateQuery1();

            var sq2 = CreateQuery1();

            var hash1 = sq1.Hash();

            var hash2 = sq2.Hash();

            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void SimilarSQueriesWithDifferentOrdersMustProduceSameHashes()
        {
            var sq1 = CreateQuery1();

            var sq2 = CreateQuery1OrderAltered();

            var hash1 = sq1.Hash();

            var hash2 = sq2.Hash();

            Assert.Equal(hash1, hash2);
        }
        
        [Fact]
        public void DifferentSQueriesWithMustProduceDifferentHashes()
        {
            var sq1 = CreateQuery1();

            var sq2 = CreateQuery2();

            var hash1 = sq1.Hash();

            var hash2 = sq2.Hash();

            Assert.NotEqual(hash1, hash2);
        }


        private SearchQuery CreateQuery1()
        {
            var query = new SearchQuery();

            query.Add(new EvaluationItem
            {
                Key = "Name",
                EqualValues = new List<string> { "Mani", "Mona", "Mina", "Farshid" },
                EvaluationMethod = EvaluationMethods.Equal
            });

            query.Add(new EvaluationItem
            {
                Key = "Age",
                Maximum = "100",
                Minimum = "0",
                EvaluationMethod = EvaluationMethods.BetweenValues
            });

            query.Add(new EvaluationItem
            {
                Key = "Height",
                Minimum = "150",
                EvaluationMethod = EvaluationMethods.LargerThan
            });

            return query;
        }

        private SearchQuery CreateQuery1OrderAltered()
        {
            var query = new SearchQuery();


            query.Add(new EvaluationItem
            {
                Key = "Age",
                Maximum = "100",
                Minimum = "0",
                EvaluationMethod = EvaluationMethods.BetweenValues
            });

            query.Add(new EvaluationItem
            {
                Key = "Height",
                Minimum = "150",
                EvaluationMethod = EvaluationMethods.LargerThan
            });

            query.Add(new EvaluationItem
            {
                Key = "Name",
                EqualValues = new List<string> { "Mani", "Mona", "Mina", "Farshid" },
                EvaluationMethod = EvaluationMethods.Equal
            });


            return query;
        }

        private SearchQuery CreateQuery2()
        {
            var query = new SearchQuery();

            query.Add(new EvaluationItem
            {
                Key = "Brand",
                EqualValues = new List<string> { "Nokia", "Samsung", "Sony" },
                EvaluationMethod = EvaluationMethods.Equal
            });

            query.Add(new EvaluationItem
            {
                Key = "Rate",
                Minimum = "2.5",
                EvaluationMethod = EvaluationMethods.LargerThan
            });

            query.Add(new EvaluationItem
            {
                Key = "Battery",
                Minimum = "1500",
                EvaluationMethod = EvaluationMethods.LargerThan
            });

            return query;
        }
    }
}