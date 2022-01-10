using System;

namespace Pathfinder
{
    class ShouldNotHappenException : Exception
    {
        public ShouldNotHappenException()
        {

        }

        public ShouldNotHappenException(string message)
            : base(message)
        {

        }

        public ShouldNotHappenException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
