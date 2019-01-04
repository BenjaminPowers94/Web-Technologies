using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HMSail.Models;
using Microsoft.AspNetCore.Http;

namespace HMSail.Controllers
{
    public class HMBoatsController : Controller
    {
        private readonly SailContext _context;

        public HMBoatsController(SailContext context)
        {
            _context = context;
        }

        // GET: HMBoats
        public async Task<IActionResult> Index(int? memberId)
        {
            if(memberId.HasValue)
            {
                HttpContext.Session.SetInt32("memberId", Convert.ToInt32(memberId.ToString()));
                var sailContext = _context.Boat.Include(b => b.BoatType).Include(b => b.Member).Include(b => b.ParkingCodeNavigation)
                                    .Where(b => b.MemberId == memberId).OrderBy(b => b.BoatClass);
                ViewData["heading"] = _context.Member.Include(m => m.ProvinceCodeNavigation)
                                        .Where(m => m.MemberId == memberId).First().FullName;
                return View(sailContext);
            }
            
            {
                if (HttpContext.Session.GetInt32("memberId") != null)
                {
                    var ids = Convert.ToInt32(HttpContext.Session.GetInt32("memberId"));
                    var sailContext = _context.Boat.Include(b => b.BoatType).Include(b => b.Member).Include(b => b.ParkingCodeNavigation)
                                        .Where(b => b.MemberId == ids).OrderBy(b => b.BoatClass);
                    ViewData["heading"] = _context.Member.Include(m => m.ProvinceCodeNavigation)
                                         .Where(m => m.MemberId == ids).First().FullName;
                    return View(sailContext);
                }
                else
                {
                    TempData["message"] = "Please select a member";
                    return RedirectToAction("Index", "HMMembers");
                }
            }
            //var sailContext = _context.Boat.Include(b => b.BoatType).Include(b => b.Member).Include(b => b.ParkingCodeNavigation);
            //return View(await sailContext.ToListAsync());
        }

        // GET: HMBoats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["heading"] = _context.Member.Include(m => m.ProvinceCodeNavigation)
                                       .Where(m => m.MemberId == HttpContext.Session.GetInt32("memberId")).First().FullName;
            if (id == null)
            {
                return NotFound();
            }

            var boat = await _context.Boat
                .Include(b => b.BoatType)
                .Include(b => b.Member)
                .Include(b => b.ParkingCodeNavigation)
                .FirstOrDefaultAsync(m => m.BoatId == id);
            if (boat == null)
            {
                return NotFound();
            }

            return View(boat);
        }

        // GET: HMBoats/Create
        public IActionResult Create()
        {
            ViewData["BoatTypeId"] = new SelectList(_context.BoatType.OrderBy(b => b.Name), "BoatTypeId", "Name");
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName");
            ViewData["heading"] = _context.Member.Include(m => m.ProvinceCodeNavigation)
                                       .Where(m => m.MemberId == HttpContext.Session.GetInt32("memberId")).First().FullName;
            ViewData["ParkingCode"] = new SelectList(_context.Parking.Where(p => p.ActualBoatId == null), "ParkingCode", "ParkingCode");
            return View();
        }

        // POST: HMBoats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoatId,MemberId,BoatClass,HullColour,SailNumber,HullLength,BoatTypeId,ParkingCode")] Boat boat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(boat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BoatTypeId"] = new SelectList(_context.BoatType, "BoatTypeId", "Name", boat.BoatTypeId);
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName", boat.MemberId);
            ViewData["heading"] = _context.Member.Include(m => m.ProvinceCodeNavigation)
                                       .Where(m => m.MemberId == HttpContext.Session.GetInt32("memberId")).First().FullName;
            return View(boat);
        }

        // GET: HMBoats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boat = await _context.Boat.FindAsync(id);
            if (boat == null)
            {
                return NotFound();
            }
            ViewData["BoatTypeId"] = new SelectList(_context.BoatType.OrderBy(b => b.Name), "BoatTypeId", "Name", boat.BoatTypeId);
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName", boat.MemberId);
            ViewData["heading"] = _context.Member.Include(m => m.ProvinceCodeNavigation)
                                       .Where(m => m.MemberId == HttpContext.Session.GetInt32("memberId")).First().FullName;

            if (_context.Boat.FirstOrDefault().ParkingCode == null)
            {
                ViewData["ParkingCode"] = new SelectList(_context.Parking.Where(p => p.ActualBoatId == null), "ParkingCode", "ParkingCode");
            }
            else
            {
                ViewData["ParkingCode"] = new SelectList(_context.Parking.Where(p => p.BoatTypeId == boat.BoatTypeId && p.ActualBoatId == null), "ParkingCode", "ParkingCode");
            }
            return View(boat);
        }

        // POST: HMBoats/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BoatId,MemberId,BoatClass,HullColour,SailNumber,HullLength,BoatTypeId,ParkingCode")] Boat boat)
        {
            if (id != boat.BoatId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatExists(boat.BoatId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BoatTypeId"] = new SelectList(_context.BoatType, "BoatTypeId", "Name", boat.BoatTypeId);
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName", boat.MemberId);
            ViewData["ParkingCode"] = new SelectList(_context.Parking, "ParkingCode", "ParkingCode", boat.ParkingCode);
            ViewData["heading"] = _context.Member.Include(m => m.ProvinceCodeNavigation)
                                       .Where(m => m.MemberId == HttpContext.Session.GetInt32("memberId")).First().FullName;
            return View(boat);
        }

        // GET: HMBoats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["heading"] = _context.Member.Include(m => m.ProvinceCodeNavigation)
                                       .Where(m => m.MemberId == HttpContext.Session.GetInt32("memberId")).First().FullName;
            if (id == null)
            {
                return NotFound();
            }

            var boat = await _context.Boat
                .Include(b => b.BoatType)
                .Include(b => b.Member)
                .Include(b => b.ParkingCodeNavigation)
                .FirstOrDefaultAsync(m => m.BoatId == id);
            if (boat == null)
            {
                return NotFound();
            }

            return View(boat);
        }

        // POST: HMBoats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewData["heading"] = _context.Member.Include(m => m.ProvinceCodeNavigation)
                                       .Where(m => m.MemberId == HttpContext.Session.GetInt32("memberId")).First().FullName;
            var boat = await _context.Boat.FindAsync(id);
            _context.Boat.Remove(boat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoatExists(int id)
        {
            return _context.Boat.Any(e => e.BoatId == id);
        }
    }
}
