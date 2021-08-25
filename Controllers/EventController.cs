using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FullCalendarMVC.Models;

namespace FullCalendarMVC.Controllers
{
    public class EventController : Controller
    {
        // Tietokantayhteys esitellään kertalleen Controllerin päätasolla
        private readonly EventDBEntities db = new EventDBEntities();


        // Palauttaa alussa staattisen näkymäsivun
        public ActionResult Index()
        {
            return View();
        }


        // Palauttaa tapahtumien datan kun jQueryllä toteutettu ajax pyyntö tulee näkymästä
        public JsonResult GetEvents()
        {
             var events = db.Event.ToList();
             return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        // Käytetään sekä uuden luontiin että olemassaolevan tapahtuman muokkaukseen
        [HttpPost]
        public JsonResult SaveEvent(Event ev)
        {
            bool status = false;

            if (ev.EventID > 0) // Jos id tieto löytyy, kyse on jo olemassaolevasta tapahtumasta
            {

                var existing = db.Event.Where(ex => ex.EventID == ev.EventID).FirstOrDefault();
                if (existing != null) // Jos id:tä vastaava rivi löytyy kannasta
                {
                    existing.Subject = ev.Subject;
                    existing.Start = ev.Start;
                    existing.End = ev.End;
                    existing.Description = ev.Description;
                    existing.IsFullDay = ev.IsFullDay;
                    existing.ThemeColor = ev.ThemeColor;
                }
            }
            else // Uuden tapahtuman lisääminen
            {
                db.Event.Add(ev); 
            }

            db.SaveChanges();
            status = true;

            return new JsonResult { Data = new { status = status } };

        }


        // Tapahtuman poisto
        [HttpPost]
        public JsonResult DeleteEvent(int id)
        {
            var status = false;
           
            {
                var ev = db.Event.Where(e => e.EventID == id).FirstOrDefault();
                if (ev != null)
                {
                    db.Event.Remove(ev);
                    db.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }

    }
}