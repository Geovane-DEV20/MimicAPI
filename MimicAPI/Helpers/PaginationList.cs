using System.Collections.Generic;
using System;

namespace MimicAPI.Helpers
{
    public class PaginationList<T> : List<T>
    {
        public Paginacao Paginacao { get; set; }

    }
}
