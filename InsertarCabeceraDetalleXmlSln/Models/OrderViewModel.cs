using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InsertarCabeceraDetalleXml.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string Fecha { get; set; }
        public List<OrderDetailViewModel> Details { get; set; }
    }
}