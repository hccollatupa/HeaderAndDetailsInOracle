﻿using System.Web;
using System.Web.Mvc;

namespace InsertarCabeceraDetalleXml.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}