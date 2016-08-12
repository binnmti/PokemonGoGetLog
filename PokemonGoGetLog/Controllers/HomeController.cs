﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PokemonGoGetLog.Models;

namespace PokemonGoGetLog.Controllers
{
    public class HomeController : Controller
    {
        private const string GoogleMapUrl = "http://maps.googleapis.com/maps/api/js?libraries=places&key=";
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public ActionResult Index(int? page)
        {
            //https://github.com/TroyGoode/PagedList
            var datas = _db.PokemonGetDatas.OrderBy(x => x.CreateDateTime);
            var pageNumber = page ?? 1;
            return View(datas.ToPagedList(pageNumber, 25));
        }

        // GET: PokemonGetDatas/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PokemonGetData pokemonGetData = await _db.PokemonGetDatas.FindAsync(id);
            if (pokemonGetData == null)
            {
                return HttpNotFound();
            }
            return View(pokemonGetData);
        }

        // GET: PokemonGetDatas/Create
        public ActionResult Create()
        {
            var pokemonGetData = new PokemonGetData
            {
                Pokemons = PokemonNames.Data.OrderBy(x => x).Select((val, idx) => new PokemonData
                {
                    Id = idx,
                    Name = val
                })
            };

            var appSettings = ConfigurationManager.AppSettings;
            var appKey = appSettings["GoogleMapAPI"];
            ViewBag.GoogleMapUrl = GoogleMapUrl + appKey;
            return View(pokemonGetData);
        }

        // POST: PokemonGetDatas/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、http://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PokemonGetDataId,PokemonName,Position,MapX,MapY,Cp,User,ImageUrl,CreateDateTime,UpdateDateTime")] PokemonGetData pokemonGetData)
        {
            pokemonGetData.Pokemons = PokemonNames.Data.OrderBy(x => x).Select((val, idx) => new PokemonData
            {
                Id = idx,
                Name = val
            });
            if (ModelState.IsValid)
            {
                _db.PokemonGetDatas.Add(pokemonGetData);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(pokemonGetData);
        }

        // GET: PokemonGetDatas/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PokemonGetData pokemonGetData = await _db.PokemonGetDatas.FindAsync(id);
            if (pokemonGetData == null)
            {
                return HttpNotFound();
            }
            return View(pokemonGetData);
        }

        // POST: PokemonGetDatas/Edit/5
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、http://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PokemonGetDataId,PokemonName,Position,MapX,MapY,Cp,User,ImageUrl,CreateDateTime,UpdateDateTime")] PokemonGetData pokemonGetData)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(pokemonGetData).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(pokemonGetData);
        }

        // GET: PokemonGetDatas/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PokemonGetData pokemonGetData = await _db.PokemonGetDatas.FindAsync(id);
            if (pokemonGetData == null)
            {
                return HttpNotFound();
            }
            return View(pokemonGetData);
        }

        // POST: PokemonGetDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            PokemonGetData pokemonGetData = await _db.PokemonGetDatas.FindAsync(id);
            _db.PokemonGetDatas.Remove(pokemonGetData);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}