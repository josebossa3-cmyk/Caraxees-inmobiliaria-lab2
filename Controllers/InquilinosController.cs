using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace inmobiliaria.Controllers
{
    public class InquilinosController : Controller
    {
        private readonly InquilinoRepository _repo;

        public InquilinosController(InquilinoRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            var inquilinos = await _repo.ObtenerTodosAsync();
            return View(inquilinos);
        }

        //get inquilinos/details/5 <- id
        public async Task<IActionResult> Details(int id)
        {
            var inquilino = await _repo.ObtenerPorIdAsync(id);
            if (inquilino == null) return NotFound();
            return View(inquilino);
        }

        // get inquilinos/create
        public IActionResult Create()
        {
            return View();
        }

        // post inquilinos/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                await _repo.CrearAsync(inquilino);
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }

        // get inquilino edit
        public async Task<IActionResult> Edit(int id)
        {
            var inquilino = await _repo.ObtenerPorIdAsync(id);
            if (inquilino == null) return NotFound();
            return View(inquilino);
        }

        // get inquilinos/editpartial
        public async Task<IActionResult> EditPartial(int id)
        {
            var inquilino = await _repo.ObtenerPorIdAsync(id);
            if (inquilino == null) return NotFound();
            return PartialView("_EditPartial", inquilino);
        }

        //post inquilino edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Inquilino inquilino)
        {
            if (id != inquilino.Id) return NotFound();
            if (ModelState.IsValid)
            {
                await _repo.ActualizarAsync(inquilino);
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }

        //get inquilino delete
        public async Task<IActionResult> Delete(int id)
        {
            var inquilino = await _repo.ObtenerPorIdAsync(id);
            if (inquilino == null) return NotFound();
            return View(inquilino);
        }

        //post inquilino delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}