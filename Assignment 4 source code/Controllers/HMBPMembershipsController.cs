using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HMBPSail.Models;
using Microsoft.AspNetCore.Http;

namespace HMSail.Controllers
{
    public class HMBPMembershipsController : Controller
    {
        private readonly SailContext _context;

        public HMBPMembershipsController(SailContext context)
        {
            _context = context;
        }

        // GET: HMBPMemberships
        public async Task<IActionResult> Index(int? memberId)
        {
            if (memberId.HasValue)
            {
                HttpContext.Session.SetInt32("memberId", Convert.ToInt32(memberId.ToString()));
                var sailContext = _context.Membership.Include(m => m.Member)
                           .Include(m => m.MembershipTypeNameNavigation)
                           .Where(m => m.MemberId == memberId)
                           .OrderByDescending(m => m.Year); // Order most recent years.
                TempData["heading"] = _context.Member.Include(b => b.ProvinceCodeNavigation).Where(b => b.MemberId == memberId).First().FullName;
                return View(sailContext);
            }
            else
            {
                if (HttpContext.Session.GetInt32("memberId") != null)
                {
                    var ids = Convert.ToInt32(HttpContext.Session.GetInt32("memberId"));
                    var sailContext = _context.Membership.Include(m => m.Member)
                           .Include(m => m.MembershipTypeNameNavigation)
                           .Where(m => m.MemberId == ids)
                           .OrderByDescending(m => m.Year); // Order most recent years.
                    TempData["heading"] = _context.Member.Include(b => b.ProvinceCodeNavigation).Where(b => b.MemberId == ids).First().FullName;
                    return View(sailContext);
                }
                else
                {
                    TempData["message"] = "Please select a member";
                    return RedirectToAction("Index", "HMMembers");
                }
            }
        }

        // GET: HMBPMemberships/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            TempData["heading"] = _context.Member.Include(b => b.ProvinceCodeNavigation).Where(b => b.MemberId == HttpContext.Session.GetInt32("memberId")).First().FullName;
            if (id == null)
            {
                return NotFound();
            }

            var membership = await _context.Membership
                .Include(m => m.Member)
                .Include(m => m.MembershipTypeNameNavigation)
                .FirstOrDefaultAsync(m => m.MembershipId == id);
            if (membership == null)
            {
                return NotFound();
            }

            return View(membership);
        }

        // GET: HMBPMemberships/Create
        public IActionResult Create()
        {
            TempData["heading"] = _context.Member.Include(b => b.ProvinceCodeNavigation).Where(b => b.MemberId == HttpContext.Session.GetInt32("memberId")).First().FullName;
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName");
            ViewData["MembershipTypeName"] = new SelectList(_context.MembershipType.OrderBy(m => m.MembershipTypeName), "MembershipTypeName", "MembershipTypeName");
            return View();
        }

        // POST: HMBPMemberships/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MembershipId,MemberId,Year,MembershipTypeName,Fee,Comments,Paid")] Membership membership)
        {
            if (ModelState.IsValid)
            {
                _context.Add(membership);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            TempData["heading"] = _context.Member.Include(b => b.ProvinceCodeNavigation).Where(b => b.MemberId == HttpContext.Session.GetInt32("memberId")).First().FullName;
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName", membership.MemberId);
            ViewData["MembershipTypeName"] = new SelectList(_context.MembershipType, "MembershipTypeName", "MembershipTypeName", membership.MembershipTypeName);
            return View(membership);
        }

        // GET: HMBPMemberships/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membership = await _context.Membership.FindAsync(id);
            if (membership == null)
            {
                return NotFound();
            }
            TempData["heading"] = _context.Member.Include(b => b.ProvinceCodeNavigation).Where(b => b.MemberId == HttpContext.Session.GetInt32("memberId")).First().FullName;
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName", membership.MemberId);
            ViewData["MembershipTypeName"] = new SelectList(_context.MembershipType, "MembershipTypeName", "MembershipTypeName", membership.MembershipTypeName);
            return View(membership);
        }

        // POST: HMBPMemberships/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MembershipId,MemberId,Year,MembershipTypeName,Fee,Comments,Paid")] Membership membership)
        {
            if (id != membership.MembershipId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Display error when user edit the pior year.
                    if (membership.Year != DateTime.Now.Year)
                    {
                        TempData["message"] = "Cannot edit the information of the pior year";
                        return RedirectToAction(nameof(Edit));
                    }
                    else
                    {
                        _context.Update(membership);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembershipExists(membership.MembershipId))
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
            TempData["heading"] = _context.Member.Include(b => b.ProvinceCodeNavigation).Where(b => b.MemberId == HttpContext.Session.GetInt32("memberId")).First().FullName;
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "FirstName", membership.MemberId);
            ViewData["MembershipTypeName"] = new SelectList(_context.MembershipType.OrderBy(m => m.MembershipTypeName), "MembershipTypeName", "MembershipTypeName", membership.MembershipTypeName);
            return View(membership);
        }

        // GET: HMBPMemberships/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            TempData["heading"] = _context.Member.Include(b => b.ProvinceCodeNavigation).Where(b => b.MemberId == HttpContext.Session.GetInt32("memberId")).First().FullName;
            if (id == null)
            {
                return NotFound();
            }

            var membership = await _context.Membership
                .Include(m => m.Member)
                .Include(m => m.MembershipTypeNameNavigation)
                .FirstOrDefaultAsync(m => m.MembershipId == id);
            if (membership == null)
            {
                return NotFound();
            }

            return View(membership);
        }

        // POST: HMBPMemberships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var membership = await _context.Membership.FindAsync(id);
            _context.Membership.Remove(membership);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MembershipExists(int id)
        {
            return _context.Membership.Any(e => e.MembershipId == id);
        }
    }
}
