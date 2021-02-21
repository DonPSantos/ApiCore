using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCore.Api.Extensions
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public int ExpiracaoEmHoras { get; set; }
        public string Emissor { get; set; }
        public string ValidoEm { get; set; }
    }
}