using System;
using System.Runtime.Serialization;

namespace Example.WebView.DoubleAggregate.Human
{
    public class HumanException : Exception
    {
        public HumanException(Human human, string message = "") : base(ProcessMessage(human, message))
        {
        }

        public HumanException(string message) : base(message)
        {
        }

        private static string ProcessMessage(Human human, string message)
        {
            return $"Problem Occured Processing Tasks For Human: {human.Name} with Id:{human.Id}\n" +
                   message;
        }
    }
}