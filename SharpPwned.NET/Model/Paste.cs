using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPwned.NET.Model
{
    public class Paste
    {
        public string Source { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public int EmailCount { get; set; }
    }
}
