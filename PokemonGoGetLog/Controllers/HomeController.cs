using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
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
        private const string GoogleMapUrl = "https://maps-api-ssl.google.com/maps/api/js?sensor=true&libraries=places&key=";
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        //ToDo ページ機能動いていない。。。
        public ActionResult Index(int? page)
        {
            //https://github.com/TroyGoode/PagedList
            var srcDatas = _db.PokemonGetDatas.Join(_db.PokemonDatas, p => p.PokemonName, c => c.ImageName,
                (p, c) => new
                {
                    p.PokemonName,
                    c.Name,
                    p.Position,
                    p.Cp,
                    p.User,
                    p.CreateDateTime
                });

            var dstDatas = new List<PokemonGetData>();
            foreach (var srcData in srcDatas)
            {
                dstDatas.Add(new PokemonGetData
                {
                    PokemonName = srcData.Name,
                    PokemonImageName = srcData.PokemonName,
                    Position = srcData.Position,
                    Cp = srcData.Cp,
                    User = srcData.User,
                    CreateDateTime = srcData.CreateDateTime,
                });
            }
            var datas = dstDatas.OrderBy(x => x.CreateDateTime);
            var pageNumber = page ?? 1;
            return View(datas.ToPagedList(pageNumber, 25));
        }

#if DEBUG
#else
        [RequireHttps]
#endif
        // GET: PokemonGetDatas/Create
        public ActionResult Create()
        {
            var pokemonGetData = new PokemonGetData {Pokemons = _db.PokemonDatas};
            var appSettings = ConfigurationManager.AppSettings;
            var appKey = appSettings["GoogleMapAPI"];
            ViewBag.GoogleMapUrl = GoogleMapUrl + appKey;
            ViewBag.User = string.IsNullOrEmpty(GetCookie("User")) ? string.Empty : GetCookie("User");
            return View(pokemonGetData);
        }

        // POST: PokemonGetDatas/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、http://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
#if DEBUG
#else
        [RequireHttps]
#endif
        public async Task<ActionResult> Create([Bind(Include = "PokemonGetDataId,PokemonName,Position,MapX,MapY,Cp,User,ImageUrl,CreateDateTime,UpdateDateTime")] PokemonGetData pokemonGetData)
        {
            pokemonGetData.Pokemons = _db.PokemonDatas;
            if (ModelState.IsValid)
            {
                SetCookie("User", pokemonGetData.User);
                _db.PokemonGetDatas.Add(pokemonGetData);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(pokemonGetData);
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

        private static IEnumerable<PokemonData> GetPokemonDatas()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                using (var command = new SqlCommand("select * from PokemonDatas", con))
                {
                    using (var sdr = command.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            var co = 0;
                            while (sdr.Read())
                            {
                                var data = new PokemonData
                                {
                                    Id = sdr["EnName"].ToString(),
                                    Name = sdr["JpName"].ToString()
                                };
                                yield return data;
                                co++;
                            }
                        }
                    }
                }
            }
        }

        private void SetCookie(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var cookie = new HttpCookie(key)
                {
                    Value = value,
                    Expires = DateTime.Now.AddYears(50)
                };
                Response.Cookies.Add(cookie);
            }
        }
        private string GetCookie(string key) => Request.Cookies[key]?.Value;


    }
}