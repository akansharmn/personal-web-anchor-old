using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public abstract class LinkedResource
    {
        public List<Link> Links { get; set; }

        public string HRef { get; set; }
    }
}
