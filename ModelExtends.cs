using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace NAMESPACEPROYECT.Model
{
    public partial class NAME-OF-EDM_Entities : DbContext
    {
        public NAME-OF-EDM_Entities(string cnn)
            : base("name=" + cnn)
        {
        }
    }
}
