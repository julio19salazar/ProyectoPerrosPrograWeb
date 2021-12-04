using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using U3RazasPerros.Areas.Admin.Models;
using U3RazasPerros.Models;

namespace U3RazasPerros.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RazasController : Controller
    {
        public perrosContext Context { get; }
        public IWebHostEnvironment Host { get; }
        public RazasController(perrosContext context, IWebHostEnvironment host)
        {
            Context = context;
            Host = host;
        }
        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Razas> r = Context.Razas.OrderBy(x => x.Nombre);
            return View(r);
        }
        [HttpGet]
        public IActionResult Agregar()
        {

            return View(new RazasIndexViewModel
            {
                Paises = Context.Paises.OrderBy(x => x.Nombre)
            });
        }
        [HttpPost]
        public IActionResult Agregar(RazasIndexViewModel vm, IFormFile foto)
        {
            if (string.IsNullOrWhiteSpace(vm.Razas.Nombre))
            {
                ModelState.AddModelError("", "El nombre de la raza está vacío");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (string.IsNullOrWhiteSpace(vm.Razas.OtrosNombres))
            {
                ModelState.AddModelError("", "Espesifique otros nombres, no pueden estar vacíos");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (string.IsNullOrWhiteSpace(vm.Razas.Descripcion))
            {
                ModelState.AddModelError("", "El campo descripción no puede estar vacío");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (vm.Razas.IdPais<= 0)
            {
                ModelState.AddModelError("", "Seleccione uno de los paises para continuar");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (vm.Razas.PesoMin <= 0 )
            {
                ModelState.AddModelError("", "El peso minimo no debería de estar vacío, y debe ser mayor que 0");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (vm.Razas.PesoMax <= 0)
            {
                ModelState.AddModelError("", "El peso maximo no debe de ir vacío, y debe ser mayor que 0");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (vm.Razas.PesoMin >= vm.Razas.PesoMax)
            {
                ModelState.AddModelError("", "El peso minimo es imposible que se mayor al peso máximo ");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (vm.Razas.AlturaMin <= 0)
            {
                ModelState.AddModelError("", "La altura minima no debe ir vacía y debe ser mayor a 0");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);

            }
            if (vm.Razas.AlturaMax <= 0)
            {
                ModelState.AddModelError("", "La altura Máxima no debe ir vacía y debe ser mayor a 0");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (vm.Razas.AlturaMin >= vm.Razas.AlturaMax)
            {
                ModelState.AddModelError("", "Es imposible que la altura minima sea la misma que la maxima o mayor");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (vm.Razas.EsperanzaVida <= 0)
            {
                ModelState.AddModelError("", "La esperanza de vida no debe ir vacía y debe ser mayor a 0");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (foto != null)
            {
                if (foto.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("", "Solo se permite la carga de archivos JPG");
                    vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                    return View(vm);
                }
                if (foto.Length > 1024 * 1024 * 5)
                {
                    ModelState.AddModelError("", "No se permite la carga de archivos mayores a 5MB");
                    vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                    return View(vm);
                }
            }
            Context.Add(vm.Razas);
            Context.SaveChanges();
            if (foto != null)
            {
                var path = Host.WebRootPath + "/imgs_perros/" + vm.Razas.Id + "_0.jpg";
                FileStream fs = new FileStream(path, FileMode.Create);
                foto.CopyTo(fs);
                fs.Close();
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Modificar(int id)
        {
            Razas r = Context.Razas.FirstOrDefault(x => x.Id == id);
            Caracteristicasfisicas c = Context.Caracteristicasfisicas.FirstOrDefault(x => x.Id == id);
            if (r == null)
            {
                return RedirectToAction("Index");
            }

            return View(new RazasIndexViewModel
            {
                Razas = r,
                Paises = Context.Paises.OrderBy(x => x.Nombre)
            });
        }
        public IActionResult Modificar(RazasIndexViewModel vm, IFormFile foto)
        {
            var r = Context.Razas.Include(x => x.Caracteristicasfisicas).FirstOrDefault(x => x.Id == vm.Razas.Id);
            if (r == null)
            {
                return RedirectToAction("Index");
            }
            if (string.IsNullOrWhiteSpace(vm.Razas.Nombre))
            {
                ModelState.AddModelError("", "El nombre de la raza está vacío");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (string.IsNullOrWhiteSpace(vm.Razas.OtrosNombres))
            {
                ModelState.AddModelError("", "Espesifique otros nombres, no pueden estar vacíos");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (string.IsNullOrWhiteSpace(vm.Razas.Descripcion))
            {
                ModelState.AddModelError("", "El campo descripción no puede estar vacío");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (vm.Razas.IdPais == 0)
            {
                ModelState.AddModelError("", "Seleccione uno de los paises para continuar");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (vm.Razas.PesoMin == 0)
            {
                ModelState.AddModelError("", "El peso minimo no debería de estar vacío, y debe ser mayor que 0");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (vm.Razas.PesoMax == 0)
            {
                ModelState.AddModelError("", "El peso maximo no debe de ir vacío, y debe ser mayor que 0");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (vm.Razas.PesoMin >= vm.Razas.PesoMax)
            {
                ModelState.AddModelError("", "El peso minimo es imposible que se mayor al peso máximo ");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (vm.Razas.AlturaMin == 0)
            {
                ModelState.AddModelError("", "La altura minima no debe ir vacía y debe ser mayor a 0");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);

            }
            if (vm.Razas.AlturaMax == 0)
            {
                ModelState.AddModelError("", "La altura Máxima no debe ir vacía y debe ser mayor a 0");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (vm.Razas.AlturaMin >= vm.Razas.AlturaMax)
            {
                ModelState.AddModelError("", "Es imposible que la altura minima sea la misma que la maxima o mayor");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (vm.Razas.EsperanzaVida == 0)
            {
                ModelState.AddModelError("", "La esperanza de vida no debe ir vacía y debe ser mayor a 0");
                vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                return View(vm);
            }
            if (foto != null)
            {
                if (foto.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("", "Solo se permite la carga de archivos JPG");
                    vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                    return View(vm);
                }
                if (foto.Length > 1024 * 1024 * 5)
                {
                    ModelState.AddModelError("", "No se permite la carga de archivos mayores a 5MB");
                    vm.Paises = Context.Paises.OrderBy(x => x.Nombre);
                    return View(vm);
                }
            }
            r.Nombre = vm.Razas.Nombre;
            r.OtrosNombres = vm.Razas.OtrosNombres;
            r.IdPais = vm.Razas.IdPais;
            r.PesoMin = vm.Razas.PesoMin;
            r.PesoMax = vm.Razas.PesoMax;
            r.AlturaMin = vm.Razas.AlturaMin;
            r.AlturaMax = vm.Razas.AlturaMax;
            r.EsperanzaVida = vm.Razas.EsperanzaVida;

            r.Caracteristicasfisicas.Cola = vm.Razas.Caracteristicasfisicas.Cola;
            r.Caracteristicasfisicas.Color = vm.Razas.Caracteristicasfisicas.Color;
            r.Caracteristicasfisicas.Hocico = vm.Razas.Caracteristicasfisicas.Hocico;
            r.Caracteristicasfisicas.Patas = vm.Razas.Caracteristicasfisicas.Patas;
            r.Caracteristicasfisicas.Pelo = vm.Razas.Caracteristicasfisicas.Pelo;

            Context.Update(r);
            Context.SaveChanges();
            if (foto != null)
            {
                var path = Host.WebRootPath + "/imgs_perros/" + vm.Razas.Id + "_0.jpg";
                FileStream fs = new FileStream(path, FileMode.Create);
                foto.CopyTo(fs);
                fs.Close();
            }
            return RedirectToAction("Index");
        }



        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            Razas r = Context.Razas.FirstOrDefault(x => x.Id == id);
            if (r == null)
            {
                return RedirectToAction("Index");
            }
            return View(r);
        }
        [HttpPost]
        public IActionResult Eliminar(Razas vm)
        {
            Razas r = Context.Razas.FirstOrDefault(x => x.Id == vm.Id);
            Caracteristicasfisicas c = Context.Caracteristicasfisicas.FirstOrDefault(x => x.Id == vm.Id);
            if (r == null || c == null)
            {
                ModelState.AddModelError("", "No se encontro la raza, puede que  no exista o que otro administrador la haya borrado");
                return View(vm);
            }

            Context.Remove(c);
            Context.Remove(r);
            Context.SaveChanges();

            string path = Host.WebRootPath + "/imgs_perros/" + r.Id + "_0.jpg";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            return RedirectToAction("Index");
        }


    }
}
