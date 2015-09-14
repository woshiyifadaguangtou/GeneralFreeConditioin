using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication3.Resources;

namespace WpfApplication3
{
    public class LocalString
    {
        private Resource1 resource = new Resource1();
        public Resource1 Resource {
            get { return resource; }
        }
    }
}
