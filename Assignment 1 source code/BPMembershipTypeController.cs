//--------------------------------------
// Ben Powers
// Assignment 1 (Controller View Basics)
// Student Number 7986250
// Monday, September 17th, 2018
//--------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BPSail.Models;

namespace BPSail.Controllers
{
    public class BPMembershipTypeController : Controller
    {
        private readonly SailContext _context;

        public BPMembershipTypeController(SailContext context)
        {
            _context = context;
        }

        // GET: BPMembershipType
        public async Task<IActionResult> Index()
        {
            return View(await _context.MembershipType.ToListAsync());
        }

        // GET: BPMembershipType/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipType = await _context.MembershipType
                .FirstOrDefaultAsync(m => m.MembershipTypeName == id);
            if (membershipType == null)
            {
                return NotFound();
            }

            return View(membershipType);
        }

        // GET: BPMembershipType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BPMembershipType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MembershipTypeName,Description,RatioToFull")] MembershipType membershipType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(membershipType);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(membershipType);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.GetBaseException().Message;
                return RedirectToAction("Index", "BPMembershipType");
            }
        }

        // GET: BPMembershipType/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipType = await _context.MembershipType.FindAsync(id);
            if (membershipType == null)
            {
                return NotFound();
            }
            return View(membershipType);
        }

        // POST: BPMembershipType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MembershipTypeName,Description,RatioToFull")] MembershipType membershipType)
        {
            try
            {
                if (id != membershipType.MembershipTypeName)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(membershipType);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!MembershipTypeExists(membershipType.MembershipTypeName))
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
                return View(membershipType);
            }
            catch(Exception ex)
            {
                TempData["error"] = ex.GetBaseException().Message;
                return RedirectToAction("Index", "BPMembershipType");
            }

        }

        // GET: BPMembershipType/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipType = await _context.MembershipType
                .FirstOrDefaultAsync(m => m.MembershipTypeName == id);
            if (membershipType == null)
            {
                return NotFound();
            }

            return View(membershipType);
        }

        // POST: BPMembershipType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var membershipType = await _context.MembershipType.FindAsync(id);
            _context.MembershipType.Remove(membershipType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MembershipTypeExists(string id)
        {
            return _context.MembershipType.Any(e => e.MembershipTypeName == id);
        }
    }
}
