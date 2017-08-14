using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Twitter.Models
{
    public class TwitterMedia : Tweet
    {   
        public string Url { get; set; }
        public string MediaUrl { get; set; }

    }
}
