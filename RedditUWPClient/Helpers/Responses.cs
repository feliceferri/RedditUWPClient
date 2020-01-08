using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditUWPClient.Helpers
{
    internal class Responses
    {

        internal class SingleParam<T>
        {
            public bool Success { get; set; }
            public Exception Error { get; set; }
            public T value { get; set; }
        }
    }
}
