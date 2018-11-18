using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InsertarCabeceraDetalleXml.DAL
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int IdCabecera { get; set; }
    }
}