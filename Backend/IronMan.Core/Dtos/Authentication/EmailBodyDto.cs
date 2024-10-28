using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronMan.Core.Dtos.Authentication
{
    public class EmailBodyDto
    {
        public string subject { get; set; }
        public string body { get; set; }
        public string link { get; set; }
        public string buttontext { get; set; }
    }
}
