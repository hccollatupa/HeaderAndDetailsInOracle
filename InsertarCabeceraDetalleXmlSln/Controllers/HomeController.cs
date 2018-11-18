using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InsertarCabeceraDetalleXml.DAL;
using InsertarCabeceraDetalleXml.Models;

namespace InsertarCabeceraDetalleXml.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            OrderViewModel order = new OrderViewModel();
            order.Fecha = "20/12/2016";

            List<OrderDetailViewModel> details = new List<OrderDetailViewModel>();
            details.Add(new OrderDetailViewModel() { ProductId = 1000 });
            details.Add(new OrderDetailViewModel() { ProductId = 6000 });
            details.Add(new OrderDetailViewModel() { ProductId = 3000 });
            order.Details = details;

            return View(order);
        }

        [HttpPost]
        public JsonResult Save(OrderViewModel model)
        {
            MyRepository repository = new MyRepository();
            Order order = new Order()
            {
                Id = model.Id,
                Fecha = model.Fecha,
                Details = new List<OrderDetail>()
            };

            model.Details.ForEach(x =>
            {
                order.Details.Add(new OrderDetail()
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    IdCabecera = x.IdCabecera
                });
            });

            string IdTransaction = repository.Insert(order);

            if (!String.IsNullOrEmpty(IdTransaction))
                return Json(new
                {
                    Error = false,
                    Value = new
                    {
                        Id = IdTransaction,
                        Message = "Proceso satisfactorio registrado con código " + IdTransaction
                    }
                },
                JsonRequestBehavior.AllowGet);

            return Json(new { Error = true, Message = "Ha ocurrido un error" }, JsonRequestBehavior.AllowGet);
        }
    }
}