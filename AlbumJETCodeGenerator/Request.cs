using System;
using System.Collections.Generic;
using System.Text;

namespace AlbumJET
{
    public class Request
    {
        public string action { get; set; }
        public string nonce { get; set; }
        public string code { get; set; }
        public int id { get; set; }
        public int subcategory { get; set; }
    }
}
