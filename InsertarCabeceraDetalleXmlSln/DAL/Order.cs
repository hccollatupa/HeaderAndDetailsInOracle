using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InsertarCabeceraDetalleXml.DAL
{
    public class Order
    {
        public int Id { get; set; }
        public string Fecha { get; set; }
        public List<OrderDetail> Details { get; set; }
    }
}