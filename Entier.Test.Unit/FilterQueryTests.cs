using System.Collections.Generic;
using EnTier.Query;
using Xunit;

namespace Entier.Test.Unit
{
    public class FilterQueryTests
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


        private FilterQuery CreateQuery1()
        {
            var query = new FilterQuery();

            query.Add(new FilterItem
            {
                Key = "Name",
                EqualValues = new List<string> { "Mani", "Mona", "Mina", "Farshid" },
                EvaluationMethod = EvaluationMethods.Equal,
                ValueType = typeof(string)
            });

            query.Add(new FilterItem
            {
                Key = "Age",
                Maximum = "100",
                Minimum = "0",
                EvaluationMethod = EvaluationMethods.BetweenValues,
                ValueType = typeof(int)
            });

            query.Add(new FilterItem
            {
                Key = "Height",
                Minimum = "150",
                EvaluationMethod = EvaluationMethods.LargerThan,
                ValueType = typeof(int)
            });

            return query;
        }

        private FilterQuery CreateQuery1OrderAltered()
        {
            var query = new FilterQuery();


            query.Add(new FilterItem
            {
                Key = "Age",
                Maximum = "100",
                Minimum = "0",
                EvaluationMethod = EvaluationMethods.BetweenValues,
                ValueType = typeof(int)
            });

            query.Add(new FilterItem
            {
                Key = "Height",
                Minimum = "150",
                EvaluationMethod = EvaluationMethods.LargerThan,
                ValueType = typeof(int)
            });

            query.Add(new FilterItem
            {
                Key = "Name",
                EqualValues = new List<string> { "Mani", "Mona", "Mina", "Farshid" },
                EvaluationMethod = EvaluationMethods.Equal,
                ValueType = typeof(string)
            });


            return query;
        }

        private FilterQuery CreateQuery2()
        {
            var query = new FilterQuery();

            query.Add(new FilterItem
            {
                Key = "Brand",
                EqualValues = new List<string> { "Nokia", "Samsung", "Sony" },
                EvaluationMethod = EvaluationMethods.Equal,
                ValueType = typeof(string)
            });

            query.Add(new FilterItem
            {
                Key = "Rate",
                Minimum = "2.5",
                EvaluationMethod = EvaluationMethods.LargerThan,
                ValueType = typeof(double)
            });

            query.Add(new FilterItem
            {
                Key = "Battery",
                Minimum = "1500",
                EvaluationMethod = EvaluationMethods.LargerThan,
                ValueType = typeof(int)
            });

            return query;
        }
    }
}