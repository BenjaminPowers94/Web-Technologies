using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HMBPSail.Models;

namespace HMBPSail.Controllers
{
    public class HMBPMembersController : Controller
    {
        private readonly SailContext _context;

        public HMBPMembersController(SailContext context)
        {
            _context = context;
        }

        // GET: HMBPMembers
        public async Task<IActionResult> Index()
        {
            var sailContext = _context.Member.Include(m => m.ProvinceCodeNavigation).OrderBy(m => m.FullName); // order by fullname
            return View(await sailContext.ToListAsync());
        }

        // GET: HMBPMembers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .Include(m => m.ProvinceCodeNavigation)
                .FirstOrDefaultAsync(m => m.MemberId == id);
            TempData["heading"] = _context.Member.Include(m => m.ProvinceCodeNavigation)
                                    .Where(m => m.MemberId == id).First().FullName;
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: HMBPMembers/Create
        public IActionResult Create()
        {
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode");
            return View();
        }

        // POST: HMBPMembers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,FullName,FirstName,LastName,SpouseFirstName,SpouseLastName,Street,City,ProvinceCode,PostalCode,HomePhone,Email,YearJoined,Comment,TaskExempt,UseCanadaPost")] Member member)
        {
            // the try catch statement 
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(member);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "New Record Added"; // adding on success tempdata message
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                // on error add model into modelstate
                ModelState.AddModelError("", $"Error on saving create: " + $"{ex.GetBaseException().Message}");
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode", member.ProvinceCode);
            return View(member);
        }

        // GET: HMBPMembers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Province.OrderBy(p => p.Name), "ProvinceCode", "Name", member.ProvinceCode);
            TempData["heading"] = _context.Member.Include(m => m.ProvinceCodeNavigation)
                                    .Where(m => m.MemberId == id).First().FullName;
            return View(member);
        }

        // POST: HMBPMembers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberId,FullName,FirstName,LastName,SpouseFirstName,SpouseLastName,Street,City,ProvinceCode,PostalCode,HomePhone,Email,YearJoined,Comment,TaskExempt,UseCanadaPost")] Member member)
        {
            if (id != member.MemberId)
            {
                ModelState.AddModelError("", "you're not updating the record you requested");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "Edit record successful";
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!MemberExists(member.MemberId))
                    {
                        ModelState.AddModelError("", $"memberId is not on file");
                    }
                    else
                    {
                        ModelState.AddModelError("", ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Update error: {ex.GetBaseException().Message}"); //innermost
                }
                return RedirectToAction(nameof(Index));
            }
            TempData["heading"] = _context.Member.Include(m => m.ProvinceCodeNavigation)
                                    .Where(m => m.MemberId == id).First().FullName;
            ViewData["ProvinceCode"] = new SelectList(_context.Province.OrderBy(p => p.Name), "ProvinceCode", "Name", member.ProvinceCode);
            return View(member);
        }

        // GET: HMBPMembers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .Include(m => m.ProvinceCodeNavigation)
                .FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null)
            {
                return NotFound();
            }

            TempData["heading"] = _context.Member.Include(m => m.ProvinceCodeNavigation)
                                    .Where(m => m.MemberId == id).First().FullName;

            return View(member);
        }

        // POST: HMBPMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            TempData["heading"] = _context.Member.Include(m => m.ProvinceCodeNavigation)
                                    .Where(m => m.MemberId == id).First().FullName;
            var member = await _context.Member.FindAsync(id);
            _context.Member.Remove(member);
            await _context.SaveChangesAsync();
            TempData["message"] = "Record deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _context.Member.Any(e => e.MemberId == id);
        }
    }
}
