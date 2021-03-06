﻿//--------------------------------------
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
    public class BPBoatTypeController : Controller
    {
        private readonly SailContext _context;

        public BPBoatTypeController(SailContext context)
        {
            _context = context;
        }

        // GET: BPBoatType
        public async Task<IActionResult> Index()
        {
            return View(await _context.BoatType.ToListAsync());
        }

        // GET: BPBoatType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatType = await _context.BoatType
                .FirstOrDefaultAsync(m => m.BoatTypeId == id);
            if (boatType == null)
            {
                return NotFound();
            }

            return View(boatType);
        }

        // GET: BPBoatType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BPBoatType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoatTypeId,Name,Description,Chargeable,Sail,Image")] BoatType boatType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(boatType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(boatType);
        }

        // GET: BPBoatType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatType = await _context.BoatType.FindAsync(id);
            if (boatType == null)
            {
                return NotFound();
            }
            return View(boatType);
        }

        // POST: BPBoatType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BoatTypeId,Name,Description,Chargeable,Sail,Image")] BoatType boatType)
        {
            if (id != boatType.BoatTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boatType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatTypeExists(boatType.BoatTypeId))
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
            return View(boatType);
        }

        // GET: BPBoatType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatType = await _context.BoatType
                .FirstOrDefaultAsync(m => m.BoatTypeId == id);
            if (boatType == null)
            {
                return NotFound();
            }

            return View(boatType);
        }

        // POST: BPBoatType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var boatType = await _context.BoatType.FindAsync(id);
            _context.BoatType.Remove(boatType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoatTypeExists(int id)
        {
            return _context.BoatType.Any(e => e.BoatTypeId == id);
        }
    }
}
