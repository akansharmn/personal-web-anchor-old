using System.Collections.Generic;

namespace Models
{
    public class Link
    {
        public string Rel { get; set; }

       public string Method { get; set; }
        
        public string link { get; set; }

    }

    public class Links
    {
        public IList<Link> links = new List<Link>();
    }
}