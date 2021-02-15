using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomQuery.Models
{
    public class SavedQuery
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string SqlString { get; set; }
        public string QueryName { get; set; }
    }
}
