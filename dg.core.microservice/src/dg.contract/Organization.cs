using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dg.contract
{
    public class Organization
    {
        public string Name { get; set; }
        public string Suffix { get; set; }
        public string WebSiteUrl { get; set; }
        public string Email { get; set; }

        public Address Address { get; set;

            /*
             case "USA":
                if (value.ToString().EndsWith("LLC"))
                {
                    flag = true;
                }
                break;
            case "India":
                if (value.ToString().EndsWith("Ltd"))
                {
                    flag = true;
                }
                break;
            default:
                flag = true;
                break; 
             */
        }
    }
}
