using GenericRepository;
using Reuniones.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Reuniones.Pages
{
    public partial class dynamicPage : PageDynamic<ENTIDAD>
    {
        protected override void OnInit(EventArgs e)
        {
            base.Panel = this.PN_DYNAMIC_PAGE;
            base.OnInit(e);
        }
        
    }
}
