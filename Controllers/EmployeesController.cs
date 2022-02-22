using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeTimeLog.Data;
using EmployeeTimeLog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace EmployeeTimeLog.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EmployeesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            return View(await _context.Employee.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            var timelogs = await _context.TimeLog.Where(x => x.EmployeeId == employee.Id).ToListAsync();

            var dto = new EmployeeTimeLogDTO()
            {
                Employee = employee,
                TimeLogs = timelogs

            };



            return View(dto);
        }

        // GET: Employees/Create
        public IActionResult Create(string userName)
        {
            if(String.IsNullOrEmpty(userName))
            {
                return View();
            }
            else
            {
                return View(new Employee()
                {
                    Username = userName
                });
            }
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,FirstName,LastName")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,FirstName,LastName")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
             .FirstOrDefaultAsync(m => m.Id == id);

            var user = await _userManager.GetUserAsync(User);
          
            if (user.UserName == employee.Username)
            {
                //blocks user from deleting own employee data
                return RedirectToAction(nameof(Index));
            }

         
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
           


            var employee = await _context.Employee.FindAsync(id);          
            
            
            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.Id == id);
        }

        private bool TimeLogExists(int id)
        {
            return _context.TimeLog.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Search(string searchText)
        { 
            return View(nameof(Index), await _context.Employee.Where(x=>x.Username.Contains(searchText) ||
            x.FirstName.ToLower().Contains(searchText.ToLower()) ||x.LastName.ToLower().Contains(searchText.ToLower())).ToListAsync());
        }


        public async Task<IActionResult> TimeLogs()
        {
            var user = await _userManager.GetUserAsync(User);

            var employee = await _context.Employee
               .FirstOrDefaultAsync(m => m.Username == user.UserName);
            if (employee == null)
            { 
                return RedirectToAction(nameof(Create),new { userName = user.UserName});
            }

            var timelogs = await _context.TimeLog.Where(x => x.EmployeeId == employee.Id).ToListAsync();

            var dto = new EmployeeTimeLogDTO()
            {
                Employee = employee,
                TimeLogs = timelogs

            };

            return View(dto);
        }

        public async Task<IActionResult> CreateTimeInLog()
        {
            var user = await _userManager.GetUserAsync(User);
            var employee = _context.Employee.FirstOrDefault(x => x.Username == user.UserName);

            if(employee != null)
            {
                var timelog = new TimeLog()
                {
                    Employee = employee,
                    EmployeeId = employee.Id,
                    TimeIn = DateTime.Now,
                    TimeOut =null
                };
                _context.Add(timelog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TimeLogs));
            }

            return NotFound();
        }

        public async Task<IActionResult> CreateTimeOutLog(int id)
        {
            var timeLog = await _context.TimeLog.FindAsync(id);

            if (ModelState.IsValid)
            {
                try
                {
                    timeLog.TimeOut = DateTime.Now;
                    _context.Update(timeLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(timeLog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(TimeLogs));
            }
            return NotFound();
        }
    }

    

}
