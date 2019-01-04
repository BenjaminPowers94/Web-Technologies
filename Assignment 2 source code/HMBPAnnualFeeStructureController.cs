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
    public class HMBPAnnualFeeStructureController : Controller
    {
        private readonly SailContext _context;

        public HMBPAnnualFeeStructureController(SailContext context)
        {
            _context = context;
        }

        // GET: HMBPAnnualFeeStructure
        public async Task<IActionResult> Index()
        {
            return View(await _context.AnnualFeeStructure.OrderByDescending(m => m.Year).ToListAsync()); // Oder by year
        }

        // GET: HMBPAnnualFeeStructure/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var annualFeeStructure = await _context.AnnualFeeStructure
                .FirstOrDefaultAsync(m => m.Year == id);
            if (annualFeeStructure == null)
            {
                return NotFound();
            }

            return View(annualFeeStructure);
        }

        // GET: HMBPAnnualFeeStructure/Create
        public IActionResult Create()
        {
            AnnualFeeStructure annualFee = _context.AnnualFeeStructure.LastOrDefault(); //preload the view with data from the most recent record on file & overlay the year with the current year
            return View(annualFee);
        }

        // POST: HMBPAnnualFeeStructure/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Year,AnnualFee,EarlyDiscountedFee,EarlyDiscountEndDate,RenewDeadlineDate,TaskExemptionFee,SecondBoatFee,ThirdBoatFee,ForthAndSubsequentBoatFee,NonSailFee,NewMember25DiscountDate,NewMember50DiscountDate,NewMember75DiscountDate")] AnnualFeeStructure annualFeeStructure)
        {
            // Throw exception with any error use when they create a new record.
            // Display the TemData error at the top of the page.
            // Also keep the variable they input before submit.
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(annualFeeStructure);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    throw new Exception();

                }
            }
            catch (Exception)
            {
                TempData["message"] = "Unable to create a new record.";
                return View(annualFeeStructure);
            }           
        }

        // GET: HMBPAnnualFeeStructure/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var annualFeeStructure = await _context.AnnualFeeStructure.FindAsync(id);
            if (annualFeeStructure == null)
            {
                return NotFound();
            }
            return View(annualFeeStructure);
        }

        // POST: HMBPAnnualFeeStructure/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Year,AnnualFee,EarlyDiscountedFee,EarlyDiscountEndDate,RenewDeadlineDate,TaskExemptionFee,SecondBoatFee,ThirdBoatFee,ForthAndSubsequentBoatFee,NonSailFee,NewMember25DiscountDate,NewMember50DiscountDate,NewMember75DiscountDate")] AnnualFeeStructure annualFeeStructure)
        {
            if (id != annualFeeStructure.Year)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Check the year of the user try to edit.
                    //If user edit the prior year, show and error in TempData and return to Index.
                    if (annualFeeStructure.Year != DateTime.Now.Year)
                    {
                        TempData["message"] = "Cannot edit the record of the prior year";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        _context.Update(annualFeeStructure);
                        await _context.SaveChangesAsync();
                    }                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnnualFeeStructureExists(annualFeeStructure.Year))
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
            return View(annualFeeStructure);
        }

        // GET: HMBPAnnualFeeStructure/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var annualFeeStructure = await _context.AnnualFeeStructure
                .FirstOrDefaultAsync(m => m.Year == id);
            if (annualFeeStructure == null)
            {
                return NotFound();
            }

            return View(annualFeeStructure);
        }

        // POST: HMBPAnnualFeeStructure/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var annualFeeStructure = await _context.AnnualFeeStructure.FindAsync(id);
            _context.AnnualFeeStructure.Remove(annualFeeStructure);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnnualFeeStructureExists(int id)
        {
            return _context.AnnualFeeStructure.Any(e => e.Year == id);
        }
    }
}
