using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InsertarCabeceraDetalleXml.Models
{
    public class OrderDetailViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int IdCabecera { get; set; }
    }
}