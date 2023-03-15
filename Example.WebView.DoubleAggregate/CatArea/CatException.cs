using System;

namespace Example.WebView.DoubleAggregate.CatArea
{
    public class CatException : Exception
    {
        public CatException(Cat cat, string message = "") : base(ProcessMessage(cat, message))
        {
        }

        public CatException(string message) : base(message)
        {
        }

        private static string ProcessMessage(Cat cat, string message)
        {
            return $"Problem Occured Processing Tasks For Human: {cat.Name} with Id:{cat.Id}\n" +
                   message;
        }
    }
}