using System;
using System.Linq;

namespace IruddBlog.Commands
{
    public class RandomIdGenerator : IRandomIdGenerator
    {
        private Random Random { get; }
        private readonly object randomLock = new object();
        private const string AllowedChars = "abcdefghijklmnopqrstuvwxyz0123456789";

        public RandomIdGenerator(int? seed) 
        {
            this.Random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        private char NextChar() 
        {
            lock(randomLock)
            {
                return AllowedChars[Random.Next(0, AllowedChars.Length)];
            }
        }

        public string GenerateId(int length) 
        {
            return new string(Enumerable.Range(1, length).Select(_ =>  NextChar()).ToArray());
        }

        public RandomIdGenerator() : this(null)
        {
            
        }
    }
}