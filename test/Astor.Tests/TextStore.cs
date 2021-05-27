using System;
using System.Collections.Generic;

namespace Astor.Tests
{
    public class TextStore
    {
        public string TextOne { get; set; }
        
        public string TextTwo { get; set; }
        
        public string TextThree { get; set; }

        public List<string> Messages { get; } = new List<string>();

        public void WriteMessagesToConsole()
        {
            Console.WriteLine("Messages are:");

            foreach (var message in this.Messages)
            {
                Console.WriteLine(message);
            }
        }
    }
}