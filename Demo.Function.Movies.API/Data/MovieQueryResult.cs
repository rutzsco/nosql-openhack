using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Function.Movies.API.Data
{
    public class QueryResult<T>
    {
        public QueryResult(T result, double requestChange)
        {
            Result = result;
            RequestChange = requestChange;
        }

        public T Result { get; set; }

        public double RequestChange { get; set; }
    }
}
